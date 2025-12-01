

using RealStateApp.Core.Application.ViewModels.Message;
using RealStateApp.Core.Application.ViewModels.offer;

namespace RealStateApp.Core.Application.ViewModels.Properties
{
    public class DetailsPropertyCompleteViewModel
    {

        public DetailsPropertyViewModel? DetailsPropertyViewModel { get; set; }
        public CreateMessageViewModel CreateMessageViewModel { get; set; } = new() { Content = "", ReceiverUserId = "", SenderUserId = "" };
        public CreateOfferViewModel? CreateOfferViewModel { get; set; }

        public List<DataOfferViewModel> DataListOffer { get; set; } = new List<DataOfferViewModel>();
        public List<DataConversactionViewModel> DataConversactionList { get; set; } = new List<DataConversactionViewModel>();

        public bool DesableButtonOffer { get; set; }


    }
}
