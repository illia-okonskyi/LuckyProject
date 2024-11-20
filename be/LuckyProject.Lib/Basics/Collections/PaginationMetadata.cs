using System;

namespace LuckyProject.Lib.Basics.Collections
{
    public class PaginationMetadata
    {
        public int TotalItemsCount { get; }
        public int PageSize { get; }
        public int Page { get; }
        public bool IsCyclic { get; }
        public int TotalPagesCount { get; }
        public int FirstPageItem { get; }
        public int LastPageItem { get; }
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
            TotalPagesCount = TotalItemsCount > 0
                ? (int)Math.Ceiling((double)TotalItemsCount / PageSize)
                : 1;
            Page = Math.Min(page, TotalPagesCount);
            FirstPageItem = Math.Max((Page - 1) * PageSize, 1);
            LastPageItem = Math.Min(Page * PageSize, TotalItemsCount);

            int? nextPage = Page + 1;
            if (nextPage > TotalPagesCount)
            {
                nextPage = isCyclic ? 1 : null;
            }
            NextPage = nextPage;

            int? prevPage = Page - 1;
            if (prevPage == 0)
            {
                prevPage = isCyclic ? TotalPagesCount : null;
            }
            PrevPage = prevPage;
        }
    }
}
