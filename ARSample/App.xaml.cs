using Xamarin.Forms;
using ARSample.Controls.AR;

namespace ARSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //Navigating to our AR Page
            MainPage = new ARPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
