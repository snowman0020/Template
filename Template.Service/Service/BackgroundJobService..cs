using Hangfire;
using Microsoft.Extensions.Logging;
using Template.Service.IServices;

namespace Template.Service.Services
{
    public class BackgroundJobService
    {
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IMessageService _messageService;
        private readonly IRecurringJobManager _recurringJobManager;

        public BackgroundJobService(ILogger<BackgroundJobService> logger, IMessageService messageService, IRecurringJobManager recurringJobManager)
        {
            _logger = logger;
            _messageService = messageService;
            _recurringJobManager = recurringJobManager;
        }

        public void ExecuteRecurringJob(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"call: BackgroundJob CheckMessageAsync=> Start");
            _recurringJobManager.AddOrUpdate("CheckMessageAsync", () => _messageService.CheckMessageAsync(), Cron.Minutely);
            if (Task.CompletedTask.IsCompleted)
            {
                _logger.LogInformation($"call: BackgroundJob CheckMessageAsync=> Finish");
            }
        }
    }
}