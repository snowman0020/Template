using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Template.Domain.AppSetting;
using Template.Domain.DTO;
using Template.Helper.Line;
using Template.Infrastructure;
using Template.Service.IServices;

namespace Template.UnitTest.Test
{
    public class LineUnitTest : IClassFixture<Startup>, IDisposable
    {
        private readonly TemplateDbContext _db;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        private readonly ILine _line;
        private readonly LineData _linedata;

        public LineUnitTest(TemplateDbContext db, IMessageService messageService, IUserService userService, ILine line, IDistributedCache distributedCache, IOptions<LineData> linedata)
        {
            _messageService = messageService;
            _userService = userService;
            _line = line;
            _db = db;
            _linedata = linedata.Value;
        }

        public void Dispose()
        {
            _db.Database.EnsureDeleted();
            _db.Database.Migrate();
        }

        [Fact]
        public async Task MessageFlowTest()
        {
            //Dispose();

            string userId = "";

            var userRequestAdd = new UserRequest()
            {
                FirstName = "TestFirstName1",
                LastName = "TestLastName1",
                Phone = "0999999999",
                Email = "testFirstName@email.com",
                Password = "1234"
            };

            var userAdd = await _userService.AddUserAsync(userRequestAdd, "System_Unit_Test");

            Assert.NotNull(userAdd);
            Assert.NotNull(userAdd.Id);
            Assert.NotEmpty(userAdd.Id);

            userId = userAdd.Id;

            var message = new MessageDTO();
            message.UserID = userId;
            message.Topic = "Test topic";
            message.Detail = "Test detail";

            DateTime expiredDate = DateTime.Now;

            bool isAddSuccess = false;
            try
            {
                await _messageService.AddMessageAsync(message);
                isAddSuccess = true;
            }
            catch
            {

            }

            Assert.True(isAddSuccess);

        }

        [Fact]
        public async Task LineTest()
        {
            Dispose();

            var lineRequest = new LineRequest();
            lineRequest.to = _linedata.MessageUserId;

            Assert.NotNull(lineRequest);
            Assert.NotNull(lineRequest.to);
            Assert.NotEmpty(lineRequest.to);

            var message = new MessagesData();
            message.type = _linedata.MessageType;
            message.text = "Test send line message with message api";

            Assert.NotNull(message);
            Assert.NotNull(message.type);
            Assert.NotEmpty(message.type);

            Assert.NotNull(message.text);
            Assert.NotEmpty(message.text);

            var messageList = new List<MessagesData>();
            messageList.Add(message);

            Assert.NotNull(messageList);
            Assert.True(messageList.Count > 0);

            lineRequest.messages = messageList;

            Assert.NotNull(lineRequest.messages);
            Assert.NotEmpty(lineRequest.messages);

            var result = await _line.SendMessageAsync(lineRequest);

            Assert.NotNull(result);

            if (result.isSentSuccess)
            {
                Assert.True(result.isSentSuccess);

                Assert.NotNull(result.sentMessages);
                Assert.NotEmpty(result.sentMessages);
            }
            else
            {
                Assert.False(result.isSentSuccess);

                Assert.NotNull(result.message);
                Assert.NotEmpty(result.message);
            }
        }
    }
}
