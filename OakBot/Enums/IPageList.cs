namespace OakBot.Enums
{
    // @author gibletto
    internal interface IPagedList<T>
    {
        int Count { get; set; }
        int End { get; }
        bool HasNext { get; }
        bool HasPrev { get; }
        System.Collections.Generic.IEnumerable<T> List { get; set; }
        int NextPage { get; }
        int Page { get; set; }
        int PageCount { get; }
        int PageSize { get; set; }
        int PrevPage { get; }
        int Start { get; }
    }
}