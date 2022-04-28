using Core.Framework.Model.Provider.ValueObjects;

namespace Core.Framework.Model.Provider.Resolver.ExpressionTree
{
    internal class LikeNode : Node
    {
        public MemberNode MemberNode { get; set; }
        public LikeMethod Method { get; set; }
        public string Value { get; set; }
    }
}