using OceanOfCode.Enumerator;
using OceanOfCode.Interfaces;
using System.Collections.Generic;

namespace OceanOfCode.Entity
{
    public class MoveCommand : ICommand
    {
        public MoveCommand(ECardinalDirection directon)
        {
            Direction = directon;
        }

        public ECardinalDirection Direction { get; set; }
        public bool Torpedo { get; set; }
        public bool Sonar { get; set; }
        public bool Silence { get; set; }
        public bool Mine { get; set; }

        public void RechargePower()
        {
            Torpedo = true;
        }

        public string Execute()
        {
            var command = new List<string>()
            {
                $"MOVE {Direction.ToString()}"
            };

            if (Torpedo)
            {
                command.Add("TORPEDO");
            }

            return string.Concat(' ', command);
        }
    }
}
