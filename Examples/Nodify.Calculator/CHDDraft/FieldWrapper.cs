using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.CHDDraft
{
    public class FieldWrapper
    {
        public string? FieldName { get; set; }
        public Type FieldType { get; set; }
        public object? Value { get; set; }  
    }
}
