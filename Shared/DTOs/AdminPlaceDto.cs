using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class AdminPlaceDto
    {
            public int Id { get; set; }
        public string Name { get; set; }
            public string CategoryName { get; set; }  
            public string City { get; set; }          
            public string ImageUrl { get; set; }
            public decimal PriceEgAdult { get; set; }
            public decimal PriceForeignAdult { get; set; }
            public decimal PriceEgStudent { get; set; }
            public decimal PriceForeignStudent { get; set; }
        
    }
}
