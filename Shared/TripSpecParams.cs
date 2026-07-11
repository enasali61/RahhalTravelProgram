using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum TripSortOption
    {
        NameAsc,
        NameDesc,
        StartDateAsc,
        StartDateDesc,
        BudgetAsc,
        BudgetDesc,
        StatusAsc,
        StatusDesc
    }

    public class TripSpecParams
    {
        public int? UserId { get; set; }
        public TripSortOption? Sort { get; set; }
        public bool? IsTemplate { get; set; }    // for explore (ready trips)

        private const int MaxPageSize = 10;

        private const int DefaultPageSize = 6;
        public int PageIndex { get; set; } = 1;

        private int pageSize = DefaultPageSize;
        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string? Search { get; set; }
    }
}
