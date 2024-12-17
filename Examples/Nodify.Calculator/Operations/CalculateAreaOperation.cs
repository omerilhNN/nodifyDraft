using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{

    //Operation Factory'e implement etmen gerekebilir fakat şuanda bu işi BinaryOperation yapıyor çünkü aynı parametre ve dönüş değerlerini almaktadırlar
    public class CalculateAreaOperation:IOperation
    {
        private readonly Func<RectangleViewModel, double> _func;

        public CalculateAreaOperation(Func<RectangleViewModel, double> func)
           => _func = func;
        public object Execute(params object[] operands)
        {
            var param1 = operands[0] as RectangleViewModel;
            if (param1 == null)
            {
                Console.WriteLine("Conversion failed: Operand is not a RectangleViewModel.");
            }



            // Invoke the function with the converted parameter
            return _func.Invoke(param1);

        }
    }
}
