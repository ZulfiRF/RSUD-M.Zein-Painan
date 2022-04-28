namespace Core.Framework.Windows.Contracts
{
    using System;
    using System.Windows;

    public interface IMainWindow
    {
        #region Public Methods and Operators

        void ChangeContent(
            FrameworkElement controlOld,
            FrameworkElement controlNew,
            string header = "",
            Action complete = null);

        void RemoveContent(FrameworkElement control);

        void SetContent(FrameworkElement control, string header = "");

        #endregion

        void SetInformationUser(string userName, string namaRuangan);
        void ShowWindow();
        bool ClearAll { set; }
        int GetDocumentCount();
        void CloseWindow();
    }

    public interface INewMainWindow
    {
        void ChangeContent(FrameworkElement oldFrameworkElement, FrameworkElement newFrameworkElement, string Title);
        void BackHome();
        void ShowMessage(string title, string msg);
    }
}