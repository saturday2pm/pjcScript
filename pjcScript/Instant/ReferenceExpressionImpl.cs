using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pjcScript.Instant
{
    class ReferenceExpressionImpl : ReferenceExpression 
    {
        public ReferenceExpressionImpl(string v)
            : base(v)
        {
        }

        public override object Visit(Dictionary<string, object> table)
        {
            int intRes;
            float floatRes;

            if (var.StartsWith("\"") && var.EndsWith("\""))
                return var.Substring(1, var.Length - 2);

            if (int.TryParse(var, out intRes))
            {
                return intRes;
            }

            if (float.TryParse(var, out floatRes))
            {
                return floatRes;
            }

            if (table.ContainsKey(var))
            {
                return table[var];
            }

            return null;
        }
    }
}
