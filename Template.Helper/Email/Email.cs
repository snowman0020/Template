using Azure.Identity;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using MimeKit;
using System.Text.Json;
using Template.Domain.AppSetting;
using Template.Domain.DTO;

namespace Template.Helper.Email
{
    public class Email : IEmail
    {
        private readonly ILogger<Email> _logger;
        private readonly EmailData _emailData;

        public Email(ILogger<Email> logger, IOptions<EmailData> emailData)
        {
            _logger = logger;
            _emailData = emailData.Value;
        }

        public async Task SendEmailAsync(EmailDTO input)
        {
            _logger.LogInformation($"call: SendEmailAsync=> Start");

            var mimeMessage = new MimeMessage();

            string sendToConvertToJson = "";
            string ccToConvertToJson = "";

            string subject = input.Subject ?? "";
            string body = input.Body ?? "";
            var bodyType = input.BodyType;

            if (input.EmailType == EmailType.SMTP)
            {
                _logger.LogInformation($"email type: SMTP");

                var builder = new BodyBuilder();

                mimeMessage.Subject = subject;

                switch (bodyType)
                {
                    case Domain.DTO.BodyType.HTML: builder.HtmlBody = body; _logger.LogInformation($"body type: HTML"); break;
                    case Domain.DTO.BodyType.TEXT: builder.TextBody = body; _logger.LogInformation($"body type: TEXT"); break;
                }

                mimeMessage.Body = builder.ToMessageBody();

                string server = "";
                int port = 587;
                string username = "";
                string password = "";
                bool passwordEnable = true;

                if (_emailData.Smtp != null && _emailData.Smtp.Server != null)
                {
                    server = _emailData.Smtp.Server ?? "";
                    port = _emailData.Smtp.Port;
                    username = _emailData.Smtp.Username ?? "";
                    password = _emailData.Smtp.Password ?? "";
                    passwordEnable = _emailData.Smtp.PasswordEnable;
                }

                if (input.SendTo != null && input.SendTo.Count > 0)
                {
                    input.SendTo.ForEach(s => mimeMessage.To.Add(new MailboxAddress(s.Name, s.Email)));
                    sendToConvertToJson = JsonSerializer.Serialize(input.SendTo);
                }

                if (input.CcTo != null && input.CcTo.Count > 0)
                {
                    input.CcTo.ForEach(s => mimeMessage.Cc.Add(new MailboxAddress(s.Name, s.Email)));
                    ccToConvertToJson = JsonSerializer.Serialize(input.CcTo);
                }

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(server, port);

                    if (passwordEnable)
                    {
                        await client.AuthenticateAsync(username, password);
                    }

                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);

                    _logger.LogDebug($"email: {JsonSerializer.Serialize(mimeMessage)}");
                }
            }
            else if (input.EmailType == EmailType.GRAPH_API)
            {
                _logger.LogInformation($"email type: GRAPH_API");

                string tenantId = "";
                string clientId = "";
                string clientSecret = "";
                string principalId = "";
                string principalName = "";
                string url = "";

                if (_emailData.GraphApi != null && _emailData.GraphApi.TenantId != null)
                {
                    tenantId = _emailData.GraphApi.TenantId ?? "";
                    clientId = _emailData.GraphApi.ClientId ?? "";
                    clientSecret = _emailData.GraphApi.ClientSecret ?? "";
                    principalId = _emailData.GraphApi.PrincipalId ?? "";
                    principalName = _emailData.GraphApi.PrincipalName ?? "";
                    url = _emailData.GraphApi.Url ?? "";
                }

                var scopes = new[] { url };

                var options = new TokenCredentialOptions
                {
                    AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
                };

                var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);
                var graphServiceClient = new GraphServiceClient(clientSecretCredential, scopes);

                var emailBody = new SendMailPostRequestBody();

                var itemBody = new ItemBody();
                itemBody.Content = body;

                var message = new Message();
                message.Subject = subject;
                message.Body = itemBody;

                emailBody.Message = message;
                emailBody.SaveToSentItems = true;

                switch (bodyType)
                {
                    case Domain.DTO.BodyType.HTML: emailBody.Message.Body.ContentType = Microsoft.Graph.Models.BodyType.Html; _logger.LogInformation($"body type: HTML"); break;
                    case Domain.DTO.BodyType.TEXT: emailBody.Message.Body.ContentType = Microsoft.Graph.Models.BodyType.Text; _logger.LogInformation($"body type: TEXT"); break;
                }

                if (input.SendTo != null && input.SendTo.Count > 0)
                {
                    var sendTo = new List<Recipient>();

                    input.SendTo.ForEach(g => sendTo.Add(new Recipient()
                    {
                        EmailAddress = new EmailAddress
                        {
                            Name = g.Name,
                            Address = g.Email
                        }
                    }));

                    emailBody.Message.ToRecipients = sendTo;

                    sendToConvertToJson = JsonSerializer.Serialize(input.SendTo);
                }

                if (input.CcTo != null && input.CcTo.Count > 0)
                {
                    var ccTo = new List<Recipient>();

                    input.CcTo.ForEach(g => ccTo.Add(new Recipient()
                    {
                        EmailAddress = new EmailAddress
                        {
                            Name = g.Name,
                            Address = g.Email
                        }
                    }));

                    emailBody.Message.CcRecipients = ccTo;

                    ccToConvertToJson = JsonSerializer.Serialize(input.CcTo);
                }

                await graphServiceClient.Users[principalName].SendMail.PostAsync(emailBody);

                _logger.LogDebug($"email: {JsonSerializer.Serialize(emailBody)}");
            }

            _logger.LogDebug($"data: {JsonSerializer.Serialize(input)}");

            _logger.LogInformation($"call: SendEmailAsync=> Finish");
        }
    }
}