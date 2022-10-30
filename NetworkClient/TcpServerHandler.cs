using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Text;

namespace NetworkClient;

public class TcpServerHandler
{
    private int port; // port the server will be listening in on
    //private IPAddress localHost = IPAddress.Parse("127.0.0.1"); // gets the local ip of the server

    private bool serverIsUp = false;
    private bool clientIsConnected = false;

    private TcpListener server = null;
    private TcpClient client = null; // object that will keep track of our client (the pi)
    private NetworkStream stream = null;

    public TcpServerHandler(int prt)
    {
        port = prt;
    }

    public void StartServer()
    {
        // Make sure there isnt already a server running
        if (!serverIsUp)
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                serverIsUp = true;
                Console.WriteLine("Listening on port " + port);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to start server!");
                Console.WriteLine(e);
            }
        }
        else
        {
            Console.WriteLine("Server is already running!");
        }
    }

    public void StopServer()
    {
        // Make sure there is a server running before you try to stop said server
        if (serverIsUp)
        {
            // But before that, close the connection with the client
            if (clientIsConnected)
            {
                client.Close();
            }
            server.Stop();
            serverIsUp = false;
            Console.WriteLine("Closed server");
        }
        else
        {
            Console.WriteLine("No server running to stop!");
        }

    }

    public void Connect()
    {   
        // Make sure the server is up before you attempt to accept a client
        if (serverIsUp)
        {
            // If there currently is no connected client, proceed with connecting them
            if (!clientIsConnected) 
            {
                Console.WriteLine("Looking for client...");
                client = server.AcceptTcpClient();
                clientIsConnected = true;
            }
            stream = client.GetStream();
            // Confirm connection and print clients IP
            Console.WriteLine("Connected to " + client.Client.RemoteEndPoint);
        } else
        {
            Console.WriteLine("Must open server before you can accept connections!");
        }
        
    }

    public void Send(String message)
    {
        if (clientIsConnected)
        {
            byte[] sendBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(sendBytes, 0, sendBytes.Length);
        }
    }

    public string Receive()
    {
        string message = "";
        byte[] receiveByte = new byte[1024];
        if (clientIsConnected)
        {
            int i;
            while ((i = stream.Read(receiveByte, 0, receiveByte.Length)) != 0)
            {
               message = Encoding.ASCII.GetString(receiveByte, 0, i);
                return message;
            }
            return "";
        }
        else
        {
            return "No client to receive from";
        }
    }
}


