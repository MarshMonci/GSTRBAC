using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MobileClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_Application
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class logInPage : ContentPage
    {
        MobileClientManager client;
        public logInPage()
        {
            InitializeComponent();
            client = new MobileClientManager();
            client.SetServerIPAddress("10.67.65.94"); //For testing, make sure this is not localhost/127.0.0.1!
            client.SetPortNumber("23000");
        }

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(usernameEntry.Text) && !string.IsNullOrEmpty(passwordEntry.Text))
            {
                //Timer ConnectTimeout = new Timer(100);
                if (client.getClient() == null || !client.isConnected() || !client.credentialsAreCorrect())
                {
                    await client.ConnectToServer();
                    //ConnectTimeout.Start();
                    while (true)
                    {
                        if (client.isConnected())
                        {
                            await client.CredentialVerification(usernameEntry.Text.Trim(), passwordEntry.Text.Trim());
                            break;
                        }
                        else
                        {
                            await client.ConnectToServer();
                        }
                    }
                }
            }
                if (client.getClient() != null)
                {
                    //submits all information entered by the user to the form into the package to be sent to the RAM
                    //client.CredentialVerification(usernameEntry.Text.Trim(), passwordEntry.Text.Trim());
                    if (client.credentialsAreCorrect())
                    {
                        await Navigation.PushAsync(new MainPage(client));
                    }
                    else if (client.isConnected() && !client.credentialsAreCorrect())
                    {
                        await DisplayAlert("Error!", "Incorrect username or password!", "Ok");
                    }
                }
            else
            {
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