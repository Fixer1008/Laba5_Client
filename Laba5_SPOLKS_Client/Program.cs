using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Laba5_SPOLKS_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            FileSender fileSender = new FileSender();
            const string remoteIp = "192.168.0.104";

            //var addresses = Dns.GetHostAddresses("ALEX-NOTE");
            //string remoteIp = addresses[1].ToString();

            Console.WriteLine(remoteIp);
            
            try
            {
                Console.Write("File name: ");
                string filePath = Console.ReadLine();
                var result = fileSender.SendFile(filePath, remoteIp);

                if (result == -1)
                {
                    Console.WriteLine("Error!");
                }
                else
                {
                    Console.WriteLine("Success!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}
