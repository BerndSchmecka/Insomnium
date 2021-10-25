
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Insomnium {

    struct ResponseData {
        public HttpListenerResponse Response {get;}
        public byte[] Data {get;}
        public string ContentType {get;}
        public int StatusCode {get;}

        public ResponseData(HttpListenerResponse resp, string data_to_send, string contentType, int statusCode){
            this.Response = resp;
            this.Data = Encoding.UTF8.GetBytes(data_to_send);
            this.ContentType = contentType;
            this.StatusCode = statusCode;

            this.Response.ContentType = this.ContentType;
            this.Response.ContentEncoding = Encoding.UTF8;
            this.Response.ContentLength64 = this.Data.LongLength;
            this.Response.StatusCode = this.StatusCode;
        }
    }

    class MatrixServer {
        private static HttpListener listener;
        private static string url = "http://*:8008/";

        private async Task HandleInboundConnections() {
            bool runServer = true;

            while(runServer) {
                HttpListenerContext context = await listener.GetContextAsync();

                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

                ResponseData rd = new ResponseData(response, "404 Not Found", "text/plain", 404);

                if(request.HttpMethod == "GET"){
                    switch(request.Url.AbsolutePath) {
                        case "/_matrix/client/versions":
                            rd = new ResponseData(response, JsonConvert.SerializeObject(new JSON_Versions(Program.SUPPORTED_CLIENT_VERSIONS)), "application/json", 200);
                            break;
                        case "/_matrix/federation/v1/version":
                            rd = new ResponseData(response, JsonConvert.SerializeObject(new JSON_Federation_Version("Insomnium", Program.VERSION)), "application/json", 200);
                            break;
                    }
                } else if (request.HttpMethod == "OPTIONS"){
                    rd = new ResponseData(response, "", "application/json", 200);
                }
                
                await response.OutputStream.WriteAsync(rd.Data, 0, rd.Data.Length);
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