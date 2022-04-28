namespace Core.Framework.Windows.Helper.DragDropFramework
{
    public interface IControlProvider
    {
        bool CanDrag { get; set; }
        void FirsLoad();
        void Edited();
    }
}