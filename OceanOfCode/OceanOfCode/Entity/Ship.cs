using System.Drawing;

namespace OceanOfCode.Entity
{
    public class Ship
    {
        public Ship(int id)
        {
            Id = id;
            Life = 6;
            Position = new Point();
            Torpedo = new Torpedo();            
        }

        public int Id { get; set; }
        public int Life { get; set; }
        public Point Position { get; set; }
        public Torpedo Torpedo { get; private set; }
    }
}
