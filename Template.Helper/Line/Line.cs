using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;

namespace Template.Helper.Line
{
    public class Line : ILine
    {
        private readonly ILogger<Line> _logger;
        private readonly LineData _lineData;

        public Line(ILogger<Line> logger, IOptions<LineData> lineData)
        {
            _logger = logger;
            _lineData = lineData.Value;
        }
 
        public async Task<LineDTO> SendMessageAsync(LineRequest input)
        {
            _logger.LogInformation($"call: SendMessageAsync=> Start");

            var result = new LineDTO();

            bool isSentSuccess = false;

            if (input != null)
            {
                HttpClient client = new HttpClient();

                string messageUrl = _lineData.MessageUrl ?? "";
                string messageAccessToken = _lineData.MessageAccessToken ?? "";

                client.DefaultRequestHeaders.Accept.Clear();

                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + messageAccessToken);

                var messageNewFormat = JsonSerializer.Serialize(input);

                var requestContent = new StringContent(messageNewFormat, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(messageUrl, requestContent);

                response.EnsureSuccessStatusCode();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    var responseReadAsString = await response.Content.ReadAsStringAsync();

                    var responseReadAsStringConvert = JsonSerializer.Deserialize<LineResponse>(responseReadAsString);

                    if (responseReadAsStringConvert != null)
                    {
                        result.message = responseReadAsStringConvert.message ?? null;
                    }

                    _logger.LogError($"Error: {response.ReasonPhrase} status: {response.StatusCode}, message: {JsonSerializer.Serialize(response.Content.ReadAsStringAsync())}");
                }
                else
                {
                    isSentSuccess = true;
              
                    var responseReadAsString = await response.Content.ReadAsStringAsync();

                    var responseReadAsStringConvert = JsonSerializer.Deserialize<LineResponse>(responseReadAsString);

                    result.sentSuccessDate = DateTime.Now;

                    if (responseReadAsStringConvert != null)
                    {
                        result.sentMessages = responseReadAsStringConvert.sentMessages ?? null;
                    }

                    _logger.LogInformation($"Success: {response.ReasonPhrase} status: {response.StatusCode}, message: {JsonSerializer.Serialize(response.Content.ReadAsStringAsync())}");
                }

                result.isSentSuccess = isSentSuccess;

                result.message = "";

                _logger.LogDebug($"data sent success: {isSentSuccess}, request: {JsonSerializer.Serialize(input)}");
            }

            _logger.LogDebug($"data sent success: {isSentSuccess}, result: {JsonSerializer.Serialize(result)}");

            _logger.LogInformation($"call: SendMessageAsync=> Finish");

            return result;
        }
    }
}