using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using MobileClient;
using SerializablePackages;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Mobile_Application
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReadWritePage : ContentPage
    {
        MobileClientManager client;
        Packages.ResourcePackage RAMPackage;
        System.Timers.Timer validZoneTimer;
        string UserInput { get; set; }
        public string filepath { get; set; }
        public bool EndWriting { get; set; }
        private Thread writeThread;

        public ReadWritePage(MobileClientManager paramClient)
        {

            InitializeComponent();
            client = paramClient;
            client.SetTextBox(bodyText);
            RAMPackage = new Packages.ResourcePackage();
            filepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            client.Builder.RaiseWriteToFileEvent += HandleWriteToFile;
            client.RequestResources(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), paramClient.UserCryptoInfo);
            StartValidationTimer();
        }

        private async Task StartValidationTimer()
        {
            validZoneTimer = new System.Timers.Timer(5000);
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
                    accessGranted = client.CheckOngoingAuthorization(client.UserCryptoInfo);
                    if (!accessGranted)
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await DisplayAlert("Access expired!", "Access has been revoked due to being in an invalid zone! " +
                                "Please check to make sure you are still accessing from a valid time and location if you wish to continue access.", "Ok");
                            writeThread.Abort();
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
                        writeThread.Abort();
                        await Navigation.PopAsync();
                    });
                    validZoneTimer.Stop();
                    validZoneTimer.Enabled = false;
                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }

        private void userMessage(string title, string message, string button)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert(title, message, button);
            });
        }

        private void FileWriteThread()
        {
            System.Diagnostics.Debug.WriteLine("File write thread created.");
            EndWriting = false;
            try
            {
                using (StreamWriter sWriter = File.AppendText(filepath))
                {
                    while (true)
                    {
                        if (EndWriting)
                        {
                            sWriter.Close();
                            UserInput = null;
                            break;
                        }
                        else if (UserInput != null)
                        {
                            sWriter.WriteLine(UserInput);
                            userMessage("New text added to file!", "Entered: " + UserInput, "Ok");
                            UserInput = null;
                        }
                    }
                }

                
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }

        private async void HandleWriteToFile(object sender, WriteToFileEventArgs writeFile)
        {
            filepath = writeFile.filepath;
            ThreadStart strtThread = new ThreadStart(FileWriteThread);
            writeThread = new Thread(strtThread);
            writeThread.IsBackground = true;
            writeThread.Start();
        }

        private void submitEditButton_Clicked(object sender, EventArgs e)
        {
            UserInput = resourceAddition.Text.Trim();
            resourceAddition.Text = "";
        }

        private void finalizeEditButton_Clicked(object sender, EventArgs e)
        {
            validZoneTimer.Stop();
            validZoneTimer.Enabled = false;
            Packages serializer = new Packages();
            try
            {
                var dataSent = false;
                EndWriting = true;
                Packages.UserInfoPackage userInfo = client.rBuilder.userInfo;
                do
                {
                    if (!writeThread.IsAlive)
                    {
                        using (FileStream fstream = File.Open(filepath, FileMode.Open))
                        {
                            fstream.Position = 0;
                            byte[] fileData = new byte[fstream.Length];
                            for (int i = 0; i < fileData.Length; i++)
                            {
                                fileData[i] = (byte)fstream.ReadByte();
                            }
                            fstream.Close();

                            userInfo.userFile = fileData;
                            userInfo.requestedActivity.ActivityID = Convert.ToInt32(Packages.Actions.UpdateFile);

                            if (userInfo.userFile == null)
                            {
                                DisplayAlert("Warning", "Empty package supplied to send.", "Ok");
                            }
                        }

                        AESEncryptor encryptor = new AESEncryptor(client.UserCryptoInfo.GetKey());
                        Packages.EncryptedPackage encrypted = encryptor.Encrypt(userInfo);
                        serializer.BinarySerializer(client.getClientStream(), encrypted);
                        DisplayAlert("Success", "File entry updated!", "Ok");
                        dataSent = true;
                        File.Delete(filepath);
                        client.Builder.RaiseWriteToFileEvent -= HandleWriteToFile;
                        Navigation.PopAsync();
                    }
                } while (dataSent == false);
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