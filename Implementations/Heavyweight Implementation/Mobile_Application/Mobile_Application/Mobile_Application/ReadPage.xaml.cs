using MobileClient;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Mobile_Application {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadPage : ContentPage {
        MobileClientManager client;
        Timer validZoneTimer;
        //private Stream pdfDocumentStream;
        ///// <summary>
        ///// The PDF document stream that is loaded into the instance of the PDF viewer. 
        ///// </summary>
        //public Stream PdfDocumentStream
        //{
        //    get
        //    {
        //        return m_pdfDocumentStream;
        //    }
        //    set
        //    {
        //        m_pdfDocumentStream = value;
        //        NotifyPropertyChanged("PdfDocumentStream");
        //    }
        //}

        ///// <summary>
        ///// An event to detect the change in the value of a property.
        ///// </summary>
        //public event PropertyChangedEventHandler PropertyChanged;

        public ReadPage(MobileClientManager paramClient) {
            InitializeComponent();
            client = paramClient;
            client.SetTextBox(bodyText);
            //PdfDocument PdfResource = new PdfDocument();
            //client.SetPdfReader(PdfResource);
            client.RequestResources(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), paramClient.UserCryptoInfo);
            StartValidationTimer();
        }

        private async Task StartValidationTimer() {
            validZoneTimer = new Timer(5000);
            validZoneTimer.AutoReset = false;
            validZoneTimer.Enabled = true;
            try {
                validZoneTimer.Elapsed += OnValidationEvent;
            }
            catch (Exception excp) {
                Debug.WriteLine(excp.ToString());
            }
        }

        private async void OnValidationEvent(Object source, ElapsedEventArgs e) {
            bool accessGranted = false;
            try {
                if (client.CheckIfValidRequest()) {
                    accessGranted = client.CheckOngoingAuthorization(client.UserCryptoInfo);
                    if (!accessGranted) {
                        Device.BeginInvokeOnMainThread(async () => {
                            await DisplayAlert("Access expired!", "Access has been revoked due to being in an invalid zone! " +
                                "Please check to make sure you are still accessing from a valid time and location if you wish to continue access.", "Ok");
                            await Navigation.PopAsync();
                        });
                        validZoneTimer.Stop();
                        validZoneTimer.Enabled = false;
                    }
                    else if (accessGranted) {
                        validZoneTimer.Start();
                        validZoneTimer.Enabled = true;
                    }
                }
                else {
                    Device.BeginInvokeOnMainThread(async () => {
                        await DisplayAlert("Access expired!", "Access has been revoked due to being in an invalid zone! " +
                            "Please check to make sure you are still accessing from a valid time and location if you wish to continue access.", "Ok");
                        await Navigation.PopAsync();
                    });
                    validZoneTimer.Stop();
                    validZoneTimer.Enabled = false;
                }
            }
            catch (Exception excp) {
                Debug.WriteLine(excp.ToString());
            }
        }

        protected override void OnAppearing() {
            if (!validZoneTimer.Enabled) {
                StartValidationTimer();
            }
            base.OnAppearing();
            //pdfDocumentStream = typeof(App).Assembly.GetManifestResourceStream(System.Environment.GetFolderPath(
            //System.Environment.SpecialFolder.LocalApplicationData) + "Request.pdf");
            //Load the PDF
            //pdfViewerControl.LoadDocument(pdfDocumentStream);
        }

        //private void NotifyPropertyChanged(string propertyName)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}

        protected override void OnDisappearing() {
            validZoneTimer.Stop();
            validZoneTimer.Enabled = false;
            base.OnDisappearing();
        }
    }
}