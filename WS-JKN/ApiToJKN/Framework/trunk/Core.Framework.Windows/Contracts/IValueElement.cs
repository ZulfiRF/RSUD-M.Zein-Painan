namespace Core.Framework.Windows.Contracts
{
    public interface IValueElement
    {
        bool CanFocus { get; }
        string Key { get; }
        object Value { get; set; }
    }
}