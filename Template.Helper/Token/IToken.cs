using Template.Domain.DTO;

namespace Template.Helper.Token
{
    public interface IToken
    {
        LoginDTO CreateNewToken(string userId, string email, string issuer, int expiryMinutes, string key);
    }
}