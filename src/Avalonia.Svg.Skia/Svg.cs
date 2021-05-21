﻿using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using Svg.Skia;

namespace Avalonia.Svg.Skia
{
    /// <summary>
    /// Svg control.
    /// </summary>
    public class Svg : Control, IAffectsRender
    {
        private readonly Uri _baseUri;
        private SKSvg? _svg;

        /// <summary>
        /// Defines the <see cref="Path"/> property.
        /// </summary>
        public static readonly StyledProperty<string?> PathProperty =
            AvaloniaProperty.Register<Svg, string?>(nameof(Path));

        /// <summary>
        /// Defines the <see cref="Stretch"/> property.
        /// </summary>
        public static readonly StyledProperty<Stretch> StretchProperty =
            AvaloniaProperty.Register<Svg, Stretch>(nameof(Stretch), Stretch.Uniform);

        /// <summary>
        /// Defines the <see cref="StretchDirection"/> property.
        /// </summary>
        public static readonly StyledProperty<StretchDirection> StretchDirectionProperty =
            AvaloniaProperty.Register<Svg, StretchDirection>(
                nameof(StretchDirection),
                StretchDirection.Both);

        /// <inheritdoc/>
        public event EventHandler? Invalidated;

        /// <summary>
        /// Gets or sets the Svg path.
        /// </summary>
        [Content]
        public string? Path
        {
            get => GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        /// <summary>
        /// Gets or sets a value controlling how the image will be stretched.
        /// </summary>
        public Stretch Stretch
        {
            get { return GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value controlling in what direction the image will be stretched.
        /// </summary>
        public StretchDirection StretchDirection
        {
            get { return GetValue(StretchDirectionProperty); }
            set { SetValue(StretchDirectionProperty, value); }
        }

        static Svg()
        {
            AffectsRender<Svg>(PathProperty, StretchProperty, StretchDirectionProperty);
            AffectsMeasure<Svg>(PathProperty, StretchProperty, StretchDirectionProperty);
        }
  
        /// <summary>
        /// Initializes a new instance of the <see cref="Svg"/> class.
        /// </summary>
        /// <param name="baseUri">The base URL for the XAML context.</param>
        public Svg(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Svg"/> class.
        /// </summary>
        /// <param name="serviceProvider">The XAML service provider.</param>
        public Svg(IServiceProvider serviceProvider)
        {
            _baseUri = serviceProvider.GetContextBaseUri();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_svg?.Picture == null)
            {
                return new Size();
            }

            var sourceSize = _svg?.Picture is { } 
                ? new Size(_svg.Picture.CullRect.Width, _svg.Picture.CullRect.Height) 
                : default;

            return Stretch.CalculateSize(availableSize, sourceSize, StretchDirection);

        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_svg?.Picture == null)
            {
                return new Size();
            }

            var sourceSize = _svg?.Picture is { } 
                ? new Size(_svg.Picture.CullRect.Width, _svg.Picture.CullRect.Height) 
                : default;

            return Stretch.CalculateSize(finalSize, sourceSize);
        }

        public override void Render(DrawingContext context)
        {
            var source = _svg;
            if (source?.Picture is null)
            {
                return;
            }

            var viewPort = new Rect(Bounds.Size);
            var sourceSize = new Size(source.Picture.CullRect.Width, source.Picture.CullRect.Height);

            var scale = Stretch.CalculateScaling(Bounds.Size, sourceSize, StretchDirection);
            var scaledSize = sourceSize * scale;
            var destRect = viewPort
                .CenterRect(new Rect(scaledSize))
                .Intersect(viewPort);
            var sourceRect = new Rect(sourceSize)
                .CenterRect(new Rect(destRect.Size / scale));

            var bounds = source.Picture.CullRect;
            var scaleMatrix = Matrix.CreateScale(
                destRect.Width / sourceRect.Width,
                destRect.Height / sourceRect.Height);
            var translateMatrix = Matrix.CreateTranslation(
                -sourceRect.X + destRect.X - bounds.Top,
                -sourceRect.Y + destRect.Y - bounds.Left);

            using (context.PushClip(destRect))
            using (context.PushPreTransform(translateMatrix * scaleMatrix))
            {
                context.Custom(
                    new SvgCustomDrawOperation(
                        new Rect(0, 0, bounds.Width, bounds.Height),
                        source));
            }
        }

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PathProperty)
            {
                Load();
                RaiseInvalidated(EventArgs.Empty);
            }
        }

        private void Load()
        {
            _svg?.Dispose();

            var path = Path;
            if (path is not null)
            {
                _svg = SvgSource.Load<SvgSource>(path, _baseUri);
            }
        }

        /// <summary>
        /// Raises the <see cref="Invalidated"/> event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected void RaiseInvalidated(EventArgs e) => Invalidated?.Invoke(this, e);
    }
}