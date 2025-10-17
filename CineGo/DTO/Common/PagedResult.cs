using System;
using System.Collections.Generic;

namespace CineGo.DTO.Common
{
    /// Dùng để chứa dữ liệu phân trang (paging result)
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
