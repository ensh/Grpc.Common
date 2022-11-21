using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vtb.Trade.Grpc.Common
{
    public class IntegerComparer : IComparer<int>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(int x, int y)
        {
            int xy = x - y, yx = y - x;
            return (xy >> 31) | (int)((uint)yx >> 31);
        }
    }
}
