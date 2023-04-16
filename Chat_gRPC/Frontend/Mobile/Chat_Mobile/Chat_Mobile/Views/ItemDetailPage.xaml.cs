using Chat_Mobile.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace Chat_Mobile.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}