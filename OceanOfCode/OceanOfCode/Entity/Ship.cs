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
            Torpedo = new Power(3);            
        }

        public int Id { get; set; }
        public int Life { get; set; }
        public Point Position { get; set; }
        public Power Torpedo { get; private set; }
    }
}
