﻿using OceanOfCode.Entity;
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
        private List<ECardinalDirection> MovimentHistory { get; set; }

        public Service(Ship ship, Opponent opponent, char[,] gameMap)
        {
            MyShip = ship;
            Opponent = opponent;
            Map = new Map(gameMap);
            Commands = new List<ICommand>();
            MovimentHistory = new List<ECardinalDirection>();
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

            MyShip.Sonar.Result = Console.ReadLine();
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
            if (MyShip.Position.X != 0
                && Map.IsEmptyCell(MyShip.Position.X - 1, MyShip.Position.Y)
                && Map.GetCellEmptyNeighborhood(MyShip.Position.X - 1, MyShip.Position.Y).Count > 0) // First row
            {
                availableDirections |= ECardinalDirection.W;
            }

            // Check EAST
            if (MyShip.Position.X != 14
                && Map.IsEmptyCell(MyShip.Position.X + 1, MyShip.Position.Y)
                && Map.GetCellEmptyNeighborhood(MyShip.Position.X + 1, MyShip.Position.Y).Count > 0) // last row
            {
                availableDirections |= ECardinalDirection.E;
            }

            // Check NORTH
            if (MyShip.Position.Y != 0
                && Map.IsEmptyCell(MyShip.Position.X, MyShip.Position.Y - 1)
                && Map.GetCellEmptyNeighborhood(MyShip.Position.X, MyShip.Position.Y - 1).Count > 0) // first column
            {
                availableDirections |= ECardinalDirection.N;
            }

            // Check SOUTH
            if (MyShip.Position.Y != 14
                && Map.IsEmptyCell(MyShip.Position.X, MyShip.Position.Y + 1)
                && Map.GetCellEmptyNeighborhood(MyShip.Position.X, MyShip.Position.Y + 1).Count > 0) // last column
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
                MyShip.Surfaced = true;
                Surface();
            }
            else
            {
                if (MyShip.Torpedo.Cooldown == 0)
                {
                    SendTorpedo();
                }

                var moveDirection = ECardinalDirection.E;

                if (MyShip.Surfaced && !MovimentHistory.Any())
                {
                    MyShip.Surfaced = false;
                }

                if (MyShip.Surfaced)
                {
                    var lastMoviment = MovimentHistory.Last();

                    if (lastMoviment == ECardinalDirection.N)
                    {
                        moveDirection = ECardinalDirection.S;
                    }
                    else if (lastMoviment == ECardinalDirection.S)
                    {
                        moveDirection = ECardinalDirection.N;
                    }
                    else if (lastMoviment == ECardinalDirection.E)
                    {
                        moveDirection = ECardinalDirection.W;
                    }

                    MovimentHistory.RemoveAt(MovimentHistory.Count - 1);
                }
                else
                {
                    if (availableDirections.HasFlag(ECardinalDirection.W))
                    {
                        moveDirection = ECardinalDirection.W;
                    }
                    else if (availableDirections.HasFlag(ECardinalDirection.N))
                    {
                        moveDirection = ECardinalDirection.N;
                    }
                    else if (availableDirections.HasFlag(ECardinalDirection.S))
                    {
                        moveDirection = ECardinalDirection.S;
                    }

                    MovimentHistory.Add(moveDirection);
                }

                MoveShip(moveDirection);
            }

            Console.WriteLine(string.Join(" | ", Commands.Select(x => x.Execute()).ToList()));
        }

        public (int x, int y) GetStartPosition()
        {
            for (int x = 0; x < 14; x++)
            {
                for (int y = 14; y >= 0; y--)
                {
                    if (Map.IsEmptyCell(x, y))
                    {
                        return (x, y);
                    }
                }
            }

            return (0, 0);
        }
    }
}
