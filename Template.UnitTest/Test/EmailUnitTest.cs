using Microsoft.EntityFrameworkCore;
using Template.Domain.DTO;
using Template.Helper.Email;
using Template.Infrastructure;

namespace Template.UnitTest.Test
{
    public class EmailUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly IEmail _email;
        private readonly TemplateDbContext _db;

        public EmailUnitTest(IEmail email, TemplateDbContext db)
        {
            _email = email;
            _db = db;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task SendEmailSmtpTextBodyTypeTest()
        {
            Dispose();

            bool isSendSuccess = false;
            try
            {
                var email = new EmailDTO()
                {
                    Subject = "Unit Test Subject",
                    Body = "Unit Test Body",
                    BodyType = Domain.DTO.BodyType.TEXT,
                    EmailType = EmailType.SMTP
                };

                Assert.NotNull(email);

                var sendTo = new SendTo()
                {
                    Email = "unitTestSend@email.com"
                };

                Assert.NotNull(sendTo);

                var ccTo = new CcTo()
                {
                    Email = "unitTestCc@email.com"
                };

                Assert.NotNull(ccTo);

                var sendToList = new List<SendTo>();
                sendToList.Add(sendTo);

                Assert.NotNull(sendToList);
                Assert.NotEmpty(sendToList);

                var ccToList = new List<CcTo>();
                ccToList.Add(ccTo);

                Assert.NotNull(ccToList);
                Assert.NotEmpty(ccToList);

                email.SendTo = sendToList;
                email.CcTo = ccToList;

                await _email.SendEmailAsync(email);

                isSendSuccess = true;
            }
            catch
            {

            }

            Assert.True(isSendSuccess);
        }

        [Fact]
        public async Task SendEmailGraphApiTextBodyTypeTest()
        {
            Dispose();

            bool isSendSuccess = false;
            try
            {
                var email = new EmailDTO()
                {
                    Subject = "Unit Test Subject",
                    Body = "Unit Test Body",
                    BodyType = Domain.DTO.BodyType.TEXT,
                    EmailType = EmailType.GRAPH_API
                };

                Assert.NotNull(email);

                var sendTo = new SendTo()
                {
                    Email = "unitTestSend@email.com"
                };

                Assert.NotNull(sendTo);

                var ccTo = new CcTo()
                {
                    Email = "unitTestCc@email.com"
                };

                Assert.NotNull(ccTo);

                var sendToList = new List<SendTo>();
                sendToList.Add(sendTo);

                Assert.NotNull(sendToList);
                Assert.NotEmpty(sendToList);

                var ccToList = new List<CcTo>();
                ccToList.Add(ccTo);

                Assert.NotNull(ccToList);
                Assert.NotEmpty(ccToList);

                email.SendTo = sendToList;
                email.CcTo = ccToList;

                await _email.SendEmailAsync(email);

                isSendSuccess = true;
            }
            catch
            {

            }

            Assert.True(isSendSuccess);
        }


        [Fact]
        public async Task SendEmailSmtpHtmlBodyTypeTest()
        {
            Dispose();

            bool isSendSuccess = false;
            try
            {
                var email = new EmailDTO()
                {
                    Subject = "Unit Test Subject",
                    BodyType = Domain.DTO.BodyType.HTML,
                    EmailType = EmailType.SMTP
                };

                Assert.NotNull(email);

                var sendTo = new SendTo()
                {
                    Email = "unitTestSend@email.com"
                };

                Assert.NotNull(sendTo);

                var ccTo = new CcTo()
                {
                    Email = "unitTestCc@email.com"
                };

                Assert.NotNull(ccTo);

                var sendToList = new List<SendTo>();
                sendToList.Add(sendTo);

                Assert.NotNull(sendToList);
                Assert.NotEmpty(sendToList);

                var ccToList = new List<CcTo>();
                ccToList.Add(ccTo);

                Assert.NotNull(ccToList);
                Assert.NotEmpty(ccToList);

                email.SendTo = sendToList;
                email.CcTo = ccToList;

                var baseDirectory = AppContext.BaseDirectory;
                string emailTest = File.ReadAllText(Path.Combine(baseDirectory, "Email.html"));
                emailTest = emailTest.Replace("{subject}", email.Subject);
                emailTest = emailTest.Replace("{sendTo}", email.SendTo != null ? string.Join(",", email.SendTo.Select(s => s.Name)) : "");
                emailTest = emailTest.Replace("{ccTo}", email.CcTo != null ? string.Join(",", email.CcTo.Select(s => s.Name)) : "");
                emailTest = emailTest.Replace("{bodyType}", email.BodyType.ToString());
                emailTest = emailTest.Replace("{emailType}", email.EmailType.ToString());
                emailTest = emailTest.Replace("{body}", "Unit Test Body");

                email.Body = emailTest;

                await _email.SendEmailAsync(email);

                isSendSuccess = true;
            }
            catch
            {

            }

            Assert.True(isSendSuccess);
        }

        [Fact]
        public async Task SendEmailGraphApiHtmlBodyTypeTest()
        {
            Dispose();

            bool isSendSuccess = false;
            try
            {
                var email = new EmailDTO()
                {
                    Subject = "Unit Test Subject",
                    BodyType = Domain.DTO.BodyType.TEXT,
                    EmailType = EmailType.GRAPH_API
                };

                Assert.NotNull(email);

                var sendTo = new SendTo()
                {
                    Email = "unitTestSend@email.com"
                };

                Assert.NotNull(sendTo);

                var ccTo = new CcTo()
                {
                    Email = "unitTestCc@email.com"
                };

                Assert.NotNull(ccTo);

                var sendToList = new List<SendTo>();
                sendToList.Add(sendTo);

                Assert.NotNull(sendToList);
                Assert.NotEmpty(sendToList);

                var ccToList = new List<CcTo>();
                ccToList.Add(ccTo);

                Assert.NotNull(ccToList);
                Assert.NotEmpty(ccToList);

                email.SendTo = sendToList;
                email.CcTo = ccToList;

                var baseDirectory = AppContext.BaseDirectory;
                string emailTest = File.ReadAllText(Path.Combine(baseDirectory, "Email.html"));
                emailTest = emailTest.Replace("{subject}", email.Subject);
                emailTest = emailTest.Replace("{sendTo}", email.SendTo != null ? string.Join(",", email.SendTo.Select(s => s.Name)) : "");
                emailTest = emailTest.Replace("{ccTo}", email.CcTo != null ? string.Join(",", email.CcTo.Select(s => s.Name)) : "");
                emailTest = emailTest.Replace("{bodyType}", email.BodyType.ToString());
                emailTest = emailTest.Replace("{emailType}", email.EmailType.ToString());
                emailTest = emailTest.Replace("{body}", "Unit Test Body");

                email.Body = emailTest;

                await _email.SendEmailAsync(email);

                isSendSuccess = true;
            }
            catch
            {

            }

            Assert.True(isSendSuccess);
        }
    }
}