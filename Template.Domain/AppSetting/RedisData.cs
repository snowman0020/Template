namespace Template.Domain.AppSetting
{
    public class RedisData
    {
        public string? CacheId { get; set; }
        public string? InstanceName { get; set; }
        public int AbsoluteExpirationRelativeToNow { get; set; }
        public int SlidingExpiration { get; set; }
        public string? ConnectionServer { get; set; }
    }
}