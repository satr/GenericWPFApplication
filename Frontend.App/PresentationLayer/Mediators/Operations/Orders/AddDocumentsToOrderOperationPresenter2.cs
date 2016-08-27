using System.Collections;
using System.Linq;
using ArcticFramework.BusinessLayer;
using ArcticFramework.WPF.PresentationLayer.Presenters.Operations;
using ArcticFramework.WPF.PresentationLayer.ViewModels;
using DocScanner.BusinessLayer.Documents;
using DocScanner.PresentationLayer.ViewModels.Orders;

namespace DocScanner.PresentationLayer.Presenters.Operations.Orders
{
    public class AddDocumentsToOrderOperationPresenter: ConfirmDialogOperationPresenter, IDataContextWatcher
    {
        private readonly AddDocumentsToOrderConfirmDialogViewModel _viewModel;

        public AddDocumentsToOrderOperationPresenter(IDataContext dataContext)
            : base("Add Document To Order", "Add selected documents to the order", "PresentationLayer/Images/AddDocumentToOrder.png")
        {
            dataContext.WatchSelected<ScannedDocument>(this);
            Enabled = false;
            _viewModel = new AddDocumentsToOrderConfirmDialogViewModel();
        }



        public override void Action()
        {
            base.Action();
            if(ViewModel.Result != ConfirmDialogViewModel.DialogResult.Confirm)
                return;
//            if(_orders.Count != 1)
//                throw new InvalidOperationException("Documents can be added only to one selected order.");
//            var order = _orders.First();
//            foreach (var document in _viewModel.Documents)
//            {
//                if(!order.Documents.Contains(document))
//                    order.Documents.Add(document);
//            }
        }

        protected override ConfirmDialogViewModel CreateViewModel()
        {
            return _viewModel;
        }

        public void NotifyItemsSelected(ICollection items)
        {
            _viewModel.Documents = items.OfType<ScannedDocument>().ToList();
            Enabled = _viewModel.Documents.Count > 0;
        }
    }
}