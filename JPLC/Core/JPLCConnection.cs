using Snap7;

namespace JPLC
{
    // Singleton
    public class JPLCConnection
    {       
        #region [Private]
        private static JPLCConnection instance;
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
        public bool Connected { get; private set; }
        #endregion

        #region [Methods]
        public void Connect()
        {            
            int result = S7Api.ConnectTo(IPAddress, Rack, Slot);
            if(result == 0)
            {
                Connected = false;
            }
            else
            {
                Connected = true;
            }
          
        }
        public void Disconnect()
        {
            int result = S7Api.Disconnect();
            if (result == 0)
            {
                Connected = false;
            }
            else
            {
                Connected = true;
            }
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
