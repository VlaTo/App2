namespace RayTracing
{
    public static class MathF
    {
        public static float Min(float val1, float val2) => val1 < val2 ? val1 : val2;

        public static float Max(float val1, float val2) => val1 > val2 ? val1 : val2;

        public static float Abs(float value) => 0.0f > value ? -value : value;
    }
}