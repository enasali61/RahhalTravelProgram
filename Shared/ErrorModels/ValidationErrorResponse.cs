using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.ErrorModels
{
    public class ValidationErrorResponse
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }
        public IEnumerable<ValidationErrors>Errors { get; set; }
    }
    public class ValidationErrors
    {
        public string Field { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
