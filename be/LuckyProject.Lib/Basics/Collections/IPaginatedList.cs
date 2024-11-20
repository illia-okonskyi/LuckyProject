using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Collections
{
    public interface IPaginatedList<T>
    {
        PaginationMetadata Pagination { get; }
        List<T> Items { get; }
    }
}
