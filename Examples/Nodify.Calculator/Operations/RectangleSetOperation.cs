using System;

namespace Nodify.Calculator
{
    public class RectangleSetOperation : IOperation
    {
        private readonly Func<double, double, RectangleViewModel> _func;
        public RectangleSetOperation(Func<double,double,RectangleViewModel> func) => _func = func;
        
        public object Execute(params object[] operands)
        {
            var param1 = Convert.ToDouble(operands[0]);
            var param2 = Convert.ToDouble(operands[1]); 

            return _func.Invoke(param1, param2);
        }
    }
}