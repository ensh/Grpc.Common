using System;
using System.Collections.Generic;

namespace Vtb.Trade.Grpc.Common
{
    public static class SelectExtension
    {
        public static IEnumerable<T> Select<T>(this T value) { yield return value; }
    }
}
