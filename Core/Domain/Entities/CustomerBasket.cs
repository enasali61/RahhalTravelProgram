using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class CustomerBasket
    {  
            // Cart each has id and have items = places or trips
            public string Id { get; set; }
            public IEnumerable<BasketItems> Items { get; set; }

                  
    }
}
