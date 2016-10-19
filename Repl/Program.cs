using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pjcScript;

namespace Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var interpreter = new Interpreter();
            var bind = new Dictionary<string, object>();

            while (true)
            {
                Console.Write(">> ");
                var obj = interpreter.Exec(Console.ReadLine());

                Console.Write("= ");
                Console.WriteLine(obj);
            }
        }
    }
}
