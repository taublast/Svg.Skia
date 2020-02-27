﻿using System;
using Xml;

namespace Svg
{
    [Element("linearGradient")]
    public class SvgLinearGradientServer : SvgGradientServer, ISvgPresentationAttributes, ISvgStylableAttributes, ISvgResourcesAttributes
    {
        [Attribute("x1", SvgElement.SvgNamespace)]
        public string? X1
        {
            get => GetAttribute("x1");
            set => SetAttribute("x1", value);
        }

        [Attribute("y1", SvgElement.SvgNamespace)]
        public string? Y1
        {
            get => GetAttribute("y1");
            set => SetAttribute("y1", value);
        }

        [Attribute("x2", SvgElement.SvgNamespace)]
        public string? X2
        {
            get => GetAttribute("x2");
            set => SetAttribute("x2", value);
        }

        [Attribute("y2", SvgElement.SvgNamespace)]
        public string? Y2
        {
            get => GetAttribute("y2");
            set => SetAttribute("y2", value);
        }

        [Attribute("gradientUnits", SvgElement.SvgNamespace)]
        public string? GradientUnits
        {
            get => GetAttribute("gradientUnits");
            set => SetAttribute("gradientUnits", value);
        }

        [Attribute("gradientTransform", SvgElement.SvgNamespace)]
        public string? GradientTransform
        {
            get => GetAttribute("gradientTransform");
            set => SetAttribute("gradientTransform", value);
        }

        [Attribute("spreadMethod", SvgElement.SvgNamespace)]
        public string? SpreadMethod
        {
            get => GetAttribute("spreadMethod");
            set => SetAttribute("spreadMethod", value);
        }

        [Attribute("href", SvgElement.XLinkNamespace)]
        public string? Href
        {
            get => GetAttribute("href");
            set => SetAttribute("href", value);
        }

        public override void Print(Action<string> write, string indent)
        {
            base.Print(write, indent);

            if (X1 != null)
            {
                write($"{indent}{nameof(X1)}: \"{X1}\"");
            }
            if (Y1 != null)
            {
                write($"{indent}{nameof(Y1)}: \"{Y1}\"");
            }
            if (X2 != null)
            {
                write($"{indent}{nameof(X2)}: \"{X2}\"");
            }
            if (Y2 != null)
            {
                write($"{indent}{nameof(Y2)}: \"{Y2}\"");
            }
            if (GradientUnits != null)
            {
                write($"{indent}{nameof(GradientUnits)}: \"{GradientUnits}\"");
            }
            if (GradientTransform != null)
            {
                write($"{indent}{nameof(GradientTransform)}: \"{GradientTransform}\"");
            }
            if (SpreadMethod != null)
            {
                write($"{indent}{nameof(SpreadMethod)}: \"{SpreadMethod}\"");
            }
            if (Href != null)
            {
                write($"{indent}{nameof(Href)}: \"{Href}\"");
            }
        }
    }
}
