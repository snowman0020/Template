namespace Template.Helper.BackgroundWorkJob
{
    public interface IBackgroundWorkJob
    {
        Task StartAsync(CancellationToken cancellationToken);

        Task StopAsync(CancellationToken cancellationToken);
    }
}