﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pjcScript.Instant
{
    class FunctionExpressionImpl : FunctionExpression
    {
        public FunctionExpressionImpl(string name, List<List<string>> paramList)
            : base(name, paramList)
        {
        }

        public override object Visit(Dictionary<string, object> table)
        {
            var p = param.Select(e => e.Visit(table)).ToArray();

            return table[func].GetType().GetMethod("Invoke").Invoke(table[func], p);
        }
    }
}
