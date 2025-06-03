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
        public async Task SendEmailSmtpTest()
        {
            bool isSendSuccess = false;
            try
            {
                var email = new EmailDTO()
                {
                    Subject = "Unit Test Subject",
                    Body = "Unit Test Body",
                    BodyType = BodyType.TEXT,
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
        public async Task SendEmailGraphApiTest()
        {
            bool isSendSuccess = false;
            try
            {
                var email = new EmailDTO()
                {
                    Subject = "Unit Test Subject",
                    Body = "Unit Test Body",
                    BodyType = BodyType.TEXT,
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
    }
}