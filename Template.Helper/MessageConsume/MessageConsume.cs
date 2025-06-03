using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;

namespace Template.Helper.MessageConsume
{
    public class MessageConsume : IMessageConsume, IConsumer<MessageDTO>
    {
        private readonly ILogger<MessageConsume> _logger;
        private readonly CustomSettingData _customSettingData;

        public MessageConsume(ILogger<MessageConsume> logger, IOptions<CustomSettingData> customSettingData)
        {
            _logger = logger;
            _customSettingData = customSettingData.Value;
        }

        public async Task Consume(ConsumeContext<MessageDTO> consumeContext)
        {
            _logger.LogInformation($"call: MessageConsumeAsync=> Start");

            _logger.LogDebug($"Id: {consumeContext.MessageId}, message: {JsonSerializer.Serialize(consumeContext.Message)}");

            //await Task.Run(() => CallApiAddMessageAsync(consumeContext.Message));

            await CallApiAddMessageAsync(consumeContext.Message);

            _logger.LogInformation($"call: MessageConsumeAsync=> Finish");
        }

        private async Task CallApiAddMessageAsync(MessageDTO message)
        {
            _logger.LogInformation($"call: CallApiAddMessageAsync=> Start");

            _logger.LogDebug($"data: {JsonSerializer.Serialize(message)}");

            HttpClient client = new HttpClient();

            string mainUrl = _customSettingData.MainUrl ?? "";

            string addMessageApi = $"{mainUrl}/api/Message";

            client.DefaultRequestHeaders.Accept.Clear();

            var messageNewFormat = JsonSerializer.Serialize(message);

            var requestContent = new StringContent(messageNewFormat, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(addMessageApi, requestContent);

            response.EnsureSuccessStatusCode();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogError($"Error: {response.ReasonPhrase} status: {response.StatusCode}, message: {JsonSerializer.Serialize(response.Content.ReadAsStringAsync())}");
            }
            else
            {
                _logger.LogInformation($"Success: {response.ReasonPhrase} status: {response.StatusCode}, message: {JsonSerializer.Serialize(response.Content.ReadAsStringAsync())}");
            }

            _logger.LogInformation($"call: CallApiAddMessageAsync=> Finish");
        }
    }
}