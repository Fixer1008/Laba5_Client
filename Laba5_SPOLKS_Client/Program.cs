using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Laba5_SPOLKS_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            const string EndMessage = "<End>";

            UdpClient sendUdpClient = new UdpClient(5001);

            IPAddress ipAddress = IPAddress.Parse("192.168.0.103");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 11000);

            sendUdpClient.Connect(ipEndPoint);

            try
            {
                while (true)
                {
                    Console.Write("Clent: ");
                    var sendMessage = Console.ReadLine();
                    var sentBytes = sendUdpClient.Send(Encoding.UTF8.GetBytes(sendMessage), sendMessage.Length);

                    if (sendMessage == EndMessage)
                    {
                        break;
                    }

                    var receiveBuffer = sendUdpClient.Receive(ref ipEndPoint);
                    Console.WriteLine("Server: {0}", Encoding.UTF8.GetString(receiveBuffer));

                    if (Encoding.UTF8.GetString(receiveBuffer) == EndMessage)
                    {
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sendUdpClient.Close();
            }

            Console.ReadLine();
        }
    }
}
