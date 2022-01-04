using System;  
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  
  
// State object for reading client data asynchronously  
public class StateObject {  
    // Client  socket.  
    public Socket workSocket = null;  
    // Size of receive buffer.  
    public const int BufferSize = 2048;  
    // Receive buffer.  
    public byte[] buffer = new byte[BufferSize];  
// Received data string.  
    public StringBuilder sb = new StringBuilder();    
}  
  
public class AsynchronousSocketListener {  
    // Thread signal.  
    public static ManualResetEvent allDone = new ManualResetEvent(false);  
  
    public AsynchronousSocketListener() {  
    }  
  
    public static void StartListening() {  
        // Establish the local endpoint for the socket.  
        // The DNS name of the computer  
        // running the listener is "host.contoso.com".  
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 10000);  
  
        // Create a TCP/IP socket.  
        Socket listener = new Socket(IPAddress.Any.AddressFamily,  
            SocketType.Stream, ProtocolType.Tcp );  
  
        // Bind the socket to the local endpoint and listen for incoming connections.  
        try {  
            listener.Bind(localEndPoint);  
            listener.Listen(100);  
  
            while (true) {                
                // Set the event to nonsignaled state.  
                allDone.Reset();  
  
                // Start an asynchronous socket to listen for connections.  
                listener.BeginAccept(   
                    new AsyncCallback(AcceptCallback),  
                    listener );  
  
                // Wait until a connection is made before continuing.  
                allDone.WaitOne();                
            }  
  
        } 
        catch (ThreadAbortException abortException)
        {
            SystemHealthAPI.Program.system.close();
        }
        catch (Exception e)
        {

        }


    }  
  
    public static void AcceptCallback(IAsyncResult ar) {  
        // Signal the main thread to continue.  
        allDone.Set();  
  
        // Get the socket that handles the client request.  
        Socket listener = (Socket) ar.AsyncState;  
        Socket handler = listener.EndAccept(ar);  
  
        // Create the state object.  
        StateObject state = new StateObject();  
        state.workSocket = handler;  
        handler.BeginReceive( state.buffer, 0, StateObject.BufferSize, 0,  
            new AsyncCallback(ReadCallback), state);  
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        String content = String.Empty;

        // Retrieve the state object and the handler socket  
        // from the asynchronous state object.  
        StateObject state = (StateObject)ar.AsyncState;
        Socket handler = state.workSocket;

        // Read data from the client socket.   
        int bytesRead = handler.EndReceive(ar);



        if (bytesRead > 0)
        {
            state.sb.Append(Encoding.ASCII.GetString( state.buffer, 0, bytesRead));
            var data = state.sb.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            //GET /?callback=asfj9aajs9wf HTTP/1.1
            foreach (var item in data)
            {
                if (item.Contains("Referer:"))
                    continue;
                if (item.Contains("callback="))
                {
                    int start = item.IndexOf("callback=")+9;
                    int check = item.IndexOf("&");

                    int end = item.IndexOf(" HTTP");
                    end = (check==-1) ? end : check;
                    content = item.Substring(start, end-start);
                    break;
                }
            } 
            
            Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",content.Length, content);
            // Echo the data back to the client.  
            Send(handler, "HTTP/1.1 200 OK\r\nContent-Type: application/javascript\r\n\r\n" + SystemHealthAPI.Program.system.ToString(content,false));
        }
    }  
  
    private static void Send(Socket handler, String data) {
        // Convert the string data to byte data using ASCII encoding.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.  
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }  
  
    private static void SendCallback(IAsyncResult ar) {
        try
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to client.", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }  
}  




//Program.system.ToString()+ "<EOF>"