using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Diagnostics;

namespace AspectLauncher
{
    public class TcpServer
    {
        public void ReceiveCallback(IAsyncResult AsyncCall)
        {            
            UTF8Encoding encoding = new UTF8Encoding();            

            // The original listening socket is returned in the AsyncCall, we need to call "EndAccept" to
            // receive the client socket which we can use to send and receive data.
            Socket listener = (Socket)AsyncCall.AsyncState;
            Socket client = listener.EndAccept(AsyncCall);

            try
            {
                byte[] buffer = new byte[4096];
                int bufferCount = client.Receive(buffer);

                string[] postHeaders = Regex.Split(encoding.GetString(buffer, 0, bufferCount), "\r\n");
                string[] getParam = Regex.Split(postHeaders.First(), " ");
                string requestString = getParam[1];

                string[] param = Regex.Split(requestString.Substring(requestString.IndexOf("?") + 1), "&");

                Dictionary<string, string> paramList = new Dictionary<string, string>();
                foreach (var p in param)
                {
                    string[] keyvalue = p.Split('=');
                    paramList.Add(keyvalue.First(), keyvalue.Last());
                }

                string app = paramList["app"];
                Guid userID = new Guid(paramList["userID"]);
                Guid _dictNomen = new Guid(paramList["_dictNomen"]);
                string callback = paramList["callback"];
                string userSecretKey = paramList["secretKey"];

                string realSecretKey = MD5Hash(String.Format("{0}{1}{2}", app.Replace(@"\", @"\\"), userID, _dictNomen));                

                if (realSecretKey == userSecretKey)
                {
                    Process.Start(app, String.Format("{0} {1}", userID, _dictNomen));
                }

                string response = String.Format("{0}({{ }})", callback);
                client.Send(encoding.GetBytes(response));
            }
            catch { }

            client.Close();
            // At the end of the connection, we need to tell the OS that we can receive another call
            listener.BeginAccept(new AsyncCallback(ReceiveCallback), listener);
        }

        private string MD5Hash(string instr)
        {
            string strHash = string.Empty;

            foreach (byte b in new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(instr)))
            {
                strHash += b.ToString();
            }
            return strHash;
        }

        private Socket _listenSocket = null;

        private void StartListen()
        {            
            // Define the kind of socket we want: Internet, Stream, TCP
            _listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Define the address we want to claim: the first IP address found earlier, at port 2200
            IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2200);

            // Bind the socket to the end point
            _listenSocket.Bind(ipEndpoint);

            // Start listening, only allow 1 connection to queue at the same time
            _listenSocket.Listen(1);
            _listenSocket.BeginAccept(new AsyncCallback(ReceiveCallback), _listenSocket);

            // Start being important while the world rotates
            while (true)
            {                
                Thread.Sleep(5000);
            }
        }

        private Thread _listenThread = null;

        public void Start()
        {
            lock (this)
            {
                if (_listenThread == null)
                {
                    _listenThread = new Thread(new ThreadStart(StartListen));
                    _listenThread.Start();
                }
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (_listenThread != null)
                {
                    _listenThread.Abort();
                    _listenThread.Join();
                    _listenThread = null;
                }
            }
        }
    }
}
