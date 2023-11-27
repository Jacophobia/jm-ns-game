using System;
using client.Controllers;

namespace client;

public static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length > 1 && args[0] == "--client")
        {
            switch (args[1].ToLower())
            {
                case "host":
                    using (var game = new HostingClient())
                        game.Run();
                    break;

                case "thin":
                    using (var game = new NonHostingClient())
                        game.Run();
                    break;

                default:
                    Console.WriteLine("Invalid client type specified.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("No valid arguments provided. Exiting...");
        }
    }
}