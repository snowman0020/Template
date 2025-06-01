using Template.Domain.DTO;

namespace Template.Helper.MessagePublish
{
    public interface IMessagePublish
    {
        Task MessagePublishAsync(MessageDTO message, string queueName);
    }
}