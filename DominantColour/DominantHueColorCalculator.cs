﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Linq;

namespace DominantColor
{
    public class DominantHueColorCalculator : IDominantColorCalculator
    {
        private float saturationThreshold;
        private float brightnessThreshold;
        private int hueSmoothFactor;
        private Dictionary<int, uint> hueHistogram;
        private Dictionary<int, uint> smoothedHueHistogram;

        /// <summary>
        /// The Hue histogram used in the calculation for dominant color
        /// </summary>
        public Dictionary<int, uint> HueHistogram
        {
            get
            {
                return new Dictionary<int, uint>(this.hueHistogram);
            }
        }

        /// <summary>
        /// The smoothed histogram used in the calculation for dominant color
        /// </summary>
        public Dictionary<int, uint> SmoothedHueHistorgram
        {
            get
            {
                return new Dictionary<int, uint>(this.smoothedHueHistogram);
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="saturationThreshold">The saturation thresshold</param>
        /// <param name="brightnessThreshold">The brightness thresshold</param>
        /// <param name="hueSmoothFactor">hue smoothing factor</param>
        public DominantHueColorCalculator(float saturationThreshold, float brightnessThreshold, int hueSmoothFactor)
        {
            this.saturationThreshold = saturationThreshold;
            this.brightnessThreshold = brightnessThreshold;
            this.hueSmoothFactor = hueSmoothFactor;
            this.hueHistogram = new Dictionary<int, uint>();
            this.smoothedHueHistogram = new Dictionary<int, uint>();
        }

        public DominantHueColorCalculator() : 
            this(0.3f, 0.0f, 4)
        {
        }

        /// <summary>
        /// Get dominant hue in given hue histogram
        /// </summary>
        /// <param name="hueHistogram"></param>
        /// <returns></returns>
        private int GetDominantHue(Dictionary<int, uint> hueHistogram)
        {
            int dominantHue = hueHistogram.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            return dominantHue;
        }

        /// <summary>
        /// Calculate dominant color for given bitmap
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public Color CalculateDominantColor<TPixel>(Image<TPixel> bitmap) where TPixel : unmanaged, IPixel<TPixel>
        {
            this.hueHistogram = ColorUtils.GetColorHueHistogram(bitmap, this.saturationThreshold,
                this.brightnessThreshold);
            this.smoothedHueHistogram = ColorUtils.SmoothHistogram(this.hueHistogram, this.hueSmoothFactor);
            int dominantHue = GetDominantHue(this.smoothedHueHistogram);
            var rgb = new SixLabors.ImageSharp.ColorSpaces.Conversion.ColorSpaceConverter().ToRgb(new Hsv(dominantHue, 1, 0.8f));
            return Color.FromRgb((byte)(rgb.R*255), (byte)(rgb.G * 255), (byte)(rgb.B * 255));
        }
    }
}
