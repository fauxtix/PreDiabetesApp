using PreDiabetes.Models;
using PreDiabetes.PageModels;

namespace PreDiabetes.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}