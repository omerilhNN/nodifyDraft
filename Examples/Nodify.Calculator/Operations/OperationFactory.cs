using BSP_Drivers_Common_DD_DRIVER_TYPES;
using DriverBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Tmds.DBus.Protocol;

namespace Nodify.Calculator
{
    public static class OperationFactory
    {
        public static List<OperationInfoViewModel> GetOperationsInfo(Type container)
        {
            List<OperationInfoViewModel> result = new List<OperationInfoViewModel>();

            //DELEGATE TANIMLAMALARI İLE RUNTİME'DA YAKALANIR VE ÇALIŞTIRILIR -> PERFORMANS ++++
            foreach (var method in container.GetMethods())
            {
                if (method.IsStatic)
                {
                    OperationInfoViewModel op = new OperationInfoViewModel
                    {
                        Title = method.Name
                    };

                    var attr = method.GetCustomAttribute<OperationAttribute>();
                    var para = method.GetParameters();

                    bool generateInputNames = true;

                    op.Type = OperationType.Normal;
                    //if (method.ReturnType == typeof(ChdViewModel))
                    //{
                    //    var delType = typeof(Func<List<dynamic>, ChdViewModel>);
                    //    var del = (Func<List<dynamic>, ChdViewModel>)Delegate.CreateDelegate(delType, method);
                    //    op.Type = OperationType.ChdFieldSet;
                    //    op.Operation = new ChdSetOperation(del);
                    //}
                    ////2 parametresi CHD ViewModel
                    //if (method.ReturnType == typeof(object) && method.GetParameters().Length == 1 &&
                    //    method.GetParameters().First().ParameterType == typeof(List<object>))
                    //{
                    //    var delType = typeof(Func<List<object>, object>);
                    //    var del = (Func<List<object>, object>)Delegate.CreateDelegate(delType, method);

                    //    op.Operation = new ChdSetOperation(del);
                    //}
                    if (para.Length == 2 && para.All(p => p.ParameterType == typeof(Nodify.Calculator.ChdViewModel)))
                    {
                        //2 tane chd viewModel soktuğumuz CheckSame
                        //if(method.ReturnType == typeof(bool))
                        //{
                        //    var delType = typeof(Func<ChdViewModel, ChdViewModel, bool>);
                        //    var del = (Func<ChdViewModel, ChdViewModel, bool>)Delegate.CreateDelegate(delType, method);
                        //    op.Operation = new CheckSameOperation<ChdViewModel>(del);
                        //}
                    }
                    if (para.Length == 2)
                    {
                        //if(method.ReturnType == typeof(double))
                        //{
                        //    var delType = typeof(Func<double, double, double>);
                        //    var del = (Func<double, double, double>)Delegate.CreateDelegate(delType, method);

                        //    op.Operation = new BinaryOperation(del);
                        //}
                        //if(method.ReturnType == typeof(bool))
                        //{
                        //    var delType = typeof(Func<double, double, bool>);
                        //    var del = (Func<double, double, bool>)Delegate.CreateDelegate(delType, method);
                        //    op.Operation = new CheckSameOperation<double>(del);
                        //}
                        //if(method.ReturnType == typeof(RectangleViewModel))
                        //{
                        //    var delType = typeof(Func<double,double, RectangleViewModel>);
                        //    var del = (Func<double, double, RectangleViewModel>)Delegate.CreateDelegate(delType, method);
                        //    op.Operation = new RectangleSetOperation(del);
                        //}
                    }
                    else if (para.Length == 1)
                    {
                        //if ( para[0].ParameterType == typeof(RectangleViewModel)
                        //&& method.ReturnType == typeof(double))
                        //{
                        //    var delType = typeof(Func<RectangleViewModel, double>);
                        //    var del = (Func<RectangleViewModel, double>)Delegate.CreateDelegate(delType, method);
                        //    op.Operation = new CalculateAreaOperation(del);
                        //}
                        //if (para[0].ParameterType.IsArray)
                        //{
                        //    op.Type = OperationType.Expando;

                        //    var delType = typeof(Func<double[], double>);
                        //    var del = (Func<double[], double>)Delegate.CreateDelegate(delType, method);

                        //    op.Operation = new ParamsOperation(del);
                        //    op.MaxInput = int.MaxValue;
                        //}
                        //else
                        //{
                        //    var delType = typeof(Func<double, double>);
                        //    var del = (Func<double, double>)Delegate.CreateDelegate(delType, method);

                        //    op.Operation = new UnaryOperation(del);
                        //}
                    }
                

                      
                    else if (para.Length == 0)
                    {
                        var delType = typeof(Func<double>);
                        var del = (Func<double>)Delegate.CreateDelegate(delType, method);

                        op.Operation = new ValueOperation(del);
                    }

                    if (attr != null)
                    {
                        op.MinInput = attr.MinInput;
                        op.MaxInput = attr.MaxInput;
                        generateInputNames = attr.GenerateInputNames;
                    }
                    else
                    {
                        op.MinInput = (uint)para.Length;
                        op.MaxInput = (uint)para.Length;
                    }

                    foreach (var param in para)
                    {
                        op.Input.Add(generateInputNames ? param.Name : null);
                    }

                    for (int i = op.Input.Count; i < op.MinInput; i++)
                    {
                        op.Input.Add(null);
                    }

                    result.Add(op);
                }
            }

            return result;
        }

        public static OperationViewModel GetOperation(OperationInfoViewModel info)
        {
            var input = info.Input.Select(i => new ConnectorViewModel
            {
                Title = i
            });

            switch (info.Type)
            {
                //Özelleştirilmiş bir Operation kullanmak istediğinde bu kısmı kullanman gerekir
                case OperationType.CalculateArea:
                    return new CalculateAreaViewModel
                    { 
                        Title = info.Title,
                        Output = new ConnectorViewModel(),
                        Operation = info.Operation,
                    };
                case OperationType.RectangleSet:
                    return new RectangleSetOperationViewModel { 
                        Title = info.Title,
                        Output = new ConnectorViewModel(),
                        Operation = info.Operation
                    };
                case OperationType.CheckSame:
                return new CheckSameOperationViewModel
                {
                    Title = info.Title,
                    Operation = info.Operation,
                    Output = new ConnectorViewModel
                    {
                       ValueType = typeof(Boolean)
                    }
                };
                case OperationType.Calculator:
                    return new CalculatorOperationViewModel
                    {
                        Title = info.Title,
                        Operation = info.Operation,
                    };
                case OperationType.ChdFieldSet:
                    return new ChdFieldSetOperationViewModel(info.ParentClassType)
                    {
                        Title = info.Title,
                        Operation = info.Operation,
                        Output = new ConnectorViewModel
                        {
                            ValueType = typeof(ChdViewModel)
                        }

                    };
                default:
                {
                    var op = new OperationViewModel
                    {
                        Title = info.Title,
                        Output = new ConnectorViewModel(),
                        Operation = info.Operation,
                    };

                    op.Input.AddRange(input);
                    return op;
                }
            }
        }
    }
}
