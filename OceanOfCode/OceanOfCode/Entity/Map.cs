using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OceanOfCode.Entity
{
    public class Map
    {
        public Map(char[,] initialContent)
        {
            Content = initialContent;
            GenerateBannedCells();
        }

        public char[,] Content { get; private set; }

        internal void MoveShip(Point position)
        {
            Content[position.X, position.Y] = MapConstant.SHIP;
        }

        internal void BlockCell(Point position)
        {
            Content[position.X, position.Y] = MapConstant.BLOCKED;
        }

        internal bool IsBlockedCell(Point point)
        {
            return Content[point.X, point.Y] == MapConstant.BLOCKED;
        }

        internal void ReleaseCell(Point path)
        {
            Content[path.X, path.Y] = MapConstant.EMPTY;
        }

        internal bool IsEmptyCell(int x, int y)
        {
            if (x < 0 || x > 14 || y < 0 || y > 14)
            {
                return false;
            }

            return Content[x, y] == MapConstant.EMPTY;
        }

        internal void Print()
        {
            for (var x = 0; x < 15; x++)
            {
                var row = string.Empty;
                for (var y = 0; y < 15; y++)
                {
                    row += Content[y, x].ToString();
                }

                Console.Error.WriteLine(row);
            }
        }

        internal bool IsIsland(int x, int y)
        {
            return Content[x, y] == MapConstant.ISLAND;
        }

        internal List<Point> GetCellEmptyNeighborhood(int x, int y)
        {
            var cells = new List<Point>();

            if (IsEmptyCell(x + 1, y))
            {
                cells.Add(new Point(x + 1, y));
            }

            if (IsEmptyCell(x - 1, y))
            {
                cells.Add(new Point(x - 1, y));
            }

            if (IsEmptyCell(x, y + 1))
            {
                cells.Add(new Point(x, y + 1));
            }

            if (IsEmptyCell(x, y - 1))
            {
                cells.Add(new Point(x, y - 1));
            }

            return cells;
        }

        internal void GenerateBannedCells()
        {
            var highBannedPropability = new List<Point>();

            for (int x = 0; x < 15; x++)
            {
                for (int y = 0; y < 15; y++)
                {
                    if (IsEmptyCell(x, y))
                    {
                        Point? currentCell = new Point(x, y);

                        do
                        {
                            var emptyNeighborhood = GetCellEmptyNeighborhood(currentCell.Value.X, currentCell.Value.Y);

                            if (emptyNeighborhood.Count < 2)
                            {
                                highBannedPropability.Add(currentCell.Value);
                                currentCell = emptyNeighborhood.First();
                            }
                            else
                            {
                                currentCell = null;

                                foreach (var cell in highBannedPropability)
                                {
                                    BanishCell(cell);
                                }

                                highBannedPropability.Clear();
                            }

                        } while (currentCell != null);
                    }
                }
            }
        }

        internal void BanishCell(Point position)
        {
            Content[position.X, position.Y] = MapConstant.BANNED;
        }
    }
}
