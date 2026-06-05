using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;

namespace Domain.Exceptions
{
    public sealed class PlaceNotFoundException : NotFoundException
    {
        public PlaceNotFoundException(int id) : base($"place with this id {id} not found")
        {
            
        }
    }
}
