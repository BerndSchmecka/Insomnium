using Newtonsoft.Json;
using System.Net;
using System;

namespace Insomnium {

    enum ELoginErrorCode {
        OK,
        M_FORBIDDEN,
        M_USER_DEACTIVATED
    }

    public class LoginHandler
    {
         public ResponseData LoginResponse {get; private set;}

         public LoginHandler(string requestStr, HttpListenerResponse response) {
             dynamic req = JsonConvert.DeserializeObject(requestStr);

             try {
                 string device_id;
                 if(req.device_id != null){
                     device_id = req.device_id;
                 } else {
                     device_id = generateDeviceID(Program.RANDOM);
                 }

                 if(validate_login() == ELoginErrorCode.OK){
                     LoginResponse = new ResponseData(response, JsonConvert.SerializeObject(new JSON_Login_Response("@" + req.identifier.user + ":" + Program.HOMESERVER_NAME, generateAccessToken(), Program.HOMESERVER_NAME, device_id)), "application/json", 200);
                 } else {
                     LoginResponse = new ResponseData(response, "{\"errcode\":\"" + validate_login().ToString() + "\"}", "application/json", 403);
                 }
             } catch (Exception ex) {
                 LoginResponse = new ResponseData(response, "{\"errcode\":\"M_UNKNOWN\", \"error\":\"" + ex.Message + "\"}", "application/json", 400);
             }
         }

         private string generateAccessToken(){
             return "not_implemented";
         }

         private string generateDeviceID(Random rnd){
             string char_store = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
             string device_id = "";
             int length = rnd.Next(5, 10+1); //5-10
             for(int i = 0; i<length; i++){
                 int index = rnd.Next(0, char_store.Length); //0-25
                 device_id += char_store[index];
             }
             return device_id;
         }

         private ELoginErrorCode validate_login() {
             return ELoginErrorCode.M_USER_DEACTIVATED;
         }
    }
}