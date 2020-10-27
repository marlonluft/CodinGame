namespace OceanOfCode.Entity
{
    public class Power
    {
        public Power(int defaultCooldown)
        {
            Cooldown = defaultCooldown;
        }

        public int Cooldown { get; set; }
    }
}
