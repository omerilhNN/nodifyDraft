using DriverBase;
using Nodify.Calculator.CodeGenerationTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Nodify.Calculator
{
    public class CalculatorViewModel : ObservableObject
    {
        private readonly ObservableCollection<OperationViewModel> _droppedOperations;

        public IReadOnlyList<OperationViewModel> DroppedOperations => _droppedOperations;
        public static CalculatorViewModel Instance { get; private set; }
        public CalculatorViewModel()
        {
            Instance = this;
            _droppedOperations = new ObservableCollection<OperationViewModel>();
            ExecuteAllOperationsAndGenerateCodeCommand = new AsyncDelegateCommand(ExecuteAllOperationsAndGenerateCode);

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
        private async Task ExecuteAllOperationsAndGenerateCode()
        {
          //!!! REVERSE için Açıklama !!!
          //CODE GENERATION'DA nodelardan generate edilen kod ters sırada yazılıyordu bunun önüne geçmek için REVERSE metodu eklendi!!!!
            var sortedOperations = TopologicalSort(_droppedOperations).ToList();
            sortedOperations.Reverse();

            foreach (var operation in sortedOperations)
            {
                await ExecuteOperationAsync(operation);
            }
            var generatedCode = GenerateCodeFromOperations(sortedOperations);
            var directoryPath = Path.Combine("..", "..", "..", "GeneratedCode");
            var filePath = Path.Combine(directoryPath, "GeneratedOperationsCode_GEN.cs");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(filePath, generatedCode);

            var assembly = CompileCode(filePath);
            await ExecuteMethod(assembly, "TC_GENERATED_OMER", "TCF_OFI");
        }
        private Assembly CompileCode(string filePath)
        {
            var code = File.ReadAllText(filePath);

            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location), // System.Private.CoreLib
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location), // System.Console
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location), // System.Linq

        // Add reference to System.Runtime
                MetadataReference.CreateFromFile(Path.Combine(
                    System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(),
                    "System.Runtime.dll")),

        // Add reference to Nodify.Calculator
                MetadataReference.CreateFromFile(@"Nodify.Calculator.dll"),
                MetadataReference.CreateFromFile(@"DriverBase.dll"),
                MetadataReference.CreateFromFile(@"Octopus.dll"),
                MetadataReference.CreateFromFile(@"OctopusDriverBase.dll"),
        // Add reference to OctopusDriverBase
    };
        


            // Validate that all references exist
            foreach (var reference in references)
            {
                if (!File.Exists(reference.Display))
                {
                    throw new FileNotFoundException($"Reference not found: {reference.Display}");
                }
            }

            var compilation = CSharpCompilation.Create("GeneratedAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references)
                .AddSyntaxTrees(syntaxTree);


            using (var ms = new MemoryStream()) 
            { 
                var result = compilation.Emit(ms); //derleme işleminin sonucunu MemoryStream gibi bir belleğe yazma işlemini gerçekleştirir- Emit
                if (!result.Success)
                {
                    var errors = result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d=> d.ToString());
                    throw new InvalidOperationException($"COMPILATION FAILED" + string.Join("\n",errors));
                }
                ms.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(ms.ToArray());  // bellekte derlenmiş kodu yükleyerek çalışma zamanında kullanılabilir hale getirir
            }
        }
        private async Task ExecuteMethod(Assembly assembly, string className,string methodName)
        {
            var type = assembly.GetType(className);
            if(type == null)
            {
                throw new InvalidOperationException($"{className} doesn't exist");
            }

            var method = type.GetMethod(methodName);    
            if(method == null)
            {
                throw new InvalidOperationException($"Method {methodName} not found");
            }

            object instance = null;
            if (!method.IsStatic)
            {
                instance = Activator.CreateInstance(type);
                if (instance == null)
                {
                    throw new InvalidOperationException($"Failed to create an instance of class '{className}'.");
                }
            }
            // Invoke the method
            try
            {
                method.Invoke(instance, null);
            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine($"Inner Exception Message: {ex.InnerException?.Message}");
                Console.WriteLine($"Inner Exception Stack Trace: {ex.InnerException?.StackTrace}");
                throw;
            }
        }

        //Ekrana bırakılan operationların asenkron çalıştırılması
        private async Task ExecuteOperationAsync(OperationViewModel operation)
        {
            #region TASK COMPLETION ROUTINE HALİ
            //if (operation != null)
            //{
            //    Console.WriteLine($"Executing operation: {operation.GetType().Name}");

            //    // Use TaskCompletionSource to wait for execution
            //    var tcs = new TaskCompletionSource<bool>();

            //    // Start executing the operation
            //    Task.Run(async () =>
            //    {
            //        try
            //        {
            //            operation.ExecuteOperation(); // Execute the operation logic
            //            PropagateValues(operation);  // Ensure values propagate after execution

            //            tcs.SetResult(true); // Indicate completion
            //            await Task.Delay(100);
            //        }
            //        catch (Exception ex)
            //        {
            //            tcs.SetException(ex); // Handle any exceptions
            //        }
            //    });
            //    await tcs.Task;
            //try
            //{
            //    // Execute the operation asynchronously
            //    await Task.Run(() =>
            //    {
            //        operation.ExecuteOperation(); // Execute the operation logic
            //        PropagateValues(operation);  // Ensure values propagate after execution

            //        tcs.SetResult(true); // Indicate completion
            //    });

            //    // Await the completion of the TaskCompletionSource
            //    await tcs.Task;

            //    Console.WriteLine($"Completed operation: {operation.GetType().Name}");
            //}
            //catch (Exception ex)
            //{
            //    tcs.SetException(ex); // Handle any exceptions
            //    Console.WriteLine($"An error occurred: {ex.Message}");
            //}
            //}
            #endregion
            if (operation == null) return;
            Console.WriteLine($"Executing operation: {operation.GetType().Name}");

            try
            {
                await Task.Run(() =>
                {
                    operation.ExecuteOperation(); // Perform the operation logic
                    PropagateValues(operation);  // Ensure values propagate
                });

                await Task.Delay(100);

                await TriggerDependentOperationsAsync(operation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing operation {operation.GetType().Name}: {ex.Message}");
            }
        }
        
        /// !! Operationları Dependencylerine göre sıralama işlemi
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

        //Bir operasyonun çıtkısına bağlı olarak diğer operasyonları asenkron olarak tetiklemek ve sıralı bir şekilde yürütmek için
        private async Task TriggerDependentOperationsAsync(OperationViewModel operation)
        {
            if (operation == null) return;

            //Connections: Tüm bağlantılar
            // Çıkışı şu anki operasyondan üretilen -- Girişine gelen node'un operation'ı boş olmayanları al ->> Giriş düğümündeki operasyonları seç listede tut
            var dependentOperations = Connections
                .Where(c => c.Output.Operation == operation && c.Input.Operation != null)
                .Select(c => c.Input.Operation)
                .OfType<OperationViewModel>()
                .ToList();

            foreach (var dependentOperation in dependentOperations)
            {
                await ExecuteOperationAsync(dependentOperation); // Her dependent operasyon asenkron çalıştır
            }
        }
        ///IEnumerable -> LazyEvaluation var - kodun veri kaynağına bağımlılığı azaltılır veri kaynağı değişse de bir sorun yaratmaz 
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
                Log.Set_Author(""Ömer Faruk İlhan"");
                Log.Set_ExecutedBy(""Ömer Faruk İlhan"");
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
                    sb.AppendLine($"rectangle.Width=  {rectangleSet.Input[0].Value};");
                    sb.AppendLine($"rectangle.Height= {rectangleSet.Input[1].Value};");


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
                else if (operation is ChdFieldSetOperationViewModel chdFieldSet)
                {
                    sb.AppendLine($@"           var msgo = new {chdFieldSet.ValueNamespace}.{chdFieldSet.Title}();");
                    foreach(var input in chdFieldSet.Input)
                    {
                        sb.AppendLine($@"           msgo.chd.{input.Title} = {input.Value};");
                    }
                    sb.AppendLine($@"           uut.SendMsg(msgo);");
                    sb.AppendLine($@"           var msgi = uut.GetMsg<{chdFieldSet.ValueNamespace}.{chdFieldSet.Title}>();");
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

