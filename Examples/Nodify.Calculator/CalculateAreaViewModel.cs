using StringMath;
using System.Collections.Generic;
using System.Linq;

namespace Nodify.Calculator
{
    public class CalculateAreaViewModel : OperationViewModel
    {
        public CalculateAreaViewModel()
        {
            Input.Add(new ConnectorViewModel
            {
                Title = "Rectangle Input:"
            });
        }
        public RectangleViewModel InputRectangle { get; set; } = new RectangleViewModel(); 
        public RectangleViewModel OutputRectangle { get; private set; }

        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            if (InputRectangle != null)
            {
                try
                {
                OutputRectangle = new RectangleViewModel { 
                    Width = InputRectangle.Width,
                    Height = InputRectangle.Height,
                    Area = InputRectangle.Width * InputRectangle.Height
                };

                }
                catch
                {
                    InputRectangle = OutputRectangle;
                }
                    
            }


        }

    }
}
