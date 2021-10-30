using Newtonsoft.Json;
using System.Net;

namespace Insomnium {
    public class LoginHandler
    {
         public ResponseData LoginResponse {get; private set;}

         public LoginHandler(string requestStr, HttpListenerResponse response) {
             dynamic req = JsonConvert.DeserializeObject(requestStr);
             if(req.user == null){
                 LoginResponse = new ResponseData(response, "400 Bad request", "application/json", 400);
             } else {
                 LoginResponse = new ResponseData(response, JsonConvert.SerializeObject(new JSON_Login_Response("@" + req.user + ":dev.dunkelmann.eu", "sdsdasdsds", "dev.dunkelmann.eu", "ABCDEFGHI")), "application/json", 200);
             }
         }
    }
}