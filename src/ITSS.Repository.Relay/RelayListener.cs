using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Azure.Relay;
using ITSS.Repository.Relay.Interfaces;

namespace ITSS.Repository.Relay
{
    public class RelayListener : IRelayListener
    {
        private HybridConnectionListener _listener; 
        public RelayListener(RelayParam relayParam)
        {
            if (relayParam == null) 
                throw new ArgumentNullException(nameof(relayParam));

            InitializeHybridConnection(relayParam);
        }

        public void Start(IListenerRequestHandler listenerRequestHandler)
        {
            if( listenerRequestHandler == null)
                throw new ArgumentNullException(nameof(listenerRequestHandler));

            byte[] byteRequest;
            // Subscribe to the status events.
            _listener.Connecting += (o, e) => { Console.WriteLine("Connecting"); };
            _listener.Offline += (o, e) => { Console.WriteLine("Offline"); };
            _listener.Online += (o, e) => { Console.WriteLine("Online"); };

            // Provide an HTTP request handler
            _listener.RequestHandler = (context) =>
            {
                // Do something with context.Request.Url, HttpMethod, Headers, InputStream...
                context.Response.StatusCode = HttpStatusCode.OK;
                context.Response.StatusDescription = "OK";

                byteRequest = new byte[context.Request.InputStream.Length];
                context.Request.InputStream.ReadAsync(byteRequest, 0, (int)context.Request.InputStream.Length);

                string request = Encoding.ASCII.GetString(byteRequest);
                Console.WriteLine("We recieved the next request message => {0}", request);

                var outputMessage = listenerRequestHandler.Handle(request);
                WriteInOutputStream(context, outputMessage);

                // The context MUST be closed here
                context.Response.Close();
            };

            _listener.OpenAsync().Wait();
            Console.WriteLine("Server listening");
        }

        public void Stop()
        {
            _listener?.CloseAsync().Wait();
        }

        private void InitializeHybridConnection(RelayParam relayParam)
        {
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(relayParam.AzureRelayKeyName, relayParam.AzureRelayKey);
            _listener = new HybridConnectionListener(new Uri(string.Format("sb://{0}/{1}", relayParam.AzureRelayNamespace, relayParam.AzureRelayConnectionName)), tokenProvider);
        }

        private void WriteInOutputStream(RelayedHttpListenerContext context, string outputMessage)
        {
            using (var sw = new StreamWriter(context.Response.OutputStream))
            {
                sw.Write(outputMessage);
            }
        }
    }
}
