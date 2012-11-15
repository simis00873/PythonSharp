﻿namespace PythonSharp.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using PythonSharp.Commands;
    using PythonSharp.Exceptions;

    public class DefinedFunction : DynamicObject, IFunction, IValues
    {
        private string name;
        private IList<Parameter> parameters;
        private int nminparameters;
        private int nmaxparameters;
        private int nparameters;
        private bool hasdefault;
        private bool haslist;
        private ICommand body;

        public DefinedFunction(string name, IList<Parameter> parameters, ICommand body)
            : base(null)
        {
            this.name = name;
            this.parameters = parameters;
            this.body = body;

            if (parameters != null)
            {
                this.nparameters = parameters.Count;
                this.nmaxparameters = parameters.Count;
                foreach (var parameter in parameters)
                {
                    if (parameter.DefaultValue != null)
                        this.hasdefault = true;
                    
                    if (parameter.IsList)
                    {
                        this.haslist = true;
                        this.nmaxparameters = Int32.MaxValue;
                    }

                    if (!this.hasdefault && !this.haslist)
                        this.nminparameters++;
                }
            }
        }

        public string Name { get { return this.name; } }

        public ICollection<Parameter> Parameters { get { return this.parameters; } }

        public ICommand Body { get { return this.body; } }

        public object Apply(IContext ctx, IList<object> arguments, IDictionary<string, object> namedArguments)
        {
            BindingEnvironment context = new BindingEnvironment(ctx);

            int nargs = 0;

            if (arguments != null)
                nargs = arguments.Count;

            if (nargs < this.nminparameters || nargs > this.nmaxparameters)
                throw new TypeError(string.Format("{0}() takes {4} {1} positional argument{2} ({3} given)", this.name, this.nminparameters, this.nminparameters == 1 ? string.Empty : "s", nargs, this.hasdefault ? "at least" : "exactly"));

            if (namedArguments != null)
                foreach (var namarg in namedArguments)
                    context.SetValue(namarg.Key, namarg.Value);

            if (this.parameters != null)
            {
                int k;

                for (k = 0; k < this.parameters.Count; k++)
                    if (arguments != null && arguments.Count > k)
                    {
                        if (namedArguments != null && namedArguments.ContainsKey(this.parameters[k].Name))
                            throw new TypeError(string.Format("{0}() got multiple values for keyword argument '{1}'", this.name, this.parameters[k].Name));
                        if (this.parameters[k].IsList)
                            context.SetValue(this.parameters[k].Name, GetSublist(arguments, k));
                        else
                            context.SetValue(this.parameters[k].Name, arguments[k]);
                    }
                    else if (this.parameters[k].IsList)
                    {
                        if (this.parameters[k].DefaultValue == null)
                            context.SetValue(this.parameters[k].Name, new List<object>());
                        else
                            context.SetValue(this.parameters[k].Name, this.parameters[k].DefaultValue);

                        break;
                    }
                    else if (namedArguments == null || !namedArguments.ContainsKey(this.parameters[k].Name))
                        context.SetValue(this.parameters[k].Name, this.parameters[k].DefaultValue);
            }

            this.body.Execute(context);

            if (context.HasReturnValue())
                return context.GetReturnValue();

            return null;
        }

        private static IList<object> GetSublist(IList<object> list, int from)
        {
            return list.Skip(from).ToList();
        }

        public override string ToString()
        {
            // TODO add id?
            return string.Format("<function {0}>", this.name);
        }
    }
}
