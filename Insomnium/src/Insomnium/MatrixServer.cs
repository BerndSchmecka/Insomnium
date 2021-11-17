
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

namespace Insomnium {

    public struct ResponseData {
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
        private static string url = "http://*:9009/"; //SHOULD be 8008!!!!

        private async Task Listen(string prefix, int maxConcurrentRequests, CancellationToken token){
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();

            var requests = new HashSet<Task>();
            for(int i=0; i < maxConcurrentRequests; i++)
                requests.Add(listener.GetContextAsync());

            while (!token.IsCancellationRequested){
                Task t = await Task.WhenAny(requests);
                requests.Remove(t);

                if (t is Task<HttpListenerContext>){
                    var context = (t as Task<HttpListenerContext>).Result;
                    requests.Add(HandleInboundConnections(context));
                    requests.Add(listener.GetContextAsync());
                }
            }
        }

        private async Task HandleInboundConnections(HttpListenerContext context) {
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
                        case "/_matrix/client/r0/login":
                            rd = new ResponseData(response, JsonConvert.SerializeObject(new JSON_Login_Flows(Program.SUPPORTED_LOGIN_FLOWS)), "application/json", 200);
                            break;
                    }
                } else if (request.HttpMethod == "OPTIONS"){
                    rd = new ResponseData(response, "", "application/json", 200);
                } else if (request.HttpMethod == "POST") {
                    switch(request.Url.AbsolutePath){
                        case "/_matrix/client/r0/login":
                            StreamReader sr = new StreamReader(request.InputStream);
                            LoginHandler lh = new LoginHandler(sr.ReadToEnd(), response);
                            rd = lh.LoginResponse;
                            break;
                    }
                }
                
                await response.OutputStream.WriteAsync(rd.Data, 0, rd.Data.Length);
                response.Close();
        }

        public void StartServer(){
            CancellationToken token = new CancellationToken();
            Task listenTask = Listen(url, 32, token);
            listenTask.GetAwaiter().GetResult();
        }
    }
}