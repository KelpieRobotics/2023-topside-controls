using System.Net;
using System.Net.Sockets;
using System.Text;

using var client = new TcpClient();

IPAddress hostname = IPAddress.Parse("10.192.77.99");
client.Connect(hostname, 9999);

using NetworkStream stream = client.GetStream();

while (true)
{
    var message = Console.ReadLine();

    if (!String.IsNullOrEmpty(message))
    {
        message += "\r\n";
        byte[] bytes = Encoding.ASCII.GetBytes(message);
        stream.Write(bytes, 0, bytes.Length);
    }
    else if (message == "quit")
    {
        client.Close();
        break;
    }

    if (stream.DataAvailable)
    {

        byte[] readBuffer = new byte[1024];
        StringBuilder completeMessage = new StringBuilder();
        stream.ReadTimeout = 250;
        try
        { // try while the length of the read !=0
            int numberOfBytesRead;
            while ((numberOfBytesRead = stream.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                Console.WriteLine(stream.DataAvailable);
                completeMessage.Append(Encoding.UTF8.GetString(readBuffer, 0, numberOfBytesRead));
                Console.WriteLine(completeMessage);
            }
        }
        catch(IOException)
        {
            Console.WriteLine("Transmission Finished...");
        }
    }
}