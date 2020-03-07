﻿using System;
using Xml;

namespace Svg.FilterEffects
{
    [Element("filter")]
    public class SvgFilter : SvgStylableElement,
        ISvgCommonAttributes,
        ISvgPresentationAttributes,
        ISvgStylableAttributes,
        ISvgResourcesAttributes
    {
        [Attribute("x", SvgNamespace)]
        public string? X
        {
            get => this.GetAttribute("x");
            set => this.SetAttribute("x", value);
        }

        [Attribute("y", SvgNamespace)]
        public string? Y
        {
            get => this.GetAttribute("y");
            set => this.SetAttribute("y", value);
        }

        [Attribute("width", SvgNamespace)]
        public string? Width
        {
            get => this.GetAttribute("width");
            set => this.SetAttribute("width", value);
        }

        [Attribute("height", SvgNamespace)]
        public string? Height
        {
            get => this.GetAttribute("height");
            set => this.SetAttribute("height", value);
        }

        [Attribute("filterRes", SvgNamespace)]
        public string? FilterRes
        {
            get => this.GetAttribute("filterRes");
            set => this.SetAttribute("filterRes", value);
        }

        [Attribute("filterUnits", SvgNamespace)]
        public string? FilterUnits
        {
            get => this.GetAttribute("filterUnits");
            set => this.SetAttribute("filterUnits", value);
        }

        [Attribute("primitiveUnits", SvgNamespace)]
        public string? PrimitiveUnits
        {
            get => this.GetAttribute("primitiveUnits");
            set => this.SetAttribute("primitiveUnits", value);
        }

        [Attribute("href", XLinkNamespace)]
        public override string? Href
        {
            get => this.GetAttribute("href");
            set => this.SetAttribute("href", value);
        }

        public override void SetPropertyValue(string key, string? value)
        {
            base.SetPropertyValue(key, value);
            switch (key)
            {
                case "x":
                    X = value;
                    break;
                case "y":
                    Y = value;
                    break;
                case "width":
                    Width = value;
                    break;
                case "height":
                    Height = value;
                    break;
                case "filterRes":
                    FilterRes = value;
                    break;
                case "filterUnits":
                    FilterUnits = value;
                    break;
                case "primitiveUnits":
                    PrimitiveUnits = value;
                    break;
                case "href":
                    Href = value;
                    break;
            }
        }

        public override void Print(Action<string> write, string indent)
        {
            base.Print(write, indent);

            if (X != null)
            {
                write($"{indent}{nameof(X)}: \"{X}\"");
            }
            if (Y != null)
            {
                write($"{indent}{nameof(Y)}: \"{Y}\"");
            }
            if (Width != null)
            {
                write($"{indent}{nameof(Width)}: \"{Width}\"");
            }
            if (Height != null)
            {
                write($"{indent}{nameof(Height)}: \"{Height}\"");
            }
            if (FilterRes != null)
            {
                write($"{indent}{nameof(FilterRes)}: \"{FilterRes}\"");
            }
            if (FilterUnits != null)
            {
                write($"{indent}{nameof(FilterUnits)}: \"{FilterUnits}\"");
            }
            if (PrimitiveUnits != null)
            {
                write($"{indent}{nameof(PrimitiveUnits)}: \"{PrimitiveUnits}\"");
            }
            if (Href != null)
            {
                write($"{indent}{nameof(Href)}: \"{Href}\"");
            }
        }
    }
}
