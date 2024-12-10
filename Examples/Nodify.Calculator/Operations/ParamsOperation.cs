using System;
using System.Linq;

namespace Nodify.Calculator
{
    public class ParamsOperation : IOperation
    {
        private readonly Func<double[], double> _func;

        public ParamsOperation(Func<double[], double> func) => _func = func;

        public object Execute(params object[] operands)
        {
            double[] parameters = operands.Select(o => Convert.ToDouble(o)).ToArray();

            return _func.Invoke(parameters);
        }
    }
}
