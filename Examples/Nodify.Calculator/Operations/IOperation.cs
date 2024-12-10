namespace Nodify.Calculator
{
    public interface IOperation
    {
        object Execute(params object[] operands);
    }
}
