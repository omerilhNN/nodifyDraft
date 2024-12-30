using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{

    //Operation Factory'e implement etmen gerekebilir fakat şuanda bu işi BinaryOperation yapıyor çünkü aynı parametre ve dönüş değerlerini almaktadırlar
    public class ChdSetOperation : IOperation
    {
        private readonly Func<List<object>, object> _func;

        public ChdSetOperation(Func<List<object>, object> func)
           => _func = func;
        public object Execute(params object[] operands)
        {

            if (operands == null ||operands.Length == 0)
            {
                Console.WriteLine("Conversion failed: Operand is not a RectangleViewModel.");
                return null;
            }
            var dynamicOperands = new List<object>();
            foreach (var operand in operands)
            {
                dynamicOperands.Add((object)operand);
            }

            return _func.Invoke(dynamicOperands);
        }
    }
}
