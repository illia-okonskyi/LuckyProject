using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Models
{
    public class PaginatedList<T> : IPaginatedList<T> 
    {
        public PaginationMetadata Pagination { get; init; }
        public List<T> Items { get; init; }
    }
}
