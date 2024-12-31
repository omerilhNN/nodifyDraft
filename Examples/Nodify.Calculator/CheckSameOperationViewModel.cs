using Nodify.Calculator.Helpers;
using System;
using System.Linq;

namespace Nodify.Calculator
{
    public class CheckSameOperationViewModel : OperationViewModel
    {
        public CheckSameOperationViewModel()
        {
            Input.Add(new ConnectorViewModel { 
                Title = "a"
            });
            Input.Add(new ConnectorViewModel
            {
                Title = "b"
            });


        }
        protected override void OnButtonClicked()
        {
            //base.OnButtonClicked();
            if (Input.Count == 2) // Ensure exactly two inputs for comparison
            {
                try
                {
                    var firstValue = Input[0].Value;
                    var secondValue = Input[1].Value;

                    // Null Check
                    if (firstValue == null || secondValue == null)
                    {
                        Console.WriteLine("One or both input values are null.");
                        IsSuccess = false;
                    }
                    else if (firstValue.GetType() == secondValue.GetType())
                    {
                        // Handle same types
                        if (firstValue is ChdViewModel chd1 && secondValue is ChdViewModel chd2)
                        {
                            // Compare ChdViewModel properties
                            IsSuccess = CompareChdViewModels(chd1, chd2);
                        }
                        else if (firstValue.GetType().IsPrimitive || firstValue is string || firstValue is decimal)
                        {
                            // Compare primitive or simple types
                            IsSuccess = Equals(firstValue, secondValue);
                        }
                        else
                        {
                            // Use reflection to compare complex types
                            IsSuccess = ReflectionTools.PublicInstancePropertiesEqual(firstValue, secondValue);
                        }
                    }
                    else
                    {
                        // Handle different types
                        Console.WriteLine($"Type mismatch: {firstValue.GetType()} != {secondValue.GetType()}");
                        IsSuccess = false;
                    }

                    // Assign the result to the output
                    if (Output != null)
                    {
                        Output.Value = IsSuccess;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during comparison: {ex.Message}");
                    IsSuccess = false;
                    if (Output != null)
                    {
                        Output.Value = IsSuccess;
                    }
                }
            }
            else
            {
                Console.WriteLine("CheckSame operation requires exactly two inputs.");
                IsSuccess = false;
                if (Output != null)
                {
                    Output.Value = IsSuccess;
                }
            }

        }
        protected override void OnInputValueChanged()
        {
            base.OnInputValueChanged();

            // CheckSame kontrolü
            if (Input.Count > 1)
            {
                try
                {
                    //var firstValue = Input.First().Value;
                    //IsSuccess = Input.All(i => Equals(i.Value, firstValue)); // Tüm input değerleri aynı mı?

                    //if (Output != null)
                    //{
                    //    // Output.Value'ı IsSuccess'e eşitle
                    //    Output.Value = IsSuccess;
                    //}
                }
                catch
                {
                    //IsSuccess = false;
                    //if (Output != null)
                    //{
                    //    Output.Value = IsSuccess;
                    //}
                }
            }
        }
        private bool CompareChdViewModels(ChdViewModel obj1, ChdViewModel obj2)
        {
            // Compare the number of input fields
            if (obj1.ChdInputFields.Count != obj2.ChdInputFields.Count)
            {
                Console.WriteLine("ChdInputFields count mismatch.");
                return false;
            }

            // Compare each input field value dynamically
            for (int i = 0; i < obj1.ChdInputFields.Count; i++)
            {
                var field1 = obj1.ChdInputFields[i];
                var field2 = obj2.ChdInputFields[i];

                if (field1 == null && field2 == null) continue; // Both null, treat as equal
                if (field1 == null || field2 == null) return false; // One is null, the other is not

                // Handle different types
                if (field1.GetType() != field2.GetType())
                {
                    Console.WriteLine($"Field type mismatch at index {i}: {field1.GetType()} != {field2.GetType()}");
                    return false;
                }

                if (field1 is ChdViewModel nestedChd1 && field2 is ChdViewModel nestedChd2)
                {
                    // Recursively compare nested ChdViewModels
                    if (!CompareChdViewModels(nestedChd1, nestedChd2))
                    {
                        return false;
                    }
                }
                else if (!Equals(field1, field2))
                {
                    Console.WriteLine($"Field value mismatch at index {i}: {field1} != {field2}");
                    return false;
                }
            }

            // Compare the ChdOutput
            if (!Equals(obj1.ChdOutput, obj2.ChdOutput))
            {
                Console.WriteLine($"ChdOutput mismatch: {obj1.ChdOutput} != {obj2.ChdOutput}");
                return false;
            }

            return true; // All fields match
        }

    }
}
