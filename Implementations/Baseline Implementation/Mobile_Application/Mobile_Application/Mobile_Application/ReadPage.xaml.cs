using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileClient;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using System.ComponentModel;
using System.Timers;
using System.Diagnostics;

namespace Mobile_Application
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadPage : ContentPage
    {
        MobileClientManager client;
        Timer validZoneTimer;

        public ReadPage(MobileClientManager paramClient)
        {
            InitializeComponent();
            client = paramClient;
            client.SetTextBox(bodyText);
            //PdfDocument PdfResource = new PdfDocument();
            //client.SetPdfReader(PdfResource);
            client.RequestResources(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData));
            StartValidationTimer();
        }

        private async Task StartValidationTimer()
        {
            validZoneTimer = new Timer(5000);
            validZoneTimer.AutoReset = false;
            validZoneTimer.Enabled = true;
            try
            {
                validZoneTimer.Elapsed += OnValidationEvent;
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }

        private async void OnValidationEvent(Object source, ElapsedEventArgs e)
        {
            bool accessGranted = false;
            try
            {
                if (client.CheckIfValidRequest()) 
                {
                    accessGranted = client.CheckOngoingAuthorization();
                    if (!accessGranted)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Access expired!", "Access has been revoked due to being in an invalid zone! " +
                                "Please check to make sure you are still accessing from a valid time and location if you wish to continue access.", "Ok");
                            await Navigation.PopAsync();
                        });
                        validZoneTimer.Stop();
                        validZoneTimer.Enabled = false;
                    }
                    else if (accessGranted)
                    {
                        validZoneTimer.Start();
                        validZoneTimer.Enabled = true;
                    }
                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Access expired!", "Access has been revoked due to being in an invalid zone! " +
                            "Please check to make sure you are still accessing from a valid time and location if you wish to continue access.", "Ok");
                    });
                    validZoneTimer.Stop();
                    validZoneTimer.Enabled = false;
                    await Navigation.PopAsync();
                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }

        protected override void OnAppearing()
        {
            if (!validZoneTimer.Enabled)
            {
                StartValidationTimer();
            }
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            validZoneTimer.Stop();
            validZoneTimer.Enabled = false;
            base.OnDisappearing();
        }
    }
}