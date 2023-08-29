using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PageList<T>: List<T>
    {
        public PageList(IQueryable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPage = (int)Math.Ceiling(count/(double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int PageSize)
        {
            var count = await source.CountAsync();
            var items =  source.Skip((pageNumber - 1) * PageSize).Take(PageSize);

            return new PageList<T>(items, count,pageNumber,PageSize);
        }
        
    }
}