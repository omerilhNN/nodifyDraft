using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Nodify.Calculator
{
    public class OperationViewModel : ObservableObject
    {
        public OperationViewModel()
        {
            Input.WhenAdded(x =>
            {
                x.Operation = this;
                x.IsInput = true;
                //x.PropertyChanged += OnInputValueChanged;
            })
            .WhenRemoved(x =>
            {
                //x.PropertyChanged -= OnInputValueChanged;
            });
        }
        //TODO: PREVIOUS NODE TUTMAK İÇİN BU GELİŞTİRİLEBİLİR
        //private OperationViewModel? _previous;
        //public OperationViewModel? Previous
        //{
        //    get => _previous;
        //    set => SetProperty(ref _previous, value);
        //}

        private void OnInputValueChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                OnInputValueChanged();
            }
        }

        private Point _location;
        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        private Size _size;
        public Size Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool? _isSuccess;
        public bool? IsSuccess
        {
            get => _isSuccess;
            set=> SetProperty(ref _isSuccess, value);    
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public bool IsReadOnly { get; set; }

        private IOperation? _operation;
        public IOperation? Operation
        {
            get => _operation;
            set => SetProperty(ref _operation, value)
                   .Then(OnInputValueChanged);
            
        }
      

        public NodifyObservableCollection<ConnectorViewModel> Input { get; } = new NodifyObservableCollection<ConnectorViewModel>();

        private ConnectorViewModel? _output;
        public ConnectorViewModel? Output
        {
            get => _output;
            set
            {
                if (SetProperty(ref _output, value) && _output != null)
                {
                    _output.Operation = this;
                }
            }
        }
        public void ExecuteOperation()
        {
            OnButtonClicked(); //Mevcut operasyon çalıştırılır
            TriggerDependentOperations(); // Mevcut operasyona bağlı operasyonlar asenkron olarak tetiklenir.
        }

        protected virtual void OnButtonClicked()
        {
            if (Output != null && Operation != null)
            {
                try
                {
                    var input = Input.Select(i => i.Value).ToArray();
                    Output.Value = Operation?.Execute(input) ?? 0;
                }
                catch
                {

                }
            }
        }
        protected virtual void OnInputValueChanged()
        {
            if (Output != null && Operation != null)
            {
                try
                {
                    var input = Input.Select(i => i.Value).ToArray();
                    Output.Value = Operation?.Execute(input) ?? 0;
                }
                catch
                {

                }
            }
        }
        protected void TriggerDependentOperations()
        {
            if (Output != null)
            {
                var dependentOperations = CalculatorViewModel.Instance.Connections
                    .Where(c => c.Output == Output && c.Input.Operation != null)
                    .Select(c => c.Input.Operation)
                    .OfType<OperationViewModel>()
                    .ToList();

                foreach (var dependentOperation in dependentOperations)
                {
                    dependentOperation.ExecuteOperation();
                }
            }
        }
    }
}
