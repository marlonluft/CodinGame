using OceanOfCode.Entity;
using System;

namespace OceanOfCode
{
    public class Player
    {
        static void Main(string[] args)
        {
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            int width = int.Parse(inputs[0]);
            int height = int.Parse(inputs[1]);
            int myId = int.Parse(inputs[2]);

            var map = new char[15, 15];
            for (int y = 0; y < height; y++)
            {
                var lines = Console.ReadLine();
                Console.Error.WriteLine(lines);
                char[] line = lines.ToCharArray();

                for (int x = 0; x < width; x++)
                {
                    map[x, y] = line[x];
                }
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");

            var service = new Service(new Ship(myId), new Opponent(), map);

            var (initialX, initialY) = service.GetStartPosition();
            Console.WriteLine($"{initialX} {initialY}");

            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');

                service.UpdateData(inputs);
                service.Execute();
            }
        }
    }
}
