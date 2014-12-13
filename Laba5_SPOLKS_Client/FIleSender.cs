using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;

namespace Laba5_SPOLKS_Client
{
    public class FileSender
    {
        private const int Size = 8192;
        private const int LocalPort = 5000;
        private const int RemotePort = 11000;

        private readonly UdpFileClient _udpFileSender;
        private readonly FileDetails _fileDetails;

        private int connectionFlag = 0;

        private IPAddress _ipAddress;
        private IPEndPoint _ipEndPoint;
        private FileStream _fileStream;

        public FileSender()
        {
            _udpFileSender = new UdpFileClient(LocalPort);
            _fileDetails = new FileDetails();
        }

        void InitializeUdpClients()
        {
            _udpFileSender.Client.SendTimeout = _udpFileSender.Client.ReceiveTimeout = 10000;
        }

        public int SendFile(string filePath, string ipAddress)
        {
            try
            {
                _ipAddress = IPAddress.Parse(ipAddress);
                _ipEndPoint = new IPEndPoint(_ipAddress, RemotePort);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            if (File.Exists(filePath))
            {
                _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                InitializeUdpClients();

                var result = SendFileDetails();
                SendFileData(filePath);

                if (result == -1)
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }

            return 0;
        }

        private int SendFileDetails()
        {
            _fileDetails.FileLength = _fileStream.Length;
            _fileDetails.FileName = _fileStream.Name.Remove(0, _fileStream.Name.LastIndexOf('\\') + 1);

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer fileDetailsSerializer = new XmlSerializer(typeof(FileDetails));
            fileDetailsSerializer.Serialize(memoryStream, _fileDetails);

            memoryStream.Position = 0;
            var fileDetailsArray = new byte[Size];
            var readBytesAmount = memoryStream.Read(fileDetailsArray, 0, Convert.ToInt32(memoryStream.Length));

            memoryStream.Dispose();

            try
            {
                _udpFileSender.Connect(_ipEndPoint);

                if (_udpFileSender.ActiveRemoteHost)
                {
                    var sendBytes = _udpFileSender.Send(fileDetailsArray, fileDetailsArray.Length);
                }
                else
                {
                    _udpFileSender.Close();
                    return -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                _udpFileSender.Close();
                return -1;
            }

            return 0;
        }

        private int SendFileData(string filePath)
        {
            int result = 0;
            int filePointer = 0;
            IPEndPoint remoteIpEndPoint = null;

            var buffer = new byte[Size];

            try
            {
                _udpFileSender.Connect(_ipEndPoint);

                if (_udpFileSender.ActiveRemoteHost)
                {
                    for (_fileStream.Position = 0; _fileStream.Position < _fileDetails.FileLength; )
                    {
                        int sendBytesAmount = 0;

                        buffer.Initialize();
                        var amountReadBytes = _fileStream.Read(buffer, 0, buffer.Length);

                        try
                        {
                            if (amountReadBytes < buffer.Length)
                            {
                                var lastFrame = new byte[amountReadBytes];
                                Array.Copy(buffer, lastFrame, amountReadBytes);

                                sendBytesAmount = _udpFileSender.Send(lastFrame, lastFrame.Length);
                            }
                            else
                            {
                                sendBytesAmount = _udpFileSender.Send(buffer, buffer.Length);
                            }

                            filePointer += sendBytesAmount;
                            Console.WriteLine(filePointer);

                            var syncSignal = _udpFileSender.Receive(ref remoteIpEndPoint);
                            var syncString = Encoding.UTF8.GetString(syncSignal);
                            Console.WriteLine(syncString);
                        }
                        catch (SocketException e)
                        {
                            if (e.SocketErrorCode == SocketError.TimedOut && connectionFlag < 3)
                            {
                                _udpFileSender.Connect(_ipEndPoint);

                                if (_udpFileSender.ActiveRemoteHost)
                                {
                                    if (filePointer < _fileStream.Position)
                                    {
                                        _fileStream.Position -= buffer.Length;
                                    }

                                    connectionFlag = 0;
                                }
                                else
                                {
                                    connectionFlag++;
                                }

                                continue;
                            }
                            else
                            {
                                _fileStream.Close();
                                _udpFileSender.Close();
                                return -1;
                            }
                        }
                    } 
                }
                else
                {
                    _udpFileSender.Close();
                    return -1;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = -1;
            }
            finally
            {
                _fileStream.Close();
                _udpFileSender.Close();
            }

            return result;
        }
    }
}
