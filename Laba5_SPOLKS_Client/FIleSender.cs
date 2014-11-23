﻿using System;
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
        private IPEndPoint _ipEndPoint;
        private FileStream _fileStream;

        public FileSender()
        {
            _udpFileClient = new UdpFileClient();
            _fileDetails = new FileDetails();
            //_remoteIp = remoteIp;
        }

        public int SendFile(string filePath, string ipAddress)
        {
            try
            {
                _ipAddress = IPAddress.Parse(ipAddress);
                _ipEndPoint = new IPEndPoint(_ipAddress, 11000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);               
                return -1;
            }

            if (File.Exists(filePath))
            {
              _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
              var result = SendFileDetails();

              //SendFileData(filePath);

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
            _fileDetails.FileName = _fileStream.Name;

            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer fileDetailsSerializer = new XmlSerializer(typeof(FileDetails));
            fileDetailsSerializer.Serialize(memoryStream, _fileDetails);

            memoryStream.Position = 0;
            var fileDetailsArray = new byte[memoryStream.Length];
            var readBytesAmount = memoryStream.Read(fileDetailsArray, 0, fileDetailsArray.Length);

            try
            {
                _udpFileClient.Connect(_ipEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }

            if (_udpFileClient.ActiveRemoteHost)
            {
                var sendBytes = _udpFileClient.Send(fileDetailsArray, fileDetailsArray.Length); 
            }
            else
            {
                _udpFileClient.Close();
                return -1;
            }

            return 0;
        }

        private int SendFileData(string filePath)
        {
            var buffer = new byte[_fileStream.Length];
            var amountReadBytes = _fileStream.Read(buffer, 0, buffer.Length);

            int result = 0;

            try
            {
                _udpFileClient.Send(buffer, buffer.Length, _ipEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = -1;
            }
            finally
            {
                _fileStream.Close();
                _udpFileClient.Close();
            }

            return result;
        }
    }
}
