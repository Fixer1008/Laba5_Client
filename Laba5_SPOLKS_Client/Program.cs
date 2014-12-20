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
            try
            {
                Console.Write("Local port: ");
                string localPort = Console.ReadLine();

                Console.Write("File name: ");
                string filePath = Console.ReadLine();

                Console.Write("Server IP: ");
                string remoteIp = Console.ReadLine();

                FileSender fileSender = new FileSender();
                var result = fileSender.SendFile(filePath, remoteIp, localPort);

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
