
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Insomnium {
    class MatrixServer {
        private static HttpListener listener;
        private static string url = "http://*:8008/";

        private static byte[] setResponseData(ref HttpListenerResponse resp, string data_to_send, string contentType, int statusCode) {
            byte[] data = Encoding.UTF8.GetBytes(data_to_send);
            resp.ContentType = contentType;
            resp.ContentEncoding = Encoding.UTF8;
            resp.ContentLength64 = data.LongLength;
            resp.StatusCode = statusCode;
            return data;
        }

        private async Task HandleInboundConnections() {
            bool runServer = true;

            while(runServer) {
                HttpListenerContext context = await listener.GetContextAsync();

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                byte[] data = setResponseData(ref response, "404 Not Found", "text/plain", 404);

                switch(request.Url.AbsolutePath) {
                    case "/_matrix/client/versions":
                        data = setResponseData(ref response, JsonConvert.SerializeObject(new JSON_Versions(Program.SUPPORTED_CLIENT_VERSIONS)), "application/json", 200);
                        break;
                }
                
                await response.OutputStream.WriteAsync(data, 0, data.Length);
                response.Close();
            }
        }

        public void StartServer(){
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();

            Task listenTask = HandleInboundConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }
}