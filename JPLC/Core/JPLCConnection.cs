using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snap7;
using System.Diagnostics;
using NLog;
using Cinch;
using MEFedMVVM.Common;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace JPLC
{
    // Singleton
    public class JPLCConnection:ViewModelBase
    {       
        #region [Private]
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static JPLCConnection instance;
        #endregion

        #region [Events]
        private BehaviorSubject<ConnectedEventArgs> connectedEvent = new BehaviorSubject<ConnectedEventArgs>(new ConnectedEventArgs(false,false,"Initial"));
        public IObservable<ConnectedEventArgs> ConnectedEvent
        {
            get { return this.connectedEvent.AsObservable().DistinctUntilChanged(); }
        }
        #endregion

        #region [Public]
        public static string IPAddress = "192.168.0.11";
        public static int Rack = 0;
        public static int Slot = 2;

        public S7Client S7Api;
        #endregion

        #region [Constructors]
        private JPLCConnection() {
            S7Api = new S7Client();      
        }

        public static JPLCConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JPLCConnection();
                    instance.S7Api = new S7Client();
                }
                return instance;
            }
        }
        public bool Connected { get { return connectedEvent.Value.IsConnected; } }
        #endregion

        #region [Methods]
        public void Connect()
        {
            logger.Info("Connecting to " + JPLCConnection.IPAddress + " Rack: " + JPLCConnection.Rack + " Slot: " +JPLCConnection.Slot );
            
            int result = S7Api.ConnectTo(JPLCConnection.IPAddress, JPLCConnection.Rack, JPLCConnection.Slot);
            if(result == 0)
            {
                connectedEvent.OnNext(new ConnectedEventArgs(true, false, S7Api.ErrorText(result)));
                NotifyPropertyChanged("Connected");
            }
            else
            {
                connectedEvent.OnNext(new ConnectedEventArgs(false, true, S7Api.ErrorText(result)));
                NotifyPropertyChanged("Connected");
            }
            logger.Info(S7Api.ErrorText(result));
          
        }
        public void Disconnect()
        {
            logger.Info("Disconnecting");
            int result = S7Api.Disconnect();
            if (result == 0)
            {
                connectedEvent.OnNext(new ConnectedEventArgs(false, false, S7Api.ErrorText(result)));
                NotifyPropertyChanged("Connected");
            }
            else
            {
                connectedEvent.OnNext(new ConnectedEventArgs(true, true, S7Api.ErrorText(result)));
                NotifyPropertyChanged("Connected");
            }
            logger.Info(S7Api.ErrorText(result));    
        }
        #endregion
    }
    public class ConnectedEventArgs
    {
        public bool IsConnected { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool HasError { get; private set; }
        public ConnectedEventArgs(bool isConnected, bool hasError, string errorMessage)
        {
            HasError = hasError;
            IsConnected = isConnected;
            ErrorMessage = errorMessage;
        }
    }
}
