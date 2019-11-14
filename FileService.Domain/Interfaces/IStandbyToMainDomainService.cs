namespace FileService.Domain.Interfaces
{
    public interface IStandbyToMainDomainService
    {
        // move n files to main service, n equals to NumOfWorkers in config
        void MoveToMain();
    }
}