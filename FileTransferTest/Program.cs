using FileTransferLib;
using System.Net.Sockets;

class Program
{
    static async Task Main(string[] args)
    {
        int type = 0;
        Console.WriteLine("Input type of app \n 1 for client. \n 2 for server.");
        type = int.Parse(Console.ReadLine());

        switch (type)
        {
            case 1:
                {
                    FTClient client = new FTClient("127.0.0.1", 25567);
                    client.Connect();
                    Console.ReadLine();
                    var taska = Task.Run(async () => await client.StartFileRecieve("C:\\Users\\Administrator\\Desktop\\FileTransferLib"));
                    Console.ReadLine();
                    break;
                }

            case 2:
                {
                    FTServer server = new FTServer("127.0.0.1", 25567);
                    var acceptingClientsTask = Task.Run(async () => await server.StartAcceptingClientsConnection());
                    Console.ReadLine();
                    await Task.Run(async () => await server.SendFileToClient(server.clients[0], "Server-Files-2.7.zip", "C:\\Users\\Administrator\\Desktop\\Send"));
                    Console.ReadLine();
                    break;
                }
        }

        return;

    }
}
