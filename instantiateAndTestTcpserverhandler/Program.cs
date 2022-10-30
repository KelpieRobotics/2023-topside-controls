using NetworkClient;


TcpServerHandler tcpServer = new TcpServerHandler(9000);

tcpServer.StartServer();
tcpServer.Connect();
while (true)
{
    var message = tcpServer.Receive();
    Console.WriteLine(message);
}
