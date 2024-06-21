using MobileClient;
using System;
using System.ComponentModel;
using System.Threading;
using Xamarin.Forms;

namespace Mobile_Application {
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage {
        MobileClientManager client;
        string UserInput { get; set; }
        public string filepath { get; set; }
        public bool EndWriting { get; set; }
        //BackgroundWorker worker;
        static EventWaitHandle handler = new AutoResetEvent(false);
        public PhysicalLocation userLocation;
        CancellationTokenSource cts;

        public MainPage(MobileClientManager paramClient) {
            InitializeComponent();
            client = paramClient;
        }

        private void requestButton_Clicked(object sender, EventArgs e) {
            //updateGPSLocation();
            if (resourceSelection.SelectedIndex == 3 && activitySelection.SelectedIndex != 0) {
                DisplayAlert("PDF Action", "PDF files can only be read by the system. Please select the \"Read\" action or select a different resource.",
                    "Ok.");
            }
            else if (activitySelection.SelectedItem != null && resourceSelection.SelectedItem != null && roleSelection.SelectedItem != null &&
                locatonSelection.SelectedItem != null && timeIntervalSelection.SelectedItem != null) {
                //submits all information entered by the user to the form into the package to be sent to the RAM
                client.ReceiveFormRequest(activitySelection.SelectedIndex, resourceSelection.SelectedIndex,
                    roleSelection.SelectedIndex, locatonSelection.SelectedIndex, timeIntervalSelection.SelectedIndex);
                if (client.getClient() != null) {
                    if (client.isConnected()) {
                        if (client.CheckIfValidRequest()) {
                            if (activitySelection.SelectedIndex == 0) {
                                Navigation.PushAsync(new ReadPage(client));
                            }
                            else if (activitySelection.SelectedIndex == 1) {
                                Navigation.PushAsync(new ReadWritePage(client));
                            }
                            else if (activitySelection.SelectedIndex == 2) {
                                Navigation.PushAsync(new WritePage(client));
                            }
                        }
                        else {
                            DisplayAlert("Failed Request!", "You are not currently in a valid location! Please make sure you are attempting to access from"
                                + " a predetermined valid location.", "Ok");
                        }
                    }
                    else {
                        client.ConnectToServer();
                    }
                }
                else {
                    client.ConnectToServer();
                }
            }
        }

        private void logOutButton_Clicked(object sender, EventArgs e) {
            client.CloseAndDisconnect();
            Navigation.PopAsync();
        }

        //LOCATION UPDATE METHOD
        //private async void updateGPSLocation()
        //{
        //    try
        //    {
        //        var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));
        //        cts = new CancellationTokenSource();
        //        userLocation = (PhysicalLocation)await Geolocation.GetLocationAsync(request);
        //        if (userLocation != null)
        //        {
        //            client.Builder.updateUserLocation(userLocation);
        //        }
        //    }
        //    catch (Exception excp)
        //    {
        //        await DisplayAlert("Error", excp.Message, "Ok");
        //    }
        //}
    }
}
