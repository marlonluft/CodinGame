using OceanOfCode.Entity;
using OceanOfCode.Enumerator;
using OceanOfCode.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OceanOfCode
{
    public class Service
    {
        public Ship MyShip { get; private set; }
        public Opponent Opponent { get; private set; }
        public Map Map { get; private set; }
        public List<ICommand> Commands { get; private set; }

        public Service(Ship ship, Opponent opponent, char[,] gameMap)
        {
            MyShip = ship;
            Opponent = opponent;
            Map = new Map(gameMap);
            Commands = new List<ICommand>();
        }

        public void UpdateData(string[] inputs)
        {
            MyShip.Position = new Point(int.Parse(inputs[0]), int.Parse(inputs[1]));

            // Update where my ship's on map
            Map.MoveShip(MyShip.Position);

            MyShip.Life = int.Parse(inputs[2]);
            Opponent.Life = int.Parse(inputs[3]);

            MyShip.Torpedo.Cooldown = int.Parse(inputs[4]);
            MyShip.Sonar.Cooldown = int.Parse(inputs[5]);
            MyShip.Silence.Cooldown = int.Parse(inputs[6]);
            int mineCooldown = int.Parse(inputs[7]);

            string sonarResult = Console.ReadLine();
            Opponent.Orders.AddRange(Console.ReadLine().Split('|').ToList());
        }

        private void MoveShip(ECardinalDirection direction)
        {
            Map.BlockCell(MyShip.Position);

            var moveCommand = new MoveCommand(direction)
            {
                Torpedo = MyShip.Torpedo.Cooldown > 0,
            };

            Commands.Add(moveCommand);
        }

        private void SendTorpedo()
        {
            var lastOpponentTorpedo = Opponent.Orders.Where(x => x.Contains("TORPEDO")).LastOrDefault();

            if (lastOpponentTorpedo != null)
            {
                var availableCells = new List<Point>();

                for (int x = 0; x < 15; x++)
                {
                    for (int y = 0; y < 15; y++)
                    {
                        int range = (x >= MyShip.Position.X) ?
                            x - MyShip.Position.X : MyShip.Position.X - x;

                        range += (y >= MyShip.Position.Y) ?
                            y - MyShip.Position.Y : MyShip.Position.Y - y;

                        if (range > 2 && range <= 4 && !Map.IsIsland(x, y))
                        {
                            availableCells.Add(new Point(x, y));
                        }
                    }
                }

                var opponentTorpedoPosition = lastOpponentTorpedo
                    .ToUpper()
                    .Replace("TORPEDO", string.Empty)
                    .Trim()
                    .Split(" ")
                    .Select(x => int.Parse(x))
                    .ToArray();

                var torpedoPosition = new Point(opponentTorpedoPosition[0], opponentTorpedoPosition[1]);
                if (!availableCells.Contains(torpedoPosition))
                {
                    // Check what's available cell is more close to the last opponent torpedo
                    torpedoPosition = availableCells.Select(cell =>
                    {
                        return new
                        {
                            Score =
                                ((cell.X >= torpedoPosition.X) ? cell.X - torpedoPosition.X : torpedoPosition.X - cell.X) +
                                ((cell.Y >= torpedoPosition.Y) ? cell.Y - torpedoPosition.Y : torpedoPosition.Y - cell.Y),
                            Cell = cell,
                        };
                    })
                    .OrderBy(x => x.Score)
                    .First()
                    .Cell;
                }

                var moveCommand = new TorpedoCommand(torpedoPosition);
                Commands.Add(moveCommand);
            }
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

            Commands.Add(new SurfaceCommand());
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
            Commands.Clear();

            var availableDirections = GetAvailableDirections();

            if (availableDirections == ECardinalDirection.None)
            {
                Surface();
            }
            else
            {
                if (MyShip.Torpedo.Cooldown == 0)
                {
                    SendTorpedo();
                }

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
            }

            Console.WriteLine(string.Join(" | ", Commands.Select(x => x.Execute()).ToList()));
        }

        public (int x, int y) GetStartPosition()
        {
            var initialX = 7;
            int initialY = 7;

            while (!Map.IsEmptyCell(initialX, initialY))
            {
                if (initialX == 14)
                {
                    initialX = 0;
                }
                else
                {
                    initialX++;
                }
            }

            return (initialX, initialY);
        }
    }
}
