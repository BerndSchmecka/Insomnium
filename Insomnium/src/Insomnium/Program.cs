﻿using System;

namespace Insomnium
{
    class Program
    {
        public static string[] SUPPORTED_CLIENT_VERSIONS = {"r0.6.0"};
        public static string VERSION = "1.0.0-alpha1";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MatrixServer ms = new MatrixServer();
            ms.StartServer();
        }
    }
}
