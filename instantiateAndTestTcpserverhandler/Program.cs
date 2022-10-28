using NetworkClient;

int port = 9000;

TcpServerHandler tcpServer = new TcpServerHandler(port);

tcpServer.StartServer();