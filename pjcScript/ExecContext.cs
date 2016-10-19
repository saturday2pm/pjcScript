using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pjcScript
{
    public class ExecContext
    {
        public Dictionary<string, object> table { get; set; }

        public ExecContext()
        {
            table = new Dictionary<string, object>();
        }
    }
}
