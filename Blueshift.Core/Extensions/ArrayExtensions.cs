using System;

namespace Blueshift.Core.Extensions
{
    public static class ArrayExtensions
    {
        public static bool IsNullOrEmpty(this Array array)
            => array is null || array.Length == 0;
    }
}
