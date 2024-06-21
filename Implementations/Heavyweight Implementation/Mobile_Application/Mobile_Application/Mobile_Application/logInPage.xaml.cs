using MobileClient;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_Application {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class logInPage : ContentPage {
        MobileClientManager client;
        public logInPage(string androidFilepath) {
            InitializeComponent();
            client = new MobileClientManager();
            client.setAndroidFilepath(androidFilepath);
            client.SetServerIPAddress("192.168.1.228"); //For testing, make sure this is not localhost/127.0.0.1!
            client.SetPortNumber("23000");
        }

        private async void LoginButton_Clicked(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(usernameEntry.Text) && !string.IsNullOrEmpty(passwordEntry.Text)) {
                //Timer ConnectTimeout = new Timer(100);
                if (client.getClient() == null || !client.isConnected() || !client.credentialsAreCorrect()) {
                    await client.ConnectToServer();
                    //ConnectTimeout.Start();
                    while (true) {
                        if (client.isConnected()) {
                            await client.CredentialVerification(usernameEntry.Text.Trim(), passwordEntry.Text.Trim());
                            break;
                        }
                        else {
                            await client.ConnectToServer();
                        }
                    }
                }
            }
            if (client.getClient() != null) {
                //submits all information entered by the user to the form into the package to be sent to the RAM
                //client.CredentialVerification(usernameEntry.Text.Trim(), passwordEntry.Text.Trim());
                if (client.credentialsAreCorrect()) {
                    await Navigation.PushAsync(new MainPage(client));
                }
                else if (client.isConnected() && !client.credentialsAreCorrect()) {
                    await DisplayAlert("Error!", "Incorrect username or password!", "Ok");
                }
            }
            else {
                await DisplayAlert("Error!", "Username and Password must be entered before connection can be made!", "Ok");
            }
        }

        //protected override void OnAppearing()
        //{
        //    if (client.getClient() != null)
        //    {
        //        if (client.isConnected())
        //        {
        //            client.CloseAndDisconnect();
        //        }
        //    }

        //    base.OnAppearing();
        //}
    }
}