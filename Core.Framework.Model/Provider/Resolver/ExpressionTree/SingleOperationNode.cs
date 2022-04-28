using System.Linq.Expressions;

namespace Core.Framework.Model.Provider.Resolver.ExpressionTree
{
    internal class SingleOperationNode : Node
    {
        public Node Child { get; set; }
        public ExpressionType Operator { get; set; }
    }
}