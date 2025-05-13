namespace Template.Domain.Paging
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PagingResponseHeadersAttribute : Attribute
    {
    }

    public class PageOutput
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public int RecordCount { get; set; }
    }

    public class PageParam
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }

    public class PagingData
    {
        public static PageOutput Paging<T>(PageParam pageParam, ref IQueryable<T> queryable)
        {
            if (pageParam.Page != null && pageParam.PageSize != null)
            {
                var totalNumberOfRecords = queryable.Count();
                var mod = totalNumberOfRecords % pageParam.PageSize;
                var totalPageCount = (totalNumberOfRecords / pageParam.PageSize) + (mod == 0 ? 0 : 1);

                var skipAmount = pageParam.PageSize * (pageParam.Page - 1);

                var output = new PageOutput();
                output.Page = pageParam.Page ?? 0;
                output.PageSize = pageParam.PageSize ?? 0;
                output.PageCount = totalPageCount ?? 0;
                output.RecordCount = totalNumberOfRecords;

                queryable = queryable.Skip(skipAmount ?? 0).Take(pageParam.PageSize ?? 0);

                return output;
            }
            else
            {
                return null;
            }
        }

    }
}
