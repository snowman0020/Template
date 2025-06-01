using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace Template.Helper.DataProtected
{
    public class DataProtected : IDataProtected
    {
        private readonly ILogger<DataProtected> _logger;
        private readonly IDataProtector _dataProtector;

        public DataProtected(ILogger<DataProtected> logger, IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _dataProtector = dataProtectionProvider.CreateProtector("Template.Data.Protection");
        }

        public string ProtectInformation(string sensitiveData)
        {
            _logger.LogInformation($"call: ProtectInformation");

            var protectedData = _dataProtector.Protect(sensitiveData);

            _logger.LogDebug($"data: {protectedData}");

            return protectedData;
        }

        public string UnProtectInformantion(string protectedData)
        {
            _logger.LogInformation($"call: UnProtectInformantion");

            var sensitiveData = _dataProtector.Unprotect(protectedData);

            //_logger.LogDebug($"data: {sensitiveData}");

            return sensitiveData;
        }
    }
}