using System;

namespace Nodify.Calculator
{
    public class ValueOperation : IOperation
    {
        private readonly Func<double> _func;

        public ValueOperation(Func<double> func) => _func = func;

        public object Execute(params object[] operands)
            => _func();
    }
}
