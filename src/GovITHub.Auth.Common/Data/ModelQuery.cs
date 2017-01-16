using System.Collections.Generic;

namespace GovITHub.Auth.Common.Data
{
    public class ModelQuery<T>
    {
        public IEnumerable<T> List { get; set; }
        public int TotalItems { get; set; }
    }
}
