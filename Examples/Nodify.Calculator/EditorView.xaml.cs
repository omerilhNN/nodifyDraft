using Avalonia.Media;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nodify.Calculator
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            InitializeComponent();

            PointerPressedEvent.AddClassHandler<NodifyEditor>(CloseOperationsMenuPointerPressed);
            ItemContainer.DragStartedEvent.AddClassHandler<ItemContainer>(CloseOperationsMenu);
            PointerReleasedEvent.AddClassHandler<NodifyEditor>(OpenOperationsMenu);
            Editor.AddHandler(DragDrop.DropEvent, OnDropNode); // OnDropNode eventi burada handle edilmektedir

            if (DataContext is OperationViewModel viewModel)
            {
                UpdateGradientBrush(viewModel.IsSuccess); // MANUEL EKLENDİ İLERİDE SİLİNEBİLİR
                viewModel.PropertyChanged += OnViewModelPropertyChanged;
            }
        }


        private void OpenOperationsMenu(object? sender, PointerReleasedEventArgs e)
        {
            if (!e.Handled && e.Source is NodifyEditor editor && !editor.IsPanning && editor.DataContext is CalculatorViewModel calculator &&
                e.InitialPressMouseButton == MouseButton.Right)
            {
                e.Handled = true;
                calculator.OperationsMenu.OpenAt(editor.MouseLocation);
            }
        }

        /// !!!!!!!!!!!!!!!!!!! 
        /// MANUEL EKLEDIN UNUTMA - SİLİNECEK
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OperationViewModel.IsSuccess) && sender is OperationViewModel viewModel)
            {
                UpdateGradientBrush(viewModel.IsSuccess);
            }
        }
        /// <!--  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! SONRADAN EKLENMİŞ OLAN KISIMLAR 
        /// 
      
        private void UpdateGradientBrush(bool? isSuccess)
        {
            var brush = (LinearGradientBrush)Resources["BorderBrushDRAFT"];
            if (brush != null)
            {
                var startColor = (bool)isSuccess ? Colors.Green : Colors.Red;
                var middleColor = (bool)isSuccess ? Colors.LightGreen : Colors.Orange;
                var endColor = (bool)isSuccess ? Colors.DarkGreen : Colors.DarkRed;

                brush.GradientStops[0].Color = startColor;
                brush.GradientStops[1].Color = middleColor;
                brush.GradientStops[2].Color = endColor;
            }
            else
            {

            }
        }

        /// ---->!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        private void CloseOperationsMenuPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.PointerUpdateKind == PointerUpdateKind.LeftButtonPressed)
                CloseOperationsMenu(sender, e);
        }
        
        private void CloseOperationsMenu(object? sender, RoutedEventArgs e)
        {
            ItemContainer? itemContainer = sender as ItemContainer;
            NodifyEditor? editor = sender as NodifyEditor ?? itemContainer?.Editor;

            if (!e.Handled && editor?.DataContext is CalculatorViewModel calculator)
            {
                calculator.OperationsMenu.Close();
            }
        }
        ///<!--  !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! DroppedNode listesine ekleme yapacağım
        private void OnDropNode(object? sender, DragEventArgs e)
        {
            NodifyEditor? editor = (e.Source as NodifyEditor) ?? (e.Source as Control)?.GetLogicalParent() as NodifyEditor;
            if(editor != null && editor.DataContext is CalculatorViewModel calculator
                && e.Data.Get(typeof(OperationInfoViewModel).FullName) is OperationInfoViewModel operation)
            {
                OperationViewModel op = OperationFactory.GetOperation(operation);
                op.Location = editor.GetLocationInsideEditor(e);
                calculator.Operations.Add(op);

                e.Handled = true;
                // Add the operation to the ViewModel's list
                //if (DataContext is EditorViewModel viewModel)
                //{
                //    viewModel.AddDroppedOperation(operation);
                //}
            }
        }
        // -->>>>>> 
        private void OnNodeDrag(object? sender, MouseEventArgs e)
        {
            if(leftButtonPressed && ((Control)sender).DataContext is OperationInfoViewModel operation)
            {
                var data = new DataObject();
                data.Set(typeof(OperationInfoViewModel).FullName, operation);
                DragDrop.DoDragDrop(e, data, DragDropEffects.Copy);
            }
        }

        private void OnNodePressed(object? sender, PointerPressedEventArgs e)
        {
            leftButtonPressed = e.GetCurrentPoint(this).Properties.PointerUpdateKind ==
                                PointerUpdateKind.LeftButtonPressed;
        }

        private void OnNodeExited(object? sender, PointerEventArgs e)
        {
            leftButtonPressed = false;
        }
        
        private bool leftButtonPressed;
    }
}
