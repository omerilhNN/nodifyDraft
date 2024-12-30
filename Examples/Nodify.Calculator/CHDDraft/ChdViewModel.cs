using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class ChdViewModel : ObservableObject
    {
        private List<object?> _chdInputFields = new List<object?> ();
        public List<object?> ChdInputFields
        {
            get => _chdInputFields;
            set => SetProperty(ref _chdInputFields,value);
        }

        private object? _chdOutput;
        public object? ChdOutput
        {
            get => _chdOutput;
            set => SetProperty(ref _chdOutput, value);
        }
    }
}
