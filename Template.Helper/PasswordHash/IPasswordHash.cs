namespace Template.Helper.PasswordHash
{
    public interface IPasswordHash
    {
        string Encrypt(string password);

        bool Verify(string passwordHash, string password);
    }
}