using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{

    //Operation Factory'e implement etmen gerekebilir fakat şuanda bu işi BinaryOperation yapıyor çünkü aynı parametre ve dönüş değerlerini almaktadırlar
    public class CalculateAreaOperation
    {
        private readonly Func<double, double, double> _func;

        public CalculateAreaOperation(Func<double, double, double> func)
           => _func = func;
        public object Execute(params object[] operands)
        {
            var param1 = Convert.ToDouble(operands[0]);
            var param2 = Convert.ToDouble(operands[1]); 

            return _func.Invoke(param1, param2);    
        }
        
    }
}
