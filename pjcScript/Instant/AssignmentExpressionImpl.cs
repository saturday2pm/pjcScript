using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pjcScript.Instant
{
    class AssignmentExpressionImpl : AssignmentExpression
    {
        public AssignmentExpressionImpl()
            : base()
        {
        }

        public override object Visit(ExecContext ctx, Dictionary<string, object> table)
        {
            var res = exp.Visit(table);

            if (table.ContainsKey(var))
            {
                table[var] = res;
            }
            else
            {
                table.Add(var, res);
            }

            return res;
        }
    }
}
