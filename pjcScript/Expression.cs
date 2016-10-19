using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace pjcScript
{
    public interface ExpressionNode
    {
        object Visit(ExecContext ctx, Dictionary<string, object> table);
    }
    public class ExpressionBuilder
    {
        public static ExpressionNode Create(List<string> tokens)
        {
            List<ExpressionNode> expressions = new List<ExpressionNode>();

            //expression 만들어놓고 연산자 따라 합침
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                switch (token)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                    case "%":
                    case "=":
                    case "?":
                        expressions.Add(BinaryOperatorExpression.Create(token));
                        break;
                    case "(":
                        //괄호 닫는 부분까지 하나의 Expression으로 만든다.
                        int parenCount = 1;
                        var parenToken = new List<string>();
                        for (int j = i + 1; j < tokens.Count; j++)
                        {
                            if (tokens[j] == "(")
                            {
                                parenCount++;
                            }
                            else if (tokens[j] == ")")
                            {
                                parenCount--;
                            }

                            if (parenCount == 0)
                            {
                                i = j;
                                expressions.Add(Create(parenToken));
                                break;
                            }

                            parenToken.Add(tokens[j]);
                        }
                        break;
                    case ")":
                        break;
                    default:
                        if (i == tokens.Count - 1 || tokens[i + 1] != "(")
                        {
                            expressions.Add(ReferenceExpression.Create(token)); //변수 상수
                        }
                        else
                        {
                            //함수
                            var paramList = new List<List<string>>();

                            int now = i + 2;
                            int pc = 1;

                            while (now < tokens.Count && pc > 0)
                            {
                                var param = new List<string>();
                                while (now < tokens.Count)
                                {
                                    if (tokens[now] == "(")
                                    {
                                        pc++;
                                    }

                                    if (tokens[now] == ")")
                                    {
                                        if (pc == 1)
                                            break;

                                        pc--;
                                    }

                                    if (tokens[now] == "," && pc == 1)
                                    {
                                        break;
                                    }

                                    param.Add(tokens[now]);
                                    now++;
                                }
                                paramList.Add(param);

                                if (tokens[now] == ")")
                                    break;

                                now++;

                            }

                            expressions.Add(FunctionExpression.Create(token, paramList));
                            i = now;
                        }
                        break;
                }
            }

            //만들어놓은 expressions를 하나로 합친다.
            //우선 중위식을 후위식으로 변환
            var postexp = new List<ExpressionNode>();
            var stack = new Stack<BinaryOperatorExpression>();

            foreach (var e in expressions)
            {
                var op = e as BinaryOperatorExpression;

                if (op != null && !op.HasOperand())
                {
                    while (stack.Count != 0 &&
                        stack.Peek().Priority > op.Priority)
                    {
                        postexp.Add(stack.Pop());
                    }

                    if (stack.Count != 0 &&
                        stack.Peek().Priority == op.Priority)
                    {
                        if (!op.IsLeft)
                        {
                            postexp.Add(stack.Pop());
                        }
                    }

                    stack.Push(op);
                }
                else
                {
                    //피 연산자는 바로 넣기
                    postexp.Add(e);
                }
            }

            while (stack.Count > 0)
            {
                postexp.Add(stack.Pop());
            }

            var expressionStack = new Stack<ExpressionNode>();

            for (int i = 0; i < postexp.Count; i++)
            {
                var e = postexp[i];
                var op = e as BinaryOperatorExpression;

                //operand가 이미 설정되어 있는 경우(괄호) 제외하기
                if (op != null && !op.HasOperand())
                {
                    //연산자면 operand 설정후 다시 푸쉬
                    var right = expressionStack.Pop();
                    var left = expressionStack.Pop();

                    op.SetOperand(left, right);
                    expressionStack.Push(op);
                }
                else
                {
                    expressionStack.Push(e);
                }
            }

            return expressionStack.Peek();
        }
    }

    class ReferenceExpression : ExpressionNode
    {
        public string Name
        {
            get { return var; }
        }

        public static ReferenceExpression Create(string v)
        {
            var node = new Instant.ReferenceExpressionImpl(v);
            return node;
        }

        protected ReferenceExpression(string v)
        {
            var = v;
        }

        public virtual object Visit(ExecContext ctx, Dictionary<string, object> table)
        {
            throw new NotImplementedException();
        }

        protected string var = null;
    }

    class AssignmentExpression : ExpressionNode
    {
        protected string var = null;
        protected ExpressionNode exp = null;

        public static AssignmentExpression Create()
        {
            var node = new Instant.AssignmentExpressionImpl();
            return node;
        }

        public virtual object Visit(ExecContext ctx, Dictionary<string, object> table)
        {
            throw new NotImplementedException();
        }
    }

    class BinaryOperatorExpression : ExpressionNode
    {
        public int Priority
        {
            get { return priority; }
        }

        public bool IsLeft
        {
            get { return isLeft; }
        }

        protected ExpressionNode lhs = null;
        protected ExpressionNode rhs = null;
        protected int priority = 0;
        protected bool isLeft = true;
        protected string s;

        public static BinaryOperatorExpression Create(string s)
        {
            var node = new Instant.BinaryOperatorExpressionImpl(s);
            return node;
        }

        protected BinaryOperatorExpression(string s)
        {
            this.s = s;

            switch (s)
            {
                case "=":
                    priority = 0;
                    isLeft = false;
                    break;
                case "*":
                case "/":
                case "%":
                    priority = 3;
                    isLeft = true;
                    break;
                case "+":
                case "-":
                    priority = 2;
                    isLeft = true;
                    break;
                case "?":
                    priority = 1;
                    isLeft = true;
                    break;
            }
        }

        public bool HasOperand()
        {
            return lhs != null && rhs != null;
        }

        public void SetOperand(ExpressionNode left, ExpressionNode right)
        {
            lhs = left;
            rhs = right;
        }

        public virtual object Visit(ExecContext ctx, Dictionary<string, object> table)
        {
            throw new NotImplementedException();
        }
    }

    class FunctionExpression : ExpressionNode
    {
        public static FunctionExpression Create(string name, List<List<string>> paramList)
        {
            var node = new Instant.FunctionExpressionImpl(name, paramList);
            return node;
        }

        protected FunctionExpression(string name, List<List<string>> paramList)
        {
            func = name;

            param = paramList.Select(p => ExpressionBuilder.Create(p)).ToList();
        }

        protected List<ExpressionNode> param;
        protected string func;

        public virtual object Visit(ExecContext ctx, Dictionary<string, object> table)
        {
            throw new NotImplementedException();
        }
    }
}
