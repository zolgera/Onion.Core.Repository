using Core.Data.Interfaces.Audit;

namespace Core.Repository.Interfaces
{
    public interface IAudit
    {
        T StampCreated<T>(T model) where T :IAuditable;
        T StampModifed<T>(T model) where T : IAuditable;
    }
}