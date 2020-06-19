namespace RayTracing
{
    public class Medium
    {
        public float Reference
        {
            get;
        }

        public float Betta
        {
            get;
        }

        public Medium(float reference, float betta)
        {
            Reference = reference;
            Betta = betta;
        }
    }
}