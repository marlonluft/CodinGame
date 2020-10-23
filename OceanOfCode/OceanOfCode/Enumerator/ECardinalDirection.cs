using System;

namespace OceanOfCode.Enumerator
{
    [Flags]
    public enum ECardinalDirection
    {
        None = 0,

        /// <summary>
        /// North
        /// </summary>
        N = 1,

        /// <summary>
        /// East
        /// </summary>
        E = 2,

        /// <summary>
        /// South
        /// </summary>
        S = 4,

        /// <summary>
        /// West
        /// </summary>
        W = 8,
    }
}
