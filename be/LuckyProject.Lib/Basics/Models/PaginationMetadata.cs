using System;

namespace LuckyProject.Lib.Basics.Models
{
    public class PaginationMetadata
    {
        public int TotalItemsCount { get; }
        public int PageSize { get; }
        public int Page { get; }
        public bool IsCyclic { get; }
        public int TotalPagesCount { get; }
        public int? NextPage { get; }
        public int? PrevPage { get; }

        public PaginationMetadata(
            int totalItemsCount,
            int pageSize,
            int page,
            bool isCyclic)
        {
            if (pageSize < 1)
            {
                throw new ArgumentException(nameof(pageSize));
            }

            if (page < 1)
            {
                throw new ArgumentException(nameof(page));
            }

            TotalItemsCount = totalItemsCount;
            PageSize = pageSize;
            Page = page;
            TotalPagesCount = (int)Math.Ceiling((double)TotalItemsCount / PageSize);

            int? nextPage = page + 1;
            if (nextPage > TotalPagesCount)
            {
                nextPage = isCyclic ? 1 : null;
            }
            NextPage = nextPage;

            int? prevPage = page - 1;
            if (prevPage == 0)
            {
                prevPage = isCyclic ? TotalPagesCount : null;
            }
            PrevPage = prevPage;
        }
    }
}
