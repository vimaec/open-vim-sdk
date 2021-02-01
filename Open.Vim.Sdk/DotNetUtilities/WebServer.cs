using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace Vim.DotNetUtilities
{
    public class WebServer
    {
        public Action<string, IDictionary<string, string>, Stream> Callback;
        private HttpListener listener;
        private Thread listenerThread;

        public void Start(Action<string, IDictionary<string, string>, Stream> callback, string uri = "http://localhost:8080/")
        {
            Callback = callback;
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start();
            listenerThread = new Thread(StartListener);
            listenerThread.Start();
            Debug.WriteLine("Server Started");
        }

        public void Stop()
        {
            listenerThread.Abort();
        }

        private void StartListener()
        {
            while (true)
            {
                var result = listener.BeginGetContext(ListenerCallback, listener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        public void ProcessQuery(HttpListenerRequest request, HttpListenerResponse response)
        {
            var r = new Dictionary<string, string>();
            foreach (string key in request.QueryString.Keys)
                r.Add(key, request.QueryString[key]);
            Callback?.Invoke(request.Url.LocalPath.Substring(1), r, response.OutputStream);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            var context = listener.EndGetContext(result);
            Debug.WriteLine("Method: " + context.Request.HttpMethod);
            ProcessQuery(context.Request, context.Response);
            context.Response.Close();
        }
    }
}
