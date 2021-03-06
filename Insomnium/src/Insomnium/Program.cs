using System;

namespace Insomnium
{
    class Program
    {
        public static string[] SUPPORTED_CLIENT_VERSIONS = {"r0.6.0"};

        public static string[] SUPPORTED_LOGIN_FLOWS = {"m.login.password"};
        public static string VERSION = "1.0.0-alpha2";

        public static string HOMESERVER_NAME = "dev.dunkelmann.eu";

        public static Random RANDOM = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server on port *:9090 ...");
            MatrixServer ms = new MatrixServer();
            ms.StartServer();
        }
    }
}
