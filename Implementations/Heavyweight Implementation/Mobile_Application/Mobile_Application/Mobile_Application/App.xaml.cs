using Xamarin.Forms;

namespace Mobile_Application {
    public partial class App : Application {
        public App(string androidFilePath) {
            InitializeComponent();

            MainPage = new NavigationPage(new logInPage(androidFilePath));
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
