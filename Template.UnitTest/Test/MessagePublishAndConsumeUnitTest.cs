using Microsoft.Extensions.Options;
using Template.Domain.AppSetting;
using Template.Domain.DTO;
using Template.Helper.MessageConsume;
using Template.Helper.MessagePublish;
using Template.Infrastructure.Models;

namespace Template.UnitTest.Test
{
    public class MessagePublishAndConsumeUnitTest : IClassFixture<Startup>
    {
        private readonly IMessagePublish _messagePublish; 
        private readonly IMessageConsume _messageConsume;
        private readonly RabbitMQData _rabbitMQData;

        public MessagePublishAndConsumeUnitTest(IMessagePublish messagePublish, IMessageConsume messageConsume, IOptions<RabbitMQData> rabbitMQData)
        {
            _messagePublish = messagePublish;
            _messageConsume = messageConsume;
            _rabbitMQData = rabbitMQData.Value;
        }

        [Fact]
        public async Task  MessagePublishTest()
        {
            var message = new MessageDTO();
            message.UserID = Guid.NewGuid().ToString();
            message.Topic = "Test Topic Unit Test.";
            message.Detail = $"Test Detail Unit Test user id {message.UserID}.";

            Assert.NotNull(message);

            var modelMessage = new Messages();

            message.AddToModel(modelMessage);

            Assert.NotNull(modelMessage);

            var messageNew = MessageDTO.CreateFromModel(modelMessage);

            Assert.NotNull(messageNew);

            string queueName = _rabbitMQData.QueueName ?? "";

            Assert.NotEmpty(queueName);
            Assert.NotNull(queueName);

            await _messagePublish.MessagePublishAsync(messageNew, queueName);  
        }

        //[Theory]
        //public async Task Consume(ConsumeContext<MessageDTO> consumeContext)
        //{
        //    var message = consumeContext.Message;

        //    Assert.NotNull(message);
        //}
    }
}