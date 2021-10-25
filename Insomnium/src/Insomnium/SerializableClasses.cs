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
}