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
        private readonly UdpFileClient _udpFileClient;
        private readonly FileDetails _fileDetails;
        private readonly string _remoteIp;

        private IPAddress _ipAddress;
        private FileStream _fileStream;

        public FileSender(string remoteIp)
        {
            _udpFileClient = new UdpFileClient();
            _fileDetails = new FileDetails();
            _remoteIp = remoteIp;
        }

        public int SendFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                SendFileDetails();
            }
            else
            {
                return -1;
            }

            return 0;
        }

        private int SendFileDetails()
        {
            try 
	        {	        
		        _ipAddress = IPAddress.Parse(_remoteIp);
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine(e.Message);
                return -1;
	        }

            _fileDetails.FileLength = _fileStream.Length;
            _fileDetails.FileName = _fileStream.Name;

            XmlSerializer fileDetailsSerializer = new XmlSerializer(typeof(FileDetails));

            //MemoryStream
            return 0;
        }
    }
}
