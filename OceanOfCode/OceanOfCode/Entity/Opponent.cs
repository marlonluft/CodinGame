using System.Collections.Generic;

namespace OceanOfCode.Entity
{
    public class Opponent
    {
        public Opponent()
        {
            Life = 6;
            Orders = new List<string>();
        }

        public int Life { get; set; }
        public List<string> Orders { get; set; }
    }
}
