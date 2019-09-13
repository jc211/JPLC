using Snap7;

namespace JPLC
{
    // Singleton
    public class JPLCConnection
    {       

        public string IPAddress { get; private set; } = "192.168.0.11";
        public int Rack { get; private set; } = 0;
        public int Slot { get; private set; } =  2;
        public S7Client S7Api { get; private set; }
        public string LastError { get; private set; } = "";


        public JPLCConnection(string ipAddress, int rack, int slot) {
            IPAddress = ipAddress;
            Rack = rack;
            Slot = slot;
            S7Api = new S7Client();      
        }

        public bool Connected { get { return S7Api.Connected(); } }

        public int Connect()
        {            
            int result = S7Api.ConnectTo(IPAddress, Rack, Slot);
            if(result != 0)
            {
                LastError = S7Api.ErrorText(result);
            }
          
            return result;
          
        }
        public int Disconnect()
        {
            int result = S7Api.Disconnect();
            if (result != 0)
            {
                LastError = S7Api.ErrorText(result);
            }
            return result;
        }
    }
}
