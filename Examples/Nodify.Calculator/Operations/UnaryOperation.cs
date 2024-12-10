using System;

namespace Nodify.Calculator
{
    public class UnaryOperation : IOperation
    {
        private readonly Func<double, double> _func;

        public UnaryOperation(Func<double, double> func) => _func = func;

        public object Execute(params object[] operands)
        {
            var param1 = Convert.ToDouble(operands[0]); 

            return _func.Invoke(param1);
        }
    }
}
