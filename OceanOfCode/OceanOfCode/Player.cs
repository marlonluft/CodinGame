using System;
using System.Collections.Generic;
using System.Linq;

namespace OceanOfCode
{
    class Player
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

            Console.WriteLine("7 7");
            var myShip = new Ship(map);

            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');

                myShip.UpdateData(inputs);
                myShip.Execute();
            }
        }
    }

    public class Ship
    {
        public int MyLife { get; set; }
        public int OppLife { get; set; }

        public int MyPositionX { get; set; }
        public int MyPositionY { get; set; }

        public int TorpedoCooldown { get; set; }

        private List<string> OpponentOrders = null;
        private List<string> commands = null;
        private char[,] map = null;

        public Ship(char[,] gameMap)
        {
            MyLife =
            OppLife =
            MyPositionX =
            MyPositionY =
            TorpedoCooldown = 0;

            commands =
            OpponentOrders =
            new List<string>();

            map = gameMap;
        }

        public void UpdateData(string[] inputs)
        {
            MyPositionX = int.Parse(inputs[0]);
            MyPositionY = int.Parse(inputs[1]);

            // Update where my ship's on map
            map[MyPositionX, MyPositionY] = 'M';

            MyLife = int.Parse(inputs[2]);
            OppLife = int.Parse(inputs[3]);

            TorpedoCooldown = int.Parse(inputs[4]);
            int sonarCooldown = int.Parse(inputs[5]);
            int silenceCooldown = int.Parse(inputs[6]);
            int mineCooldown = int.Parse(inputs[7]);

            string sonarResult = Console.ReadLine();
            OpponentOrders = Console.ReadLine().Split('|').ToList();
        }

        private void MoveNorth()
        {
            var torpedo = TorpedoCooldown > 0 ? " TORPEDO" : string.Empty;
            map[MyPositionX, MyPositionY] = 'B';

            commands.Add($"MOVE N{torpedo}");
        }

        private void MoveEast()
        {
            var torpedo = TorpedoCooldown > 0 ? " TORPEDO" : string.Empty;
            map[MyPositionX, MyPositionY] = 'B';

            commands.Add($"MOVE E{torpedo}");
        }

        private void MoveWest()
        {
            var torpedo = TorpedoCooldown > 0 ? " TORPEDO" : string.Empty;
            map[MyPositionX, MyPositionY] = 'B';

            commands.Add($"MOVE W{torpedo}");
        }

        private void MoveSouth()
        {
            var torpedo = TorpedoCooldown > 0 ? " TORPEDO" : string.Empty;
            map[MyPositionX, MyPositionY] = 'B';

            commands.Add($"MOVE S{torpedo}");
        }

        private void Surface()
        {
            for (var x = 0; x < 15; x++)
            {
                for (var y = 0; y < 15; y++)
                {
                    if (map[x, y] == 'B')
                    {
                        map[x, y] = '.';
                    }
                }
            }

            commands.Add("SURFACE");
        }

        private void Torpedo(int x, int y)
        {
            commands.Add($"TORPEDO {x} {y}");
        }

        private List<char> GetAvailableSlots()
        {
            var availableSlots = new List<char>();

            // Check WEST
            if (MyPositionX != 0 && map[MyPositionX - 1, MyPositionY] == '.') // First row
            {
                availableSlots.Add('W');
            }

            // Check EAST
            if (MyPositionX != 14 && map[MyPositionX + 1, MyPositionY] == '.') // last row
            {
                availableSlots.Add('E');
            }

            // Check NORTH
            if (MyPositionY != 0 && map[MyPositionX, MyPositionY - 1] == '.') // first column
            {
                availableSlots.Add('N');
            }

            // Check SOUTH
            if (MyPositionY != 14 && map[MyPositionX, MyPositionY + 1] == '.') // last column
            {
                availableSlots.Add('S');
            }

            return availableSlots;
        }

        public void Execute()
        {
            // x = island
            // . = empty
            // M = my ship
            // B = Blocked (already passed)

            PrintMap();
            commands.Clear();

            List<char> availableSlots = GetAvailableSlots();

            if (!availableSlots.Any())
            {
                Surface();
            }
            else
            {
                switch (availableSlots.First())
                {
                    case 'N':
                        MoveNorth();
                        break;
                    case 'S':
                        MoveSouth();
                        break;
                    case 'E':
                        MoveEast();
                        break;
                    case 'W':
                        MoveWest();
                        break;
                }

                if (TorpedoCooldown == 0)
                {
                    //var torpedoOrder = OpponentOrders.Select(x => x.Split(' ').Where(y=>!string.IsNullOrWhiteSpace(y) && !y.Equals("TORPEDO"))).ToArray();

                    //Torpedo(0,0);
                }
            }

            Console.WriteLine(string.Join(" | ", commands));
        }

        private void PrintMap()
        {
            for (var x = 0; x < 15; x++)
            {
                var row = string.Empty;
                for (var y = 0; y < 15; y++)
                {
                    row += map[x, y].ToString();
                }

                Console.Error.WriteLine(row);
            }
        }

    }
}
