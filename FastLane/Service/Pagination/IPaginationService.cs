using System;

namespace FastLane.Service.Pagination
{
    public interface IPaginationService
    {
        Pagination Calculate(int page, int pageSize, long totalCount);
        int GetNextPage(int currPage, int totalPages);
        int GetPrevPage(int currPage);
    }

    public class Pagination : IPaginationService
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int NextPage { get; set; }
        public int PrevPage { get; set; }

        public Pagination Calculate(int page, int pageSize, long totalCount)
        {
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            int nextPage = GetNextPage(page, totalPages);
            int prevPage = GetPrevPage(page);

            return new Pagination
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                NextPage = nextPage,
                PrevPage = prevPage
            };
        }

        public int GetNextPage(int currPage, int totalPages)
        {
            int nextPage = currPage + 1;
            if (nextPage > totalPages)
            {
                return -1;
            }
            return nextPage;
        }

        public int GetPrevPage(int currPage)
        {
            int prevPage = currPage - 1;
            if (prevPage < 1)
            {
                return -1;
            }
            return prevPage;
        }
    }
}
