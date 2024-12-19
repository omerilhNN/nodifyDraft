using Nodify.Calculator.CodeGenerationTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ExecuteAllOperationsAndGenerateCodeCommand = new DelegateCommand(ExecuteAllOperationsAndGenerateCode);

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
        public INodifyCommand ExecuteAllOperationsAndGenerateCodeCommand { get; }
        public INodifyCommand GenerateCodeCommand { get; }
        //!!!!! ELLE EKLEDİM : ExecuteAllOperations
        private void  ExecuteAllOperationsAndGenerateCode()
        {
          //!!! REVERSE için Açıklama !!!
          //CODE GENERATION'DA nodelardan generate edilen kod ters sırada yazılıyordu bunun önüne geçmek için REVERSE metodu eklendi!!!!
            var sortedOperations = TopologicalSort(_droppedOperations).ToList();
            sortedOperations.Reverse();

            foreach (var operation in sortedOperations)
            {
                 ExecuteAllOperations(operation);
            }
            var generatedCode = GenerateCodeFromOperations(sortedOperations);

            var directoryPath = Path.Combine("..", "..", "..", "GeneratedCode");
            var filePath = Path.Combine(directoryPath, "GeneratedOperationsCode_GEN.cs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, generatedCode);
        }
        private void ExecuteAllOperations(OperationViewModel operation)
        {
            if (operation != null)
            {
                Console.WriteLine($"Executing operation: {operation.GetType().Name}");

                try
                {
                    // Execute the operation logic
                    operation.ExecuteOperation();

                    // Ensure values propagate after execution
                    PropagateValues(operation);

                    Console.WriteLine($"Completed operation: {operation.GetType().Name}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error executing operation {operation.GetType().Name}: {ex.Message}");
                    throw; // Propagate the exception if needed
                }
            }
        }
        /// !!!!! HIEARCHIAL OPERATION EXECUTION
        private IEnumerable<OperationViewModel> TopologicalSort(IEnumerable<OperationViewModel>  operations)
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
            //foreach(var operation in operations)
            //{
            //    Visit(operation);
            //}
            foreach (var operation in operations)
            {
                if (!Connections.Any(c => c.Input.Operation == operation))
                {
                    Visit(operation);
                }
            }
            return sorted;

        }
      

        /// <-!!!!!!
        /// CODE GENERATION YAPILACAK ALAN
        private string GenerateCodeFromOperations(IEnumerable<OperationViewModel> sortedOperations)
        {
            var sb = new StringBuilder();

            sb.AppendLine($@"using System;");
            sb.AppendLine($@"using DriverBase;");
            sb.AppendLine($@"using DriverBase_Platform;");
            sb.AppendLine($@"using Octopus;");
            sb.AppendLine($@"using Nodify.Calculator;");

            sb.AppendLine($@"public class TC_GENERATED_OMER : AbsTesterDriver");
            sb.AppendLine("{");

                sb.AppendLine("static Octolog Log = new Octolog();");
                sb.AppendLine($@"public void Setup(){{
                SetupBase();
                Log.Set_Author(""Mehmet Erkuş"");
                Log.Set_ExecutedBy(""Mehmet Erkuş"");
                Log.Set_UutVersion(""SRS_LLR_DISCRET_IN(Baseline 7.0), SRS_DISCRETE_IN(Baseline 5.0), DD_DISCRETE_IN(Baseline 7.0)"");
                if (SuiteConfig.configId == ConfigId.MANUAL)
                {{
                            Defs.TIME_OUT = 1_000_000; 
                }}");
            sb.AppendLine("}");
            sb.AppendLine($@" public static class Defs{{
            public static int TIME_OUT = 1_000; // 1 second
            public static byte True = 1;
            public static byte False = 0;");
            sb.AppendLine("}");
            sb.AppendLine($@"public void TCF_OFI()");
            sb.AppendLine("{");
            sb.AppendLine("Setup();");
            foreach (var operation in sortedOperations)
            {
                if (operation is RectangleSetOperationViewModel rectangleSet)
                {
                    sb.AppendLine($"var rectangle = new RectangleViewModel();");
                    sb.AppendLine($"rectangle.Width=  {rectangleSet.Input[0].Value} ;;");
                    sb.AppendLine($"rectangle.Width= {rectangleSet.Input[0].Value};");


                }
                else if (operation is CalculateAreaViewModel calculateArea)
                {

                    sb.AppendLine($"        rectangle.Area = {calculateArea.Output.Value};");
                }
                else if (operation is CheckSameOperationViewModel checkSame)
                {
                    sb.AppendLine($@"        Log.CheckSame({operation.Input[0].Value}, {operation.Input[1].Value},
                        Spec.CFR($@""[REQ];
                        Verify result to DRIVER_INVALID_PARAMETER.""));");
                }
            }
            sb.AppendLine("}");


            sb.AppendLine("}");
            //string directoryPath = Path.Combine("..", "..", "..", "GeneratedCode");
            //string filePath = Path.Combine(directoryPath, "GeneratedOperationsCode_GEN.cs");

            //if (!Directory.Exists(directoryPath))
            //{
            //    Directory.CreateDirectory(directoryPath);
            //}
            //File.WriteAllText(filePath, sb.ToString());

            //// Optionally inform the user
            //Console.WriteLine($"Code generated successfully! Saved to {filePath}");
            return sb.ToString();
        }

        /// !!!!->


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

