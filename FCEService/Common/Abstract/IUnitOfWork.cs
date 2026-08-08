namespace FCEService.Common.Abstract
{
    public interface IUnitOfWork
    {
        Task CreateSavePoint(string SavePointName, CancellationToken cs);
        Task ExecuteAsync(Func<Task> action, CancellationToken cs);
    }
}
