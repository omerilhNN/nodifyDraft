using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator
{
    public class RectangleViewModel : ObservableObject
    {
        private double? _width;
        public double? Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }
        private double? _height;
        public double? Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }
        //Return Type
        private double? _area;
        public double? Area
        {
            get => _area;
            set => SetProperty(ref _area, value);
        }

    }
}
