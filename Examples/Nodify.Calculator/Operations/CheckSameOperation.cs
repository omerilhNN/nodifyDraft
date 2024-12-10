using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class CheckSameOperation : IOperation
    {
        private readonly Func<double, double, bool> _func;

        public CheckSameOperation(Func<double, double, bool> func) => _func = func;

        public object Execute(params object[] operands)
        {
            var param1 = Convert.ToDouble(operands[0]);
            var param2 = Convert.ToDouble(operands[1]); 

            return _func.Invoke(param1, param2);
        }

        
    }
}
