using System.Collections.Generic;

namespace Insomnium {

    class JSON_Versions {
        public string[] versions;

        public JSON_Versions(string[] versions) {
            this.versions = versions;
        }
    }

    class JSON_Federation_Version {
        public class JSON_Federation_Server_Version {
            public string name {get; set;}
            public string version {get; set;}

            public JSON_Federation_Server_Version(string name, string version){
                this.name = name;
                this.version = version;
            }
        }

        public JSON_Federation_Server_Version server {get; set;}

        public JSON_Federation_Version(string name, string version) {
            this.server = new JSON_Federation_Server_Version(name, version);
        }
    }

    class JSON_Login_Flows {
        public class JSON_flows {
            public string type {get; set;}

            public JSON_flows(string type){
                this.type = type;
            }
        }
        public List<JSON_flows> flows {get; set;}

        public JSON_Login_Flows(string[] _flows){
            this.flows = new List<JSON_flows>();
            foreach(string s in _flows){
                this.flows.Add(new JSON_flows(s));
            }
        }
    }

    class JSON_Login_Response {
        public string user_id {get;set;}
        public string access_token {get;set;}
        public string home_server {get;set;}
        public string device_id {get;set;}

        public JSON_Login_Response(string user_id, string access_token, string home_server, string device_id){
            this.user_id = user_id;
            this.access_token = access_token;
            this.home_server = home_server;
            this.device_id = device_id;
        }
    }
}