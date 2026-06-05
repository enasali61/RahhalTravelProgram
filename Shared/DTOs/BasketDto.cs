using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public record BasketDto
    {
        public string Id { get; init; }
        public IEnumerable<BasketItemsDto> Items { get; init; }
    }
}
