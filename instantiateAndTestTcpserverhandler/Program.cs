using NetworkClient;


TcpServerHandler tcpServer = new TcpServerHandler(9000);

tcpServer.StartServer();

while (true)
{
    var message = Console.ReadLine();
    message += "\n";
    tcpServer.Send(message);
}

//tcpServer.Send("hello");
//tcpServer.Send("Closing server now");
//Thread.Sleep(1000);
//tcpServer.StopServer();


