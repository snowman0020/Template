using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;
using Template.Helper.ErrorException;
using Template.Helper.PasswordHash;
using Template.Helper.Token;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<TokenService> _logger;
        private readonly JWTData _jwtData;
        private readonly IPasswordHash _passwordHash;
        private readonly IToken _token;

        public TokenService(TemplateDbContext db, ILogger<TokenService> logger, IOptions<JWTData> jwtData, IPasswordHash passwordHash, IToken token)
        {
            _db = db;
            _logger = logger;
            _jwtData = jwtData.Value;
            _passwordHash = passwordHash;
            _token = token;
        }

        public async Task<LoginDTO> LoginAsync(LoginRequest input)
        {
            var result = new LoginDTO();

            _logger.LogInformation($"call: LoginAsync: {JsonSerializer.Serialize(input)}");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {

                    string issuer = _jwtData.Issuer ?? "";
                    int expiryMinutes = _jwtData.ExpMinutes ?? 15;
                    string key = _jwtData.Key ?? "";

                    if (input.GrantType == GrantTypeStatus.PASSWORD)
                    {
                        _logger.LogInformation("Grant Type: Password");

                        string email = input.Email ?? "";
                        string password = input.Password ?? "";

                        var modelUser = await _db.Users.Where(u => u.Email == email && u.IsDeleted == false).FirstOrDefaultAsync();

                        if (modelUser == null)
                        {
                            Error.Status = ErrorStatus.UN_AUTHORIZED;
                            Error.Title = "Email not found.";
                            Error.Message = "";

                            throw new ErrorException();
                        }

                        string passwordEncrypt = _passwordHash.Encrypt(password);

                        if (!_passwordHash.Verify(passwordEncrypt, password))
                        {
                            Error.Status = ErrorStatus.UN_AUTHORIZED;
                            Error.Title = "Password wrong.";
                            Error.Message = "";

                            throw new ErrorException();
                        }

                        string userId = modelUser.ID ?? "";

                        var createNewToken = _token.CreateNewToken(userId, email, issuer, expiryMinutes, key);

                        var token = new Tokens();

                        createNewToken.AddToModel(token, expiryMinutes);

                        _db.Tokens.Add(token);
                        await _db.SaveChangesAsync();

                        transaction.Commit();

                        _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                        result = await GetTokenByEmailAsync(email);
                        result.GrantType = input.GrantType;
                        result.Expires = createNewToken.Expires;
                    }
                    else if (input.GrantType == GrantTypeStatus.REFRESH_TOKEN)
                    {
                        _logger.LogInformation("Grant Type: Refresh Token");

                        if (string.IsNullOrEmpty(input.RefreshToken))
                        {
                            Error.Status = ErrorStatus.UN_AUTHORIZED;
                            Error.Title = "Refresh token not empty.";
                            Error.Message = "";

                            throw new ErrorException();
                        }

                        string refreshToken = input.RefreshToken ?? "";

                        var modelToken = await _db.Tokens.Where(t => t.RefreshToken == refreshToken).FirstOrDefaultAsync();

                        if (modelToken == null)
                        {
                            Error.Status = ErrorStatus.UN_AUTHORIZED;
                            Error.Title = "Refresh token not found.";
                            Error.Message = "";

                            throw new ErrorException();
                        }

                        if (modelToken.ExpiredDate < DateTime.Now)
                        {
                            Error.Status = ErrorStatus.UN_AUTHORIZED;
                            Error.Title = "Refresh token has been expired.";
                            Error.Message = "";

                            throw new ErrorException();
                        }

                        string email = modelToken.Email ?? "";

                        var modelUser = await _db.Users.Where(u => u.Email == email).FirstOrDefaultAsync();

                        if (modelUser == null)
                        {
                            Error.Status = ErrorStatus.UN_AUTHORIZED;
                            Error.Title = "Email not found.";
                            Error.Message = "";

                            throw new ErrorException();
                        }

                        string userId = modelUser.ID ?? "";

                        var createRefreshToken = _token.CreateNewToken(userId, email, issuer, expiryMinutes, key);

                        _db.Remove(modelToken);
                        await _db.SaveChangesAsync();

                        var token = new Tokens();

                        createRefreshToken.AddToModel(token, expiryMinutes);

                        _db.Tokens.Add(token);
                        await _db.SaveChangesAsync();

                        transaction.Commit();

                        _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                        result = await GetTokenByEmailAsync(email);
                        result.GrantType = input.GrantType;
                        result.Expires = createRefreshToken.Expires;
                    }
                }
                catch (ErrorException)
                {
                    transaction.Rollback();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Can not login.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }

            return result;
        }

        public async Task LogoutAsync(LogoutRequest input)
        {
            var result = new UserDTO();

            _logger.LogInformation($"call: LogoutAsync: {JsonSerializer.Serialize(input)}");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var modelToken = await _db.Tokens.Where(t => t.RefreshToken == input.RefreshToken).FirstOrDefaultAsync();

                    if (modelToken == null)
                    {
                        Error.Status = ErrorStatus.UN_AUTHORIZED;
                        Error.Title = "token not found.";
                        Error.Message = "";

                        throw new ErrorException();
                    }

                    _db.Remove(modelToken);
                    await _db.SaveChangesAsync();

                    transaction.Commit();
                }
                catch (ErrorException)
                {
                    transaction.Rollback();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Can not update data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }
        }

        private async Task<LoginDTO> GetTokenByEmailAsync(string email)
        {
            var result = new LoginDTO();

            try
            {
                _logger.LogInformation($"call: GetTokenByEmailAsync: {email}");

                var modelToken = await _db.Tokens.Where(t => t.Email == email).AsNoTracking().FirstOrDefaultAsync();

                if (modelToken == null)
                {
                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Data not found.";
                    Error.Message = "";

                    throw new ErrorException();
                }

                _logger.LogDebug($"data before createFromModel: {JsonSerializer.Serialize(modelToken)}");

                result = LoginDTO.CreateFromModel(modelToken);

                _logger.LogDebug($"data after createFromModel: {JsonSerializer.Serialize(result)}");
            }
            catch (ErrorException)
            {
                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }
            catch (Exception ex)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Can not search by email.";
                Error.Message = ex.Message.ToString();

                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }

            return result;
        }
    }
}