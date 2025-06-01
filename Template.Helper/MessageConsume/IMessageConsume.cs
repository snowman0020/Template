using MassTransit;
using Template.Domain.DTO;

namespace Template.Helper.MessageConsume
{
    public interface IMessageConsume
    {
        Task Consume(ConsumeContext<MessageDTO> consumeContext);
    }
}