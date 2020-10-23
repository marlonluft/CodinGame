using OceanOfCode.Interfaces;
using System.Drawing;

namespace OceanOfCode.Entity
{
    public class TorpedoCommand : ICommand
    {
        public Point Position { get; set; }

        public TorpedoCommand(Point position)
        {
            Position = position;
        }

        public string Execute()
        {
            return $"TORPEDO {Position.X} {Position.Y}";
        }
    }
}
