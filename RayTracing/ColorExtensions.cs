using System;
using Windows.UI;

namespace RayTracing
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to an <see cref="HslColor"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>The converted <see cref="HslColor"/>.</returns>
        public static HslColor ToHsl(this Color color)
        {
            const double toDouble = 1.0 / 255;
            var r = toDouble * color.R;
            var g = toDouble * color.G;
            var b = toDouble * color.B;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chroma = max - min;
            double h1;

            if (chroma == 0)
            {
                h1 = 0;
            }
            else if (max == r)
            {
                // The % operator doesn't do proper modulo on negative
                // numbers, so we'll add 6 before using it
                h1 = (((g - b) / chroma) + 6) % 6;
            }
            else if (max == g)
            {
                h1 = 2 + ((b - r) / chroma);
            }
            else
            {
                h1 = 4 + ((r - g) / chroma);
            }

            double lightness = 0.5 * (max + min);
            double saturation = chroma == 0 ? 0 : chroma / (1 - Math.Abs((2 * lightness) - 1));
            HslColor ret;

            ret.H = 60 * h1;
            ret.S = saturation;
            ret.L = lightness;
            ret.A = toDouble * color.A;

            return ret;
        }

        public static Color ToColor(this HslColor hsl)
        {
            return FromHsl(hsl.H, hsl.S, hsl.L, hsl.A);
        }

        /// <summary>
        /// Creates a <see cref="Color"/> from the specified hue, saturation, lightness, and alpha values.
        /// </summary>
        /// <param name="hue">0..360 range hue</param>
        /// <param name="saturation">0..1 range saturation</param>
        /// <param name="lightness">0..1 range lightness</param>
        /// <param name="alpha">0..1 alpha</param>
        /// <returns>The created <see cref="Color"/>.</returns>
        public static Color FromHsl(double hue, double saturation, double lightness, double alpha = 1.0)
        {
            if (hue < 0 || hue > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(hue));
            }

            double chroma = (1 - Math.Abs((2 * lightness) - 1)) * saturation;
            double h1 = hue / 60;
            double x = chroma * (1 - Math.Abs((h1 % 2) - 1));
            double m = lightness - (0.5 * chroma);
            double r1, g1, b1;

            if (h1 < 1)
            {
                r1 = chroma;
                g1 = x;
                b1 = 0;
            }
            else if (h1 < 2)
            {
                r1 = x;
                g1 = chroma;
                b1 = 0;
            }
            else if (h1 < 3)
            {
                r1 = 0;
                g1 = chroma;
                b1 = x;
            }
            else if (h1 < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = chroma;
            }
            else if (h1 < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = chroma;
            }
            else
            {
                r1 = chroma;
                g1 = 0;
                b1 = x;
            }

            byte r = (byte)(255 * (r1 + m));
            byte g = (byte)(255 * (g1 + m));
            byte b = (byte)(255 * (b1 + m));
            byte a = (byte)(255 * alpha);

            return Color.FromArgb(a, r, g, b);
        }
    }
}