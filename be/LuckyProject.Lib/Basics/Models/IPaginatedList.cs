using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Models
{
    public interface IPaginatedList<T>
    {
        PaginationMetadata Pagination { get; }
        List<T> Items { get; }
    }
}
