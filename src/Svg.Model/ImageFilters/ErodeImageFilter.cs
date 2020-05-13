﻿namespace Svg.Model
{
    public class ErodeImageFilter : ImageFilter
    {
        public int RadiusX { get; set; }
        public int RadiusY { get; set; }
        public ImageFilter? Input { get; set; }
        public CropRect? CropRect { get; set; }
    }
}
