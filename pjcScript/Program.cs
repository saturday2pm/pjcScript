using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pjcScript
{
	class Program
	{
		static void Main(string[] args)
		{
			var interpreter = new Interpreter();
			var bind = new Dictionary<string, object>();
			interpreter.AddBind("a", null);
			interpreter.AddBind("b", -5);
			interpreter.AddBind("abs", new Func<int, int>(Math.Abs));
			interpreter.AddBind("max", new Func<int, int, int>(Math.Max));
			int res = (int)interpreter.Exec("c=a+b;abs(max(a * b, c));");

			Console.WriteLine(res);
		}
	}
}
