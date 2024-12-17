using StringMath;
using System;
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
                Title = "Rectangle Input:",
            });
            Output = new ConnectorViewModel();
        }
        protected override void OnButtonClicked()
        {
            base.OnButtonClicked();
            if (Input.Count > 0 && Input[0]?.Value != null)
            {
                try
                {
                    // Explicit type check and cast
                    if (Input[0].Value is RectangleViewModel rectangleInput)
                    {
                        // Safely calculate the area
                        if (rectangleInput.Width.HasValue && rectangleInput.Height.HasValue)
                        {
                            double w = Convert.ToDouble(rectangleInput.Width);
                            double h = Convert.ToDouble(rectangleInput.Height);
                            double area = w * h;

                            // Update the rectangle's Area property
                            rectangleInput.Area = area;

                            // Assign the area to Output
                            if (Output == null)
                                Output = new ConnectorViewModel();

                            Output.Value = area;

                            // Log results for debugging
                            Console.WriteLine($"Rectangle Area Calculated: {area}");
                        }
                        else
                        {
                            Console.WriteLine("Width or Height is null.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Input[0].Value is not of type RectangleViewModel.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while calculating the area: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Input is not properly initialized or Input[0].Value is null.");
            }


        }
        protected override void OnInputValueChanged()
        {
            base.OnInputValueChanged();

        }
    }
}
