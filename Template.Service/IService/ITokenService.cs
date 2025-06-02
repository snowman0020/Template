using Template.Domain.DTO;

namespace Template.Service.IServices
{
    public interface ITokenService
    {
        Task<LoginDTO> LoginAsync(LoginRequest input);
        Task LogoutAsync(LogoutRequest input);
        Task<LoginCacheDTO> CheckDataInCacheAsync(LoginRequest input);
    }
}