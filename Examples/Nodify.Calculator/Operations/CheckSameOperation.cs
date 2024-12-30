using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class CheckSameOperation<T> : IOperation
    {
        private readonly Func<T, T, bool> _func;

        public CheckSameOperation(Func<T, T, bool> func) => _func = func;

        public object Execute(params object[] operands)
        {
            if (operands[0] is T param1 && operands[1] is T param2)
            {
                return _func.Invoke(param1, param2);
            }

            throw new InvalidCastException($"Operands must be of type {typeof(T).Name}.");
        }


    }
}
