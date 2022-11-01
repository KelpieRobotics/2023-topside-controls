using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;
using System.Net.NetworkInformation;
using System.ComponentModel;

namespace NetworkClient;


public class TcpServerHandler
{
    private int port; // port the server will be listening in on
    //private IPAddress localHost = IPAddress.Parse("127.0.0.1"); // gets the local ip of the server

    private bool serverIsUp = false;
    private bool clientIsConnected = false;

    private TcpListener server;
    private TcpClient client; // object that will keep track of our client (the pi)
    private NetworkStream stream;
    private Ping pinger = new Ping();
    private IPEndPoint clientHost;

    public List<string> messagesReceived = new();

    /// <summary>
    /// Instantiate a new server with port <c>prt</c>
    /// </summary>
    /// <param name="prt"></param>
    public TcpServerHandler(int prt)
    {
        port = prt;
    }

    /// <summary>
    /// Start the TCP server if one is not already running
    /// </summary>
    public void StartServer()
    {
        // Make sure there isnt already a server running
        if (serverIsUp)
        {
            Console.WriteLine("Server is already running!");
            return;
        }

        try
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            serverIsUp = true;
            Console.WriteLine("Listening on port " + port);

            Thread receiveThread = new Thread(Receive);

            receiveThread.Start();
        }
        catch (SocketException e)
        {
            Console.WriteLine("Failed to start server!");
            Console.WriteLine(e);
        }
    }

    public void StopServer()
    {
        // Make sure there is a server running before you try to stop said server
        if (!serverIsUp)
        {
            Console.WriteLine("No server running to stop!");
            return;
        }
        // But before that, close the connection with the client
        if (clientIsConnected)
        {
            client.Dispose();
        }
        server.Stop();
        serverIsUp = false;
        Console.WriteLine("Closed server");
    }

    /// <summary>
    /// Accept any incoming clients, must be called AFTER <c>TcpServerHandler.StartServer()</c>
    /// </summary>
    public void Connect()
    {

        // Make sure the server is up before you attempt to accept a client
        if (!serverIsUp)
        {
            Console.WriteLine("Must open server before you can accept connections!");
            return;
        }

        Console.WriteLine("Looking for client...");
        client = server.AcceptTcpClient();
        clientIsConnected = true;
        stream = client.GetStream();
        clientHost = (IPEndPoint)client.Client.LocalEndPoint;
        Console.WriteLine("Connected to " + clientHost.Address);
        //Thread.Sleep(3000);

    }

    public void Send(String message)
    {
        if (!clientIsConnected || !serverIsUp)
        {
            Console.WriteLine("No client connected!");
            return;
        }
        byte[] sendBytes = Encoding.ASCII.GetBytes(message);
        stream.Write(sendBytes, 0, sendBytes.Length);
    }

    /// <summary>
    /// Receive any pending bytes from the client
    /// </summary>
    /// <returns>A string with the message from the client</returns>
    public void Receive()
    {
        if (!serverIsUp)
        {
            Console.WriteLine("There is no server running!");
            return;
        }
        string message;
        byte[] receiveByte = new byte[1024];
        while (true)
        {
            if (!clientIsConnected)
            {
                Connect();
            }
            int i;
            while ((i = stream.Read(receiveByte, 0, receiveByte.Length)) != 0)
            {
                message = Encoding.ASCII.GetString(receiveByte, 0, i);
                message = message.Trim();
                //return message;
                messagesReceived.Add(message);
                Console.WriteLine($"Message from Client #{messagesReceived.Count}: {message}");
            }
            clientIsConnected = client.Client.Poll(0, SelectMode.SelectError);

            if (clientIsConnected) continue;
            
            Console.WriteLine("Lost connection to client!");
            stream.Dispose();
            
        }
    }
}


