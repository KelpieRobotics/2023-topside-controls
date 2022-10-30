using NetworkClient;


TcpServerHandler tcpServer = new TcpServerHandler(9000);

tcpServer.StartServer();
tcpServer.Connect();
var message = tcpServer.Receive();
Console.WriteLine(message);
tcpServer.Send("Closing server now");
Thread.Sleep(1000);
tcpServer.StopServer();


