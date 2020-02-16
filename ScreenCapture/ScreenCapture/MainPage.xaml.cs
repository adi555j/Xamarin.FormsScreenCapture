using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScreenCapture
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            
        }

        protected void OnButtonClicked(Object sender,EventArgs e)
        {
            
            if(RecordButton.Text == "Start")
            {
                RecordButton.Text = "Stop";
            }
            else
            {
                RecordButton.Text = "Start";
            }
            MessagingCenter.Send<string,string>("OnButtonClicked", "ButtonClickEvent","teue");
        }

        protected async void NavigateButtonClicked(Object sender, EventArgs e)
        {
           await Navigation.PushModalAsync(new Page2());
        }
    }
}
