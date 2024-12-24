using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class RectangleSetOperationViewModel : OperationViewModel
    {
        public RectangleSetOperationViewModel() {
            Input.Add(new ConnectorViewModel
            {
                Title = "Width"
            });
            Input.Add(new ConnectorViewModel
            {
                Title = "Height"
            });
        }

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
           
                try
                {
                    var width = Convert.ToDouble(Input[0].Value);
                    var height = Convert.ToDouble(Input[1].Value);

                        RectangleViewModel rec = new RectangleViewModel { 
                            Area = width * height,
                            Width = width,
                            Height = height
                        };
                        Output.Value = rec;
                }
                catch
                {

                }
        }
        protected override void OnInputValueChanged()
        {
            base.OnInputValueChanged();
        }
        }
}
