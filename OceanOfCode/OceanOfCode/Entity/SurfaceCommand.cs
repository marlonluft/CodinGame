using OceanOfCode.Interfaces;

namespace OceanOfCode.Entity
{
    public class SurfaceCommand : ICommand
    {
        public string Execute()
        {
            return "SURFACE";
        }
    }
}
