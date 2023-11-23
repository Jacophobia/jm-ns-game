using System.Threading;
using client.Controllers;

// using var game = new Test2();
using var game1 = new HostingClient();
using var game2 = new NonHostingClient();
game1.Run();

var gameRunner1 = new Thread(game1.Run);
var gameRunner2 = new Thread(game2.Run);

gameRunner1.Start();
gameRunner2.Start();

gameRunner1.Join();
gameRunner2.Join();
