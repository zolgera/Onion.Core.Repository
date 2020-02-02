namespace Core.Repository.Interfaces
{
    public interface IQueryOptions
    {
        int Page { get; set; }
        int PageSize { get; set; }
        int RecordCount { get; set; }
        string Sort { get; set; }
        string SortOrder { get; set; }
        int StartRecord { get; set; }
    }
}