using System.Linq.Expressions;

namespace Core.Framework.Model.Provider.Resolver.ExpressionTree
{
    internal class OperationNode : Node
    {
        public Node Left { get; set; }
        public ExpressionType Operator { get; set; }
        public Node Right { get; set; }
    }
}