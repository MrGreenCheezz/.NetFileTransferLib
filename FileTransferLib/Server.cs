using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferLib
{
    public class FTServer
    {
        private string serverAdress;
        private int serverPort;
        private TcpListener listener;
        private NetworkStream stream;
        private bool isReadingStream = false;
        private bool isWritingStream = false;
        private bool isOpenForClients = false;

        public List<TcpClient> clients = new List<TcpClient>();

        public int Port { get => serverPort; }
        public string ServerAdress { get => serverAdress; }
        public TcpListener Listener { get => listener; }


        public FTServer(string serverAdress,int serverPort)
        {
            this.serverAdress = serverAdress;
            this.serverPort = serverPort;
            try
            {
                var adress = IPAddress.Parse(serverAdress);
                listener = new TcpListener(adress, serverPort);
                listener.Start();
                Console.WriteLine("Server started!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        public async Task StartAcceptingClientsConnection()
        {
            Console.WriteLine("Server started to accept connections!");
            isOpenForClients = true;
            try
            {
                while (isOpenForClients)
                {
                    var tmp = await listener.AcceptTcpClientAsync();
                    clients.Add( tmp);                 
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        
        }

        public async Task SendFileToClient(TcpClient client, string fileName, string filePath)
        {
            Console.WriteLine($"{fileName} {filePath} are sending!");
            var tmpStream = client.GetStream();

            byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
            byte[] fileNameLength = BitConverter.GetBytes(fileNameBytes.Length);
            await tmpStream.WriteAsync(fileNameLength, 0, fileNameLength.Length);
            await tmpStream.WriteAsync(fileNameBytes, 0, fileNameBytes.Length);

            string fullPath = Path.Combine(filePath, fileName);
            using (FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = await fs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await tmpStream.WriteAsync(buffer, 0, bytesRead);
                }
            }
        }



    }
}
