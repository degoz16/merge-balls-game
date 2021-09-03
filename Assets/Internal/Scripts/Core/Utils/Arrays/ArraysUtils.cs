using System.Linq;

namespace Internal.Scripts.Core.Utils.Arrays {
    public static class ArraysUtils {
        public static T[] ClampArray<T>(T[] array, int size) {
            var newArray = new T[size];
            var minSize = System.Math.Min(size, array.Length);
            for (int i = 0; i < minSize; i++) {
                newArray[i] = array[i];
            }

            if (minSize >= size) return newArray;
            for (int i = minSize; i < size; i++) {
                newArray[i] = array.Last();
            }
            
            return newArray;
        }
    }
}