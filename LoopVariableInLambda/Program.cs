using System;
using System.Collections.Generic;
using System.Linq;

namespace LoopVariableInLambda
{
    class Program
    {
        static void Main(string[] args)
        {
            // We want to try, test, understand one peculiar behaviour of C#.
            // That is: if you have a loop and its iteration variable, and if you use that variable in
            // a lambda inside the loop body.  Then the variable is being "captured" in the closure of 
            // of the lambda. Right?  The question is, does the variable or the value get captured?
            // That question isn't apparent until you try this: store that lambda somewhere and use it
            // _outside_ the loop.  That results in the loop variable being used indirectly outside
            // the loop.  Then what would be the value or values used?

            // This oddity has now aged, and is almost obsolete.  It's been "fixed" for .Net newer than 4.
            // But still an interesting lesson in understanding lambdas and closures.
            // Crux is: lambda always captures the variable, not its value.
            // In earlier .Net frameworks, there was singular loop variable for all iterations of the loop.
            // In later .Net frameworks, there is fresh loop variable for every iteration.

            // Example taken from Eric Lippert's blog post
            // https://blogs.msdn.microsoft.com/ericlippert/2009/11/12/closing-over-the-loop-variable-considered-harmful/
            // https://blogs.msdn.microsoft.com/ericlippert/2009/11/16/closing-over-the-loop-variable-part-two/
            
            // With the oddity, it should print 120, 120, 120.
            // That should happen in .Net frameworks 4 and earlier.
            // But in the year 2016, with VS 2012, even targeting .Net 3.5, it prints 100, 110, 120.
            
            var values = new List<int>() { 100, 110, 120 };
            var funcs = new List<Func<int>>();
            foreach (var v in values)
            {
                funcs.Add(() => v);
            }
            foreach (var f in funcs)
            {
                Console.WriteLine(f());
            }
            Console.WriteLine();

            // My own variation of the above example
            // Even this prints 100, 110, 120.

            var actions = new List<Action>();
            foreach (var val in values)
            {
                actions.Add(() => Console.WriteLine(val));
            }
            foreach (var action in actions)
            {
                action();
            }
            Console.WriteLine();

            // Example taken from StackOverflow 3168375
            // http://stackoverflow.com/questions/3168375/using-the-iterator-variable-of-foreach-loop-in-a-lambda-expression-why-fails

            // With the oddity, it's supposed to print Hi Int32, Hi Int32, Hi Int32.
            // But in 2016 with VS 2012 targeting .Net 3.5, it prints Hi String, Hi Single, Hi Int32.

            Type[] types = new Type[] { typeof(string), typeof(float), typeof(int) };
            List<PrintHelloType> helloMethods = new List<PrintHelloType>();

            foreach (var type in types)
            {
                var sayHello = new PrintHelloType(greeting => SayGreetingToType(type, greeting));
                helloMethods.Add(sayHello);
            }

            foreach (var helloMethod in helloMethods)
            {
                Console.WriteLine(helloMethod("Hi"));
            }
            Console.WriteLine();

            // Example taken from Brian's blog post
            // https://lorgonblog.wordpress.com/2008/11/12/on-lambdas-capture-and-mutability/

            // This one still shows the oddity.  It prints 10, 10, 10, 10, 10.
            // Even with .Net 4.5, 4.6, it prints all 10s.

            List<Func<int>> functions = new List<Func<int>>();
            for (int i = 0; i < 5; ++i)
            {
                functions.Add(() => i * 2);
            }
            foreach (var function in functions)
            {
                Console.WriteLine(function());
            }
            Console.WriteLine();

            // ForEach version of Brian's example
            // This does not show the oddity even with .Net 3.5, and prints 0, 2, 4, 6, 8.

            List<Func<int>> functions2 = new List<Func<int>>();
            foreach (int i in Enumerable.Range(0, 5))
            {
                functions2.Add(() => i * 2);
            }
            foreach (var function in functions2)
            {
                Console.WriteLine(function());
            }
            Console.WriteLine();
        }

        public delegate string PrintHelloType(string greeting);

        public static string SayGreetingToType(Type type, string greetingText)
        {
            return greetingText + " " + type.Name;
        }
    }
}
