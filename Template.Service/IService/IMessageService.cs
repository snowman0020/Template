using Template.Domain.DTO;

namespace Template.Service.IServices
{
    public interface IMessageService
    {
        Task AddMessageAsync(MessageDTO input);
    }
}