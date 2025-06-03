using Template.Domain.DTO;

namespace Template.Helper.Email
{
    public interface IEmail
    {
        Task SendEmailAsync(EmailDTO input);
    }
}