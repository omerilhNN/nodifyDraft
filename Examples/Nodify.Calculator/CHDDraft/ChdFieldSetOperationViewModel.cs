using DriverBase;
using Microsoft.VisualBasic.FileIO;
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
                    if(input != null && input.ValueType != null && input.Value != null)
                    {
                        try
                        {
                            chd.ChdInputFields.Add(input.Value);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to cast value '{input.Value}' of {input.Title} to {input.ValueType}: {ex.Message}");
                            chd.ChdInputFields.Add(null); // Add the raw value if casting fails
                        }
                    }
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

                #region Generic yapma fikrinden vazgeçtim yorum satırları
                //var connectorType = typeof(ConnectorViewModel<>).MakeGenericType(fieldType);
                //var connectorInstance = Activator.CreateInstance(connectorType);

                //var titleProperty = connectorType.GetProperty("Title");
                //titleProperty?.SetValue(connectorInstance, chdField.Name);

                //var addMethod = typeof(NodifyObservableCollection<>)
                //    .MakeGenericType(connectorType)
                //    .GetMethod("Add");

                //addMethod.Invoke(Input, new[] { connectorInstance});
                #endregion

                Input.Add(new ConnectorViewModel
                {
                    ValueType = chdField.FieldType,
                    Title = chdField.Name // Set the field name as the title
                });
            }
            
        }
    }
}
