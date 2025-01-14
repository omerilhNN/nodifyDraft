using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Nodify.Calculator
{
    public class ConnectorViewModel : ObservableObject
    {
        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private object? _value;
        public object? Value
        {
            get => _value;
            set
            {
#region Typecasting denemeler
                //if (value != null && ValueType != null)
                //{
                //    try
                //    {
                //        _value = Convert.ChangeType(value, ValueType); // Dynamic casting
                //    }
                //    catch (Exception ex)
                //    {
                //        Console.WriteLine($"Failed to cast value to {ValueType}: {ex.Message}");
                //        _value = null; // Reset value if cast fails
                //    }
                //}
                //else
                //{
                //    _value = value; // Assign directly if ValueType is not set
                //}

                //SetProperty(ref _value, value)
                //    .Then(() => ValueObservers.ForEach(o => Convert.ChangeType(_value,ValueType)));
#endregion
                if(value != null && ValueType != null)
                {
                    try
                    {
                    var castedValue = Convert.ChangeType(value, ValueType);
                    SetProperty(ref _value, castedValue)
                        .Then(() => ValueObservers.ForEach(o => o.Value = castedValue));
                    ValidationMessage = null;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"Invalid cast to {ValueType} : {ex.Message}");
                        ValidationMessage = $"{ex.Message} Requires {ValueType.Name} type.";
                        SetProperty(ref _value, null);
                    }
                }
                else
                {
                    SetProperty(ref _value, value)
                        .Then(() => ValueObservers.ForEach(o=> o.Value = value));
                }
            }
        }
        private string? _validationMessage;
        public string? ValidationMessage
        {
            get => _validationMessage;
            set
            {
                SetProperty(ref _validationMessage, value);
            }
        }
        private Type? _valueType;
        public Type? ValueType
        {
            get => _valueType;
            set => SetProperty(ref _valueType, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isInput;
        public bool IsInput
        {
            get => _isInput;
            set => SetProperty(ref _isInput, value);
        }

        private Point _anchor;
        public Point Anchor
        {
            get => _anchor;
            set => SetProperty(ref _anchor, value);
        }

        private OperationViewModel _operation = default!;
        public OperationViewModel Operation
        {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }

        public List<ConnectorViewModel> ValueObservers { get; } = new List<ConnectorViewModel>();
    }
}
