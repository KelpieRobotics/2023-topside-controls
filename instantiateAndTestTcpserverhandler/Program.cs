using NetworkClient;
using System;


TcpServerHandler tcpServer = new TcpServerHandler(9000);

tcpServer.StartServer();

while (true)
{
    var message = Console.ReadLine();
    message += "\n";
    double time1 = (double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    tcpServer.Send(message);
    double time2 = (double)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    Console.WriteLine("Time taken to send: " + ((time2 - time1)/2) + " seconds");
}

//tcpServer.Send("hello");
//tcpServer.Send("Closing server now");
//Thread.Sleep(1000);
//tcpServer.StopServer();


