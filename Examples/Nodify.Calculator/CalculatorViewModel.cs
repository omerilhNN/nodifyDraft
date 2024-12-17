using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Nodify.Calculator
{
    public class CalculatorViewModel : ObservableObject
    {
        private readonly ObservableCollection<OperationViewModel> _droppedOperations;

        public IReadOnlyList<OperationViewModel> DroppedOperations => _droppedOperations;
        public CalculatorViewModel()
        {
            _droppedOperations = new ObservableCollection<OperationViewModel>();
            ExecuteAllOperationsHierCommand = new DelegateCommand(ExecuteAllOperations);

            CreateConnectionCommand = new DelegateCommand<ConnectorViewModel>(
                _ => CreateConnection(PendingConnection.Source, PendingConnection.Target),
                _ => CanCreateConnection(PendingConnection.Source, PendingConnection.Target));
            StartConnectionCommand = new DelegateCommand<ConnectorViewModel>(_ => PendingConnection.IsVisible = true, (c) => !(c.IsConnected && c.IsInput));
            DisconnectConnectorCommand = new DelegateCommand<ConnectorViewModel>(DisconnectConnector);
            DeleteSelectionCommand = new DelegateCommand(DeleteSelection);
            GroupSelectionCommand = new DelegateCommand(GroupSelectedOperations, () => SelectedOperations.Count > 0);

            Connections.WhenAdded(c =>
            {
                c.Input.IsConnected = true;
                c.Output.IsConnected = true;

                c.Input.Value = c.Output.Value;

                c.Output.ValueObservers.Add(c.Input);
            })
            .WhenRemoved(c =>
            {
                var ic = Connections.Count(con => con.Input == c.Input || con.Output == c.Input);
                var oc = Connections.Count(con => con.Input == c.Output || con.Output == c.Output);

                if (ic == 0)
                {
                    c.Input.IsConnected = false;
                }

                if (oc == 0)
                {
                    c.Output.IsConnected = false;
                }

                c.Output.ValueObservers.Remove(c.Input);
            });

            Operations.WhenAdded(x =>
            {
                _droppedOperations.Add(x);
                x.Input.WhenRemoved(RemoveConnection);

                if (x is CalculatorInputOperationViewModel ci)
                {
                    ci.Output.WhenRemoved(RemoveConnection);
                }

                void RemoveConnection(ConnectorViewModel i)
                {
                    var c = Connections.Where(con => con.Input == i || con.Output == i).ToArray();
                    c.ForEach(con => Connections.Remove(con));
                }
            })
            .WhenRemoved(x =>
            {
                foreach (var input in x.Input)
                {
                    DisconnectConnector(input);
                }

                if (x.Output != null)
                {
                    DisconnectConnector(x.Output);
                }
            });

            OperationsMenu = new OperationsMenuViewModel(this);
        }

        ///SİLİNECEKKKKK
        private void PropagateValues(OperationViewModel operation)
        {
            var connections = Connections.Where(c => c.Output.Operation == operation);
            foreach (var connection in connections)
            {
                connection.Input.Value = connection.Output.Value; // Transfer value from output to connected input
            }
        }
        // !!!!!!!!!!

        private NodifyObservableCollection<OperationViewModel> _operations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> Operations
        {
            get => _operations;
            set => SetProperty(ref _operations, value);
        }

        private NodifyObservableCollection<OperationViewModel> _selectedOperations = new NodifyObservableCollection<OperationViewModel>();
        public NodifyObservableCollection<OperationViewModel> SelectedOperations
        {
            get => _selectedOperations;
            set => SetProperty(ref _selectedOperations, value);
        }

        public NodifyObservableCollection<ConnectionViewModel> Connections { get; } = new NodifyObservableCollection<ConnectionViewModel>();
        public PendingConnectionViewModel PendingConnection { get; set; } = new PendingConnectionViewModel();
        public OperationsMenuViewModel OperationsMenu { get; set; }

        public INodifyCommand StartConnectionCommand { get; }
        public INodifyCommand CreateConnectionCommand { get; }
        public INodifyCommand DisconnectConnectorCommand { get; }
        public INodifyCommand DeleteSelectionCommand { get; }
        public INodifyCommand GroupSelectionCommand { get; }
        //!!!!!!!!!!!!
        public INodifyCommand ExecuteAllOperationsHierCommand { get; }
        //!!!!!
        private void ExecuteAllOperations()
        {
            // Perform topological sort to determine execution order
            var sortedOperations = TopologicalSort(_droppedOperations);

            foreach (var operation in sortedOperations)
            {
                operation.ExecuteOperation(); // Executes the operation logic
                PropagateValues(operation);
            }
        }
        /// !!!!! HIEARCHIAL OPERATION EXECUTION
        private IEnumerable<OperationViewModel> TopologicalSort(IEnumerable<OperationViewModel> operations)
        {
            var sorted = new List<OperationViewModel>();
            var visited = new HashSet<OperationViewModel>();
            
            void Visit(OperationViewModel operation)
            {
                if (!visited.Contains(operation))
                {
                    visited.Add(operation);

                    var dependencies = Connections
                        .Where(c => c.Output.Operation == operation)
                        .Select(c => c.Input.Operation)
                        .OfType<OperationViewModel>();
                    foreach (var dependency in dependencies) 
                    { 
                        Visit(dependency);
                    }
                    sorted.Add(operation);
                }
            }
            foreach(var operation in operations)
            {
                Visit(operation);
            }
            return sorted;

        }
            
        
        
        /// !!!!!!

        private void DisconnectConnector(ConnectorViewModel connector)
        {
            var connections = Connections.Where(c => c.Input == connector || c.Output == connector).ToList();
            connections.ForEach(c => Connections.Remove(c));
        }
        public void AddDroppedOperation(OperationViewModel operation)
        {
            _droppedOperations.Add(operation);
        }
        internal bool CanCreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
            => target == null || (source != target && source.Operation != target.Operation && source.IsInput != target.IsInput);

        internal void CreateConnection(ConnectorViewModel source, ConnectorViewModel? target)
        {
            if (target == null)
            {
                PendingConnection.IsVisible = true;
                OperationsMenu.OpenAt(PendingConnection.TargetLocation);
                OperationsMenu.Closed += OnOperationsMenuClosed;
                return;
            }

            var input = source.IsInput ? source : target;
            var output = target.IsInput ? source : target;

            PendingConnection.IsVisible = false;

            DisconnectConnector(input);

            Connections.Add(new ConnectionViewModel
            {
                Input = input,
                Output = output
            });
        }

        private void OnOperationsMenuClosed()
        {
            PendingConnection.IsVisible = false;
            OperationsMenu.Closed -= OnOperationsMenuClosed;
        }

        private void DeleteSelection()
        {
            var selected = SelectedOperations.ToList();
            selected.ForEach(o => Operations.Remove(o));
        }

        private void GroupSelectedOperations()
        {
            var selected = SelectedOperations.ToList();
            var bounding = selected.GetBoundingBox(50);

            Operations.Add(new OperationGroupViewModel
            {
                Title = "Operations",
                Location = bounding.Position,
                GroupSize = new Size(bounding.Width, bounding.Height)
            });
        }
    }
}

