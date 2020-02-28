﻿using System;
using Xml;

namespace Svg
{
    public interface ISvgResourcesAttributes : IElement
    {
        [Attribute("externalResourcesRequired", SvgElement.SvgNamespace)]
        public string? ExternalResourcesRequired
        {
            get => GetAttribute("externalResourcesRequired");
            set => SetAttribute("externalResourcesRequired", value);
        }
    }
}