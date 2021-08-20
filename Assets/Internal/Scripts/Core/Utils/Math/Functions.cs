using static UnityEngine.Mathf;

namespace Internal.Scripts.Core.Utils.Math {
    public static class Functions
    {
        public static float RangeSigmoid(float zeroHeight, float limit, float x) {
            var limit2 = 2 * limit;
            return x < 0 ? 0 : limit2 * Atan(x + Tan(zeroHeight * PI / limit2)) / PI;
        }
    }
}
