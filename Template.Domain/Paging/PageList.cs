
using Microsoft.EntityFrameworkCore;

///
namespace Template.Domain.Paging
{
    public class PageList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public PageList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
        public static async Task<List<T>> ToModelList(IQueryable<T> source, PageParam pageParam)
        {
            int pageNumber = pageParam.Page;
            int pageSize = pageParam.PageSize;

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return items;
        }
        public static PageList<T> ToPagedList(List<T> source, PageParam pageParam)
        {
            var count = source.Count();
            int pageNumber = pageParam.Page;
            int pageSize = pageParam.PageSize;

            return new PageList<T>(source, count, pageNumber, pageSize);
        }
    }
}
