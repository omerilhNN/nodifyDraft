﻿using System;
using System.Collections.Generic;

namespace Nodify.Calculator
{
    public enum OperationType
    {
        Normal,
        Expando,
        Expression,
        Calculator,
        Group,
        Graph,
        CheckSame,
        CalculateArea,
        RectangleSet,
        ChdFieldSet
    }

    public class OperationInfoViewModel
    {
        public string? Title { get; set; }
        public string? ValueNamespace { get; set; }
        public OperationType Type { get; set; }
        public Type ParentClassType { get; set; }
        public IOperation? Operation { get; set; }
        public List<string?> Input { get; } = new List<string?>();
        public uint MinInput { get; set; }
        public uint MaxInput { get; set; }
    }
}
