using OceanOfCode.Entity;
using OceanOfCode.Enumerator;
using System;
using System.Collections.Generic;
using System.Drawing;
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

            var service = new Service(new Ship(myId), new Opponent(), map);

            // game loop
            while (true)
            {
                inputs = Console.ReadLine().Split(' ');

                service.UpdateData(inputs);
                service.Execute();
            }
        }
    }

    public class Service
    {
        public Ship MyShip { get; private set; }
        public Opponent Opponent { get; private set; }
        public Map Map { get; private set; }

        private List<string> commands = null;

        public Service(Ship ship, Opponent opponent, char[,] gameMap)
        {
            commands =
            new List<string>();

            MyShip = ship;
            Opponent = opponent;
            Map = new Map(gameMap);
        }

        public void UpdateData(string[] inputs)
        {
            MyShip.Position = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));

            // Update where my ship's on map
            Map.MoveShip(MyShip.Position);

            MyShip.Life = int.Parse(inputs[2]);
            Opponent.Life = int.Parse(inputs[3]);

            MyShip.Torpedo.Cooldown = int.Parse(inputs[4]);
            int sonarCooldown = int.Parse(inputs[5]);
            int silenceCooldown = int.Parse(inputs[6]);
            int mineCooldown = int.Parse(inputs[7]);

            string sonarResult = Console.ReadLine();
            Opponent.Orders.AddRange(Console.ReadLine().Split('|').ToList());
        }

        private void MoveShip(ECardinalDirection direction)
        {
            var torpedo = MyShip.Torpedo.Cooldown > 0 ? " TORPEDO" : string.Empty;
            Map.BlockCell(MyShip.Position);

            commands.Add($"MOVE {direction}{torpedo}");
        }

        private void Surface()
        {
            for (var x = 0; x < 15; x++)
            {
                for (var y = 0; y < 15; y++)
                {
                    var path = new Point(x, y);
                    if (Map.IsBlockedCell(path))
                    {
                        Map.ReleaseCell(path);
                    }
                }
            }

            commands.Add("SURFACE");
        }

        private void Torpedo(int x, int y)
        {
            commands.Add($"TORPEDO {x} {y}");
        }

        private ECardinalDirection GetAvailableDirections()
        {
            ECardinalDirection availableDirections = ECardinalDirection.None;

            // Check WEST
            if (MyShip.Position.X != 0 && Map.IsEmptyCell(MyShip.Position.X - 1, MyShip.Position.Y)) // First row
            {
                availableDirections |= ECardinalDirection.W;
            }

            // Check EAST
            if (MyShip.Position.X != 14 && Map.IsEmptyCell(MyShip.Position.X + 1, MyShip.Position.Y)) // last row
            {
                availableDirections |= ECardinalDirection.E;
            }

            // Check NORTH
            if (MyShip.Position.Y != 0 && Map.IsEmptyCell(MyShip.Position.X, MyShip.Position.Y - 1)) // first column
            {
                availableDirections |= ECardinalDirection.N;
            }

            // Check SOUTH
            if (MyShip.Position.Y != 14 && Map.IsEmptyCell(MyShip.Position.X, MyShip.Position.Y + 1)) // last column
            {
                availableDirections |= ECardinalDirection.S;
            }

            return availableDirections;
        }

        public void Execute()
        {
            Map.Print();
            commands.Clear();

            var availableDirections = GetAvailableDirections();

            if (availableDirections == ECardinalDirection.None)
            {
                Surface();
            }
            else
            {
                if (availableDirections.HasFlag(ECardinalDirection.N))
                {
                    MoveShip(ECardinalDirection.N);
                }
                else if (availableDirections.HasFlag(ECardinalDirection.S))
                {
                    MoveShip(ECardinalDirection.S);
                }
                else if (availableDirections.HasFlag(ECardinalDirection.E))
                {
                    MoveShip(ECardinalDirection.E);
                }
                else
                {
                    MoveShip(ECardinalDirection.W);
                }

                if (MyShip.Torpedo.Cooldown == 0)
                {
                    //var torpedoOrder = OpponentOrders.Select(x => x.Split(' ').Where(y=>!string.IsNullOrWhiteSpace(y) && !y.Equals("TORPEDO"))).ToArray();

                    //Torpedo(0,0);
                }
            }

            Console.WriteLine(string.Join(" | ", commands));
        }

    }
}
