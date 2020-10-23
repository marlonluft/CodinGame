using System;
using System.Drawing;

namespace OceanOfCode.Entity
{
    public class Map
    {
        public Map(char[,] initialContent)
        {
            Content = initialContent;
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
            return Content[x, y] == MapConstant.EMPTY;
        }

        internal void Print()
        {
            for (var x = 0; x < 15; x++)
            {
                var row = string.Empty;
                for (var y = 0; y < 15; y++)
                {
                    row += Content[x, y].ToString();
                }

                Console.Error.WriteLine(row);
            }
        }
    }
}
