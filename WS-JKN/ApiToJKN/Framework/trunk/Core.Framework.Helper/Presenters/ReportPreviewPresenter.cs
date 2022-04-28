using Core.Framework.Helper.Commands;
using Core.Framework.Helper.Contracts;

namespace Core.Framework.Helper.Presenters
{
    public class ReportPreviewPresenter : BasePresenterViewCommand<IReportPreviewView, IReportPreviewCommand>
    {
        public ReportPreviewPresenter(IReportPreviewView view, IReportPreviewCommand command) : base(view, command) { }

        public override void Initialize(params object[] parameters)
        {
            foreach (var parameter in parameters)
            {
                View.SelectedReport = parameter;
            }
            base.Initialize(parameters);
        }
    }

    
}