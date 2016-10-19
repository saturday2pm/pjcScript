using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace pjcScript.Instant
{
    class BinaryOperatorExpressionImpl : BinaryOperatorExpression
    {
        public delegate object Op(ExpressionNode lhs, ExpressionNode rhs, ExecContext ctx, Dictionary<string, object> table);
        Op op = null;

        public BinaryOperatorExpressionImpl(string s)
            : base(s)
        {
        }

        Op Nullable()
        {
            return (lhs, rhs, ctx, table) =>
            {
                var l = lhs.Visit(ctx, table);

                if (l == null)
                    return rhs.Visit(ctx, table);
                else
                    return l;
            };
        }

        Op Assign()
        {
            return (lhs, rhs, ctx, table) =>
            {
                var obj = lhs as ReferenceExpressionImpl;
                var res = rhs.Visit(ctx, table);

                table[obj.Name] = res;

                return res;
            };
        }

        Op Binary(string name)
        {
            return (lhs, rhs, ctx, table) =>
            {
                var l = lhs.Visit(ctx, table);
                var r = rhs.Visit(ctx, table);

                if (l is string)
                {
                    switch (name)
                    {
                        case "op_Addition":
                            return (string)l + r.ToString();
                    }
                }

                if (l is int)
                {
                    switch (name)
                    {
                        case "op_Multiply":
                            return (int)l * Convert.ToInt32(r);
                        case "op_Division":
                            return (int)l / Convert.ToInt32(r);
                        case "op_Modulus":
                            return (int)l % Convert.ToInt32(r);
                        case "op_Addition":
                            return (int)l + Convert.ToInt32(r);
                        case "op_Subtraction":
                            return (int)l - Convert.ToInt32(r);
                    }
                }

                if (l is float)
                {
                    switch (name)
                    {
                        case "op_Multiply":
                            return (float)l * Convert.ToSingle(r);
                        case "op_Division":
                            return (float)l / Convert.ToSingle(r);
                        case "op_Modulus":
                            return (float)l % Convert.ToSingle(r);
                        case "op_Addition":
                            return (float)l + Convert.ToSingle(r);
                        case "op_Subtraction":
                            return (float)l - Convert.ToSingle(r);
                    }
                }

                return l.GetType().GetMethod(name, BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { l, r });
            };
        }

        public override object Visit(ExecContext ctx, Dictionary<string, object> table)
        {
            switch (s)
            {
                case "=":
                    op = Assign();
                    break;
                case "*":
                    op = Binary("op_Multiply");
                    break;
                case "/":
                    op = Binary("op_Division");
                    break;
                case "%":
                    op = Binary("op_Modulus");
                    break;
                case "+":
                    op = Binary("op_Addition");
                    break;
                case "-":
                    op = Binary("op_Subtraction");
                    break;
                case "?":
                    op = Nullable();
                    break;
            }

            return op(lhs, rhs, table);
        }
    }
}
