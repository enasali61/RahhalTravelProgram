using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum PlacesSortOption
    {
        NameEnglishAsc,
        NameEnglishDesc,
        RatingAsc,
        RatingDesc,
        NameArabicAsc,
        NameArabicDesc
    }
    public class PlacesSpecParams
    {
        public int? CategoryId { get; set; }
        public PlacesSortOption? Sort { get; set; }

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
