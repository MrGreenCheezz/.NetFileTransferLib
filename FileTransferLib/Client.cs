using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferLib
{
    public class FTClient
    {
        private string serverAdress;
        private int serverPort;
        private TcpClient client;
        private NetworkStream stream;
        private bool isReadingStream = false;

        public int Port { get => serverPort; }
        public string ServerAdress { get => serverAdress; }
        public TcpClient Client { get => client; }

        public FTClient(string serverAdress = "", int serverPort = 0)
        {
            this.serverAdress = serverAdress;
            this.serverPort = serverPort;
            client = new TcpClient();
            Console.WriteLine("Client created");
        }

        public void Connect()
        {
            try
            {
                client.Connect(serverAdress, serverPort);
                stream = client.GetStream();
                Console.WriteLine("Client connected!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void ConnectAsync()
        {
            try
            {
                await client.ConnectAsync(serverAdress, serverPort);
                stream = client.GetStream();
                Console.WriteLine("Client connected!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task StartFileRecieve(string fileNewPath)
        {
            isReadingStream = true;
            Console.WriteLine("Started file receive!");
            while (isReadingStream)
            {

                var fileNameLengthBuffer = new byte[4];
                await stream.ReadAsync(fileNameLengthBuffer, 0, fileNameLengthBuffer.Length);
                int fileNameLength = BitConverter.ToInt32(fileNameLengthBuffer, 0);

                var fileNameBuffer = new byte[fileNameLength];
                await stream.ReadAsync(fileNameBuffer, 0, fileNameBuffer.Length);
                string fileName = Encoding.UTF8.GetString(fileNameBuffer);
                int Counter = 0;
                using (var output = File.Create(Path.Combine(fileNewPath, fileName)))
                {
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        output.Write(buffer, 0, bytesRead);
                        Counter++;
                        Console.WriteLine($"Donwloaded part {Counter}");
                    }
                    output.Close();
                }
                stream.Close();
                isReadingStream = false;
                
                return;
            }
        }


        ~FTClient()
        {
            client.Close();
        }

    }
}
