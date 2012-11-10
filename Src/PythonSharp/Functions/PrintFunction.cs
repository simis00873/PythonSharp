﻿namespace PythonSharp.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PythonSharp.Language;

    public class PrintFunction : IFunction
    {
        private Machine machine;

        public PrintFunction(Machine machine)
        {
            this.machine = machine;
        }

        public object Apply(IContext context, IList<object> arguments, IDictionary<string, object> namedArguments)
        {
            if (arguments != null)
            {
                int narg = 0;
                foreach (var argument in arguments)
                {
                    if (narg != 0)
                        this.machine.Output.Write(' ');
                    this.machine.Output.Write(argument.ToString());
                    narg++;
                }
            }

            this.machine.Output.WriteLine();

            return null;
        }
    }
}
