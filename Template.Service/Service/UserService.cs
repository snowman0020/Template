using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Template.Domain.DTO;
using Template.Domain.Filter;
using Template.Domain.Paging;
using Template.Domain.SortBy;
using Template.Helper.ErrorException;
using Template.Helper.PasswordHash;
using Template.Infrastructure;
using Template.Infrastructure.Models;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class UserService : IUserService
    {
        private readonly TemplateDbContext _db;
        private readonly ILogger<UserService> _logger;
        private readonly IPasswordHash _passwordHash;

        public UserService(TemplateDbContext db, ILogger<UserService> logger, IPasswordHash passwordHash)
        {
            _db = db;
            _logger = logger;
            _passwordHash = passwordHash;
        }

        public async Task<PageList<UserDTO>> GetUserListAsync(UserFilter? filter, PageParam pageParam, UserSortBy? sortBy)
        {
            try
            {
                _logger.LogInformation($"call: GetUserListAsync: filter: {JsonSerializer.Serialize(filter)}, pageParam: {JsonSerializer.Serialize(pageParam)}, sortBy: {JsonSerializer.Serialize(sortBy)}");

                var queryUser = _db.Users.AsNoTracking();

                #region Filter
                if (filter != null)
                {
                    if (!string.IsNullOrEmpty(filter.FullName))
                    {
                        _logger.LogInformation("filter: FullName");
                        string fullName = filter.FullName;
                        queryUser = queryUser.Where(qu => qu.FirstName == fullName || qu.LastName == fullName);
                    }
                    if (!string.IsNullOrEmpty(filter.Phone))
                    {
                        _logger.LogInformation("filter: Phone");
                        string phone = filter.Phone;
                        queryUser = queryUser.Where(qu => qu.Phone == phone);
                    }
                    if (!string.IsNullOrEmpty(filter.Email))
                    {
                        _logger.LogInformation("filter: email");
                        string email = filter.Email;
                        queryUser = queryUser.Where(qu => qu.Email == email);
                    }

                    _logger.LogDebug($"filter: data: {JsonSerializer.Serialize(queryUser.ToList())}");
                }
                #endregion

                #region SortBy
                if (sortBy != null)
                {
                    switch (sortBy.UserEnumSortBy)
                    {
                        case UserEnumSortBy.FullName:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: FullName => Ascending");
                                queryUser = queryUser.OrderBy(qu => qu.FirstName).OrderBy(o => o.LastName);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: FullName => Descending");
                                queryUser = queryUser.OrderByDescending(qu => qu.FirstName).OrderByDescending(o => o.LastName);
                            }
                            break;
                        case UserEnumSortBy.Phone:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: Phone => Ascending");
                                queryUser = queryUser.OrderBy(qu => qu.Phone);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: Phone => Descending");
                                queryUser = queryUser.OrderByDescending(qu => qu.Phone);
                            }
                            break;
                        case UserEnumSortBy.Email:
                            if (sortBy.Ascending)
                            {
                                _logger.LogInformation("sortBy: Email => Ascending");
                                queryUser = queryUser.OrderBy(qu => qu.Email);
                            }
                            else
                            {
                                _logger.LogInformation("sortBy: Email => Descending");
                                queryUser = queryUser.OrderByDescending(qu => qu.Email);
                            }
                            break;
                    }

                    _logger.LogDebug($"sortBy: data: {JsonSerializer.Serialize(queryUser.ToList())}");
                }
                else
                {
                    _logger.LogInformation("sortBy default: OrderNumber => Ascending");

                    queryUser = queryUser.OrderBy(qu => qu.OrderNumber);

                    _logger.LogDebug($"sortBy default: data: {JsonSerializer.Serialize(queryUser.ToList())}");
                }
                #endregion

                #region Paging
                var queryUserResult = await PageList<Users>.ToModelList(queryUser, pageParam);

                var userListDTO = queryUserResult.Select(qur => UserDTO.CreateFromModel(qur)).ToList();

                var result = PageList<UserDTO>.ToPagedList(userListDTO, pageParam);
                #endregion

                _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

                return result;
            }
            catch (Exception ex)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Can not get user list.";
                Error.Message = ex.Message.ToString();

                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }
        }

        public async Task<UserDTO> GetUserByIdAsync(string Id)
        {
            var result = new UserDTO();

            try
            {
                _logger.LogInformation($"call: GetUserByIdAsync: {Id}");

                var modelUser = await _db.Users.Where(u => u.ID == Id && u.IsDeleted == false).AsNoTracking().FirstOrDefaultAsync();

                if (modelUser == null)
                {
                    Error.Status = ErrorStatus.BAD_REQUEST;
                    Error.Title = "Data not found.";
                    Error.Message = "";

                    throw new ErrorException();
                }
                else
                {
                    if (modelUser.IsDeleted)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data has deleted.";
                        Error.Message = "";

                        throw new ErrorException();
                    }
                }

                _logger.LogDebug($"data before createFromModel: {JsonSerializer.Serialize(modelUser)}");

                result = UserDTO.CreateFromModel(modelUser);

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
                Error.Title = "Can not search by id.";
                Error.Message = ex.Message.ToString();

                _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                throw;
            }

            return result;
        }

        public async Task<UserDTO> AddUserAsync(UserRequest input)
        {
            var result = new UserDTO();

            _logger.LogInformation($"call: AddUserAsync: {JsonSerializer.Serialize(input)}");

            var modelUser = await _db.Users.Where(u => u.FirstName == input.FirstName && u.LastName == input.LastName).AsNoTracking().FirstOrDefaultAsync();

            if (modelUser != null)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Firstname and Lastname duplicate.";
                Error.Message = "";

                throw new ErrorException();
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var user = new Users();

                    input.AddToModel(user);

                    if (!string.IsNullOrEmpty(input.Password))
                    {
                        user.Password = _passwordHash.Encrypt(input.Password);
                    }

                    await _db.Users.AddAsync(user);
                    await _db.SaveChangesAsync();

                    string userId = user.ID ?? "";

                    result = await GetUserByIdAsync(userId);

                    _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

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
                    Error.Title = "Can not add data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }

            return result;
        }

        public async Task<UserDTO> UpdateUserAsync(string Id, UserRequest input)
        {
            var result = new UserDTO();

            _logger.LogInformation($"call: UpdateUserAsync: {Id}, {JsonSerializer.Serialize(input)}");

            if (string.IsNullOrEmpty(Id))
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Id is required.";
                Error.Message = "";

                throw new ErrorException();
            }

            var modelCheckUser = await _db.Users.Where(u => u.FirstName == input.FirstName && u.LastName == input.LastName && u.ID != Id && u.IsDeleted == true).FirstOrDefaultAsync();

            if (modelCheckUser != null)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Firstname and Lastname duplicate.";
                Error.Message = "";

                throw new ErrorException();
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var modelUser = await _db.Users.Where(u => u.ID == Id && u.IsDeleted == false).FirstOrDefaultAsync();

                    if (modelUser == null)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data not found.";
                        Error.Message = "";

                        throw new ErrorException();
                    }

                    if (modelUser == null)
                    {
                        Error.Status = ErrorStatus.BAD_REQUEST;
                        Error.Title = "Data not found.";
                        Error.Message = "";

                        throw new ErrorException();
                    }
                    else
                    {
                        if (modelUser.IsDeleted)
                        {
                            Error.Status = ErrorStatus.BAD_REQUEST;
                            Error.Title = "Data has deleted.";
                            Error.Message = "";

                            throw new ErrorException();
                        }
                    }

                    input.UpdateToModel(modelUser);

                    if (!string.IsNullOrEmpty(input.Password))
                    {
                        modelUser.Password = _passwordHash.Encrypt(input.Password);
                    }

                    _db.Users.Update(modelUser);
                    await _db.SaveChangesAsync();

                    result = await GetUserByIdAsync(Id);

                    _logger.LogDebug($"data: {JsonSerializer.Serialize(result)}");

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

            return result;
        }

        public async Task DeleteUserAsync(string Id)
        {
            _logger.LogInformation($"call: DeleteUserAsync: {Id}");

            var model = await _db.Users.Where(m => m.ID == Id && m.IsDeleted == false).FirstOrDefaultAsync();

            if (model == null)
            {
                Error.Status = ErrorStatus.BAD_REQUEST;
                Error.Title = "Data not found.";
                Error.Message = "";

                throw new ErrorException();
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var input = new UserDTO();

                    input.DeleteToModel(model);

                    _db.Users.Update(model);
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
                    Error.Title = "Can not delete data.";
                    Error.Message = ex.Message.ToString();

                    _logger.LogError($"error: Status: {Error.Status}, Title: {Error.Title}, Message: {Error.Message}");

                    throw new ErrorException();
                }
            }
        }
    }
}