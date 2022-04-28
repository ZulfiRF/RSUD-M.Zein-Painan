using Core.Framework.Helper.Presenters;

namespace Core.Framework.Helper.Contracts
{
    public interface IReportPreviewView : IAttachPresenter<ReportPreviewPresenter>
    {
        object SelectedReport { get; set; }
    }

    
}