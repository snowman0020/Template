using MassTransit;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Template.Domain.DTO;

namespace Template.Helper.MessagePublish
{
    public class MessagePublish : IMessagePublish
    {
        private readonly ILogger<MessagePublish> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public MessagePublish(ILogger<MessagePublish> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task MessagePublishAsync(MessageDTO message, string queueName)
        {
            _logger.LogInformation($"call: MessagePublishAsync=> Start");

            await _publishEndpoint.Publish<MessageDTO>(message);

            _logger.LogDebug($"message: {JsonSerializer.Serialize(message)}, queueName: {queueName}");

            _logger.LogInformation($"call: MessagePublishAsync=> Finish");
        }
    }
}