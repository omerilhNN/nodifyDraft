using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Nodify.Calculator
{
    public class EditorViewModel : ObservableObject
    {
        public event Action<EditorViewModel, CalculatorViewModel>? OnOpenInnerCalculator;
       
        //private readonly ObservableCollection<OperationInfoViewModel> _droppedOperations;

        //public IReadOnlyList<OperationInfoViewModel> DroppedOperations => _droppedOperations;
        public EditorViewModel? Parent { get; set; }

        public EditorViewModel()
        {
            //_droppedOperations = new ObservableCollection<OperationInfoViewModel>();    

            Calculator = new CalculatorViewModel();
            OpenCalculatorCommand = new DelegateCommand<CalculatorViewModel>(calculator =>
            {
                OnOpenInnerCalculator?.Invoke(this, calculator);
            });
        }
        //public void AddDroppedOperation(OperationInfoViewModel operation)
        //{
        //        _droppedOperations.Add(operation);
        //}
        public INodifyCommand OpenCalculatorCommand { get; }
        //public INodifyCommand ExecuteAllOperationsCommand => new DelegateCommand(ExecuteAllOperationsHierarchial);
        public Guid Id { get; } = Guid.NewGuid();

        //private void ExecuteAllOperationsHierarchial()
        //{
        //    // Perform topological sort to determine execution order
        //    var sortedOperations = TopologicalSort(DroppedOperations);

        //    foreach (var operation in sortedOperations)
        //    {
        //        // Execute the operation via OnButtonClicked
        //        operation.OnButtonClicked();
        //    }
        //}
        //private IEnumerable<OperationViewModel> TopologicalSort(IEnumerable<OperationViewModel> operations)
        //{
        //    var sorted = new List<OperationViewModel>();
        //    var visited = new HashSet<OperationViewModel>();

        //    void Visit(OperationViewModel operation)
        //    {
        //        if (!visited.Contains(operation))
        //        {
        //            visited.Add(operation);

        //            // Ensure we are accessing ConnectorViewModel properties correctly.
        //            foreach (var dependency in operation.Input
        //                .Where(connector => connector.ConnectedTo != null)
        //                .Select(connector => connector.ConnectedTo?.Parent)
        //                .OfType<OperationViewModel>())
        //            {
        //                Visit(dependency);
        //            }

        //            sorted.Add(operation);
        //        }
        //    }

        //    foreach (var operation in operations)
        //    {
        //        Visit(operation);
        //    }

        //    return sorted;
        //}

        private CalculatorViewModel _calculator = default!;
        public CalculatorViewModel Calculator 
        {
            get => _calculator;
            set => SetProperty(ref _calculator, value);
        }

        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
