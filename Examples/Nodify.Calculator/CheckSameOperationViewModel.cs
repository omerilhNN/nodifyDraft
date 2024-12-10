﻿using System.Linq;

namespace Nodify.Calculator
{
    public class CheckSameOperationViewModel : OperationViewModel
    {
        protected override void OnInputValueChanged()
        {
            base.OnInputValueChanged();

            // CheckSame kontrolü
            if (Input.Count > 1)
            {
                try
                {
                    var firstValue = Input.First().Value;
                    IsSuccess = Input.All(i => Equals(i.Value, firstValue)); // Tüm input değerleri aynı mı?

                    if (Output != null)
                    {
                        // Output.Value'ı IsSuccess'e eşitle
                        Output.Value = IsSuccess;
                    }
                }
                catch
                {
                    IsSuccess = false;
                    if (Output != null)
                    {
                        Output.Value = IsSuccess;
                    }
                }
            }
        }
    }
}
