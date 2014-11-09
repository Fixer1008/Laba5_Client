using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Laba5_SPOLKS_Client
{
    public class UdpFileClient : UdpClient
    {
        public bool ActiveRemoteHost
        {
            get { return Active; }
        }

        public Socket FileClientSocket 
        {
            get { return Client; }
        }
    }
}
