﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SerializablePackages;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MobileClient
{
    public class MobileClientManager
    {
  
        private ZoneManager zoneManager;
        public RequestBuilder rBuilder;
        private GPSClient gpsClient;
        Packages serializer;
        Packages.STZone currentZone = null;
        private Packages.UserInfoPackage userInfo = new Packages.UserInfoPackage();

        public RequestBuilder Builder
        {
            get { return rBuilder; }
            set { rBuilder = value; }
        }

        public Packages.UserInfoPackage UserInfo
        {
            get { return userInfo; }
            set { userInfo = value; }
        }
        Label appConsole;
        delegate void SendTextCallback(string text);

        public EventHandler<WriteToFileEventArgs> RaiseWriteToFileEvent;
        public string UserInput { get; set; }

        public MobileClientManager()
        {
            zoneManager = new ZoneManager();
            rBuilder = new RequestBuilder(zoneManager);
            gpsClient = new GPSClient();
            serializer = new Packages();
        }

        public IPAddress ServerIPAddress
        {
            get
            {
                return rBuilder.ServerIPAddress;
            }
        }

        public int ServerPort
        {
            get
            {
                return rBuilder.ServerPort;
            }
        }

        public async Task SetServerIPAddress()
        {
            await rBuilder.SetServerIPAddress();
        }

        public bool SetPortNumber(string ServerPort)
        {
            return rBuilder.SetPortNumber(ServerPort);
        }

        public NetworkStream getClientStream()
        {
            return rBuilder.UserClient.GetStream();
        }

        public bool isConnected()
        {
            try
            {
                if (rBuilder.UserClient.Connected)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                return false;
            }
        }


        public TcpClient getClient()
        {
            return rBuilder.UserClient;
        }

        public void SetTextBox(Label textBox)
        {
            rBuilder.SetTextBox(textBox);
        }

        //public void SetPdfReader(PdfDocument pdf)
        //{
        //    rBuilder.SetPdfReader(pdf);
        //}

        //probably depreciated
        public void SendText(string newText)
        {
            try
            {
                appConsole.Text = appConsole.Text + newText + Environment.NewLine;
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }

        }

        protected virtual async Task OnRaiseWriteToFileEvent(WriteToFileEventArgs writeFile)
        {
            EventHandler<WriteToFileEventArgs> handler = RaiseWriteToFileEvent;

            if (handler != null)
            {
                handler(this, writeFile);
            }
        }

        public void CloseAndDisconnect()
        {
            rBuilder.CloseAndDisconnect();
        }

        public async Task ConnectToServer()
        {
            await rBuilder.ConnectToServer();
        }

        public async Task RequestResources(string systemFilepath)
        {
            rBuilder.requestResourcesFromServer(rBuilder.UserClient.GetStream(), systemFilepath);
        }

        public async Task CredentialVerification(string username, string password)
        {
            rBuilder.CredentialVerification(username, password, rBuilder.UserClient.GetStream());
        }

        public bool credentialsAreCorrect()
        {
            return rBuilder.userInfo.correctCredentials;
        }

        public bool CheckIfValidRequest()
        {
            return rBuilder.CheckIfValidRequest();
        }

        public bool CheckOngoingAuthorization()
        {
            return rBuilder.CheckOngoingAuthorization();
        }

        public async Task ReceiveFormRequest(int activity, int resource, int role, int location, int timeIndex)
        {
            rBuilder.ReceiveFormRequest(activity, resource, role, location, timeIndex);
        }
    }
}