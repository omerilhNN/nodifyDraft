using DriverBase;
using Nodify.Calculator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class ChdFieldSetOperationViewModel : OperationViewModel
    {
        public ChdFieldSetOperationViewModel(Type opType) {
            CreateInputsFromChdFields(opType);

        
        }
        protected override void OnButtonClicked()
        {
            if (Input != null) {
                ChdViewModel chd = new ChdViewModel();

                foreach(var input in Input)
                {
                    chd.ChdInputFields.Add(input.Value);
                }
                Output.Value = chd;
            }
        }
        private void CreateInputsFromChdFields(Type opType)
        {
          
                var chdFields = ReflectionTools.GetCHDFields(opType);
            //var sub = ReflectionTools.GetSubClasses<MsgAgent>(); // MsgAgent'tan derive eden tüm subclassları alır -> 566 tane 
                foreach (var chdField in chdFields)
                {
                    Input.Add(new ConnectorViewModel
                    {
                        Title = chdField.Name, // Set the field name as the title
                        // Value can be set later when needed
                     });
                }
            
        }
    }
}
