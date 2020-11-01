using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonoVehicle
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; } // Current page
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;

            // Calculate total number of pages
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Adds items to the end of the List<T>
            this.AddRange(items);
        }

        // The properties HasPreviousPage and HasNextPage can be used to enable or disable Previous and Next paging buttons.
        public bool HasPreviousPage
        {
            // set if current page is greater than 1
            get { return (PageIndex > 1);  }
        }

        public bool HasNextPage
        {
            // set if
            get { return (PageIndex < TotalPages); }
        }

        // The CreateAsync method in this code takes page size and page number and applies the appropriate Skip and Take statements to the IQueryable.
        // A CreateAsync method is used instead of a constructor to create the PaginatedList<T> object because constructors can't run asynchronous code.
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();

            // .Skip() - skips specified number of elements in a sequence
            // .Take() - takes specified number of elements
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
