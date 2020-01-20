﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Data;
using Newtonsoft.Json;
using SkiaSharp;
using Svg.Skia;

namespace SvgToPng.ViewModels
{
    [DataContract]
    public class MainWindowViewModel
    {
        [DataMember]
        public ObservableCollection<Item> Items { get; set; }

        [DataMember]
        public string OutputPath { get; set; }

        [DataMember]
        public string ReferencePath { get; set; }

        [DataMember]
        public ObservableCollection<string> ReferencePaths { get; set; }

        [IgnoreDataMember]
        public ICollectionView ItemsView { get; set; }

        [IgnoreDataMember]
        public Predicate<object> ItemsViewFilter { get; set; }

        [DataMember]
        public string ItemsFilter { get; set; }

        [DataMember]
        public bool ShowPassed { get; set; }

        [DataMember]
        public bool ShowFailed { get; set; }

        public MainWindowViewModel()
        {
        }

        public void CreateItemsView()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Items);
            ItemsView.Filter = ItemsViewFilter;
        }

        public void LoadItems(string path)
        {
            var items = Load<ObservableCollection<Item>>(path);
            if (items != null)
            {
                Items = items;
            }
        }

        public void SaveItems(string path)
        {
            Save(path, Items);
        }

        public void ClearItems()
        {
            var items = Items.ToList();

            Items.Clear();

            foreach (var item in items)
            {
                item.Dispose();
            }
        }

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
            item.Dispose();
        }

        public void ResetItem(Item item)
        {
            item.Reset();
        }

        public void UpdateItem(Item item, Action<string> textBoxOpen, Action<string> textBoxToPicture)
        {
            if (item.Svg == null)
            {
                var currentDirectory = Directory.GetCurrentDirectory();

                try
                {
                    if (File.Exists(item.SvgPath))
                    {
                        Directory.SetCurrentDirectory(Path.GetDirectoryName(item.SvgPath));

                        var stopwatchOpen = Stopwatch.StartNew();
                        item.Svg = SKSvg.Open(item.SvgPath);
                        stopwatchOpen.Stop();
                        textBoxOpen?.Invoke($"{Math.Round(stopwatchOpen.Elapsed.TotalMilliseconds, 3)}ms");
                        Debug.WriteLine($"Open: {Math.Round(stopwatchOpen.Elapsed.TotalMilliseconds, 3)}ms");

                        if (item.Svg != null)
                        {
                            var stopwatchToPicture = Stopwatch.StartNew();
                            item.Picture = SKSvg.ToPicture(item.Svg, out var drawable);
                            item.Drawable = drawable;
                            stopwatchToPicture.Stop();
                            textBoxToPicture?.Invoke($"{Math.Round(stopwatchToPicture.Elapsed.TotalMilliseconds, 3)}ms");
                            Debug.WriteLine($"ToPicture: {Math.Round(stopwatchToPicture.Elapsed.TotalMilliseconds, 3)}ms");
                        }
                        else
                        {
                            textBoxToPicture?.Invoke($"");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load svg file: {item.SvgPath}");
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }

                Directory.SetCurrentDirectory(currentDirectory);
            }

#if true
            if (item.ReferencePng == null)
            {
                try
                {
                    if (File.Exists(item.ReferencePngPath))
                    {
                        var referencePng = SKBitmap.Decode(item.ReferencePngPath);
                        if (referencePng != null)
                        {
                            item.ReferencePng = referencePng;

                            float scaleX = referencePng.Width / item.Picture.CullRect.Width;
                            float scaleY = referencePng.Height / item.Picture.CullRect.Height;

                            using (var svgBitmap = item.Picture.ToBitmap(SKColors.Transparent, scaleX, scaleY))
                            {
                                if (svgBitmap.Width == referencePng.Width && svgBitmap.Height == referencePng.Height)
                                {
                                    var pixelDiff = PixelDiff(referencePng, svgBitmap);
                                    item.PixelDiff = pixelDiff;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Failed to load reference png: {item.ReferencePngPath}");
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
            } 
#endif
        }

        public void AddItems(List<string> paths, IList<Item> items, string referencePath, string outputPath)
        {
            var fullReferencePath = string.IsNullOrWhiteSpace(referencePath) ? default : Path.GetFullPath(referencePath);

            foreach (var path in paths)
            {
                string inputName = Path.GetFileNameWithoutExtension(path);
                string referencePng = string.Empty;
                string outputPng = Path.Combine(outputPath, inputName + ".png");

                if (!string.IsNullOrWhiteSpace(fullReferencePath))
                {
                    referencePng = Path.Combine(fullReferencePath, inputName + ".png");
                }

                var item = new Item()
                {
                    Name = inputName,
                    SvgPath = path,
                    ReferencePngPath = referencePng,
                    OutputPngPath = outputPng
                };

                items.Add(item);
            }
        }

        public void SaveItemsAsPng(IList<Item> items)
        {
            foreach (var item in items)
            {
                UpdateItem(item, null, null);

                if (item.Picture != null)
                {
                    using (var stream = File.OpenWrite(item.OutputPngPath))
                    {
                        item.Picture.ToImage(stream, SKColors.Transparent, SKEncodedImageFormat.Png, 100, 1f, 1f);
                    }
                }
            }
        }

        private static JsonSerializerSettings s_jsonSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };

        public static T Load<T>(string path)
        {
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json, s_jsonSettings);
            }
            return default;
        }

        public static void Save<T>(string path, T value)
        {
            string json = JsonConvert.SerializeObject(value, s_jsonSettings);
            File.WriteAllText(path, json);
        }

        public static IEnumerable<string> GetFiles(string inputPath)
        {
            foreach (var file in Directory.EnumerateFiles(inputPath, "*.svg"))
            {
                yield return file;
            }

            foreach (var file in Directory.EnumerateFiles(inputPath, "*.svgz"))
            {
                yield return file;
            }

            foreach (var directory in Directory.EnumerateDirectories(inputPath))
            {
                foreach (var file in GetFiles(directory))
                {
                    yield return file;
                }
            }
        }

        public static IEnumerable<string> GetFilesDrop(string[] paths)
        {
            if (paths != null && paths.Length > 0)
            {
                foreach (var path in paths)
                {
                    if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    {
                        foreach (var file in GetFiles(path))
                        {
                            yield return file;
                        }
                    }
                    else
                    {
                        var extension = Path.GetExtension(path).ToLower();
                        if (extension == ".svg" || extension == ".svgz")
                        {
                            yield return path;
                        }
                    }
                }
            }
        }

        unsafe public static SKBitmap PixelDiff(SKBitmap a, SKBitmap b)
        {
            SKBitmap output = new SKBitmap(a.Width, a.Height, SKColorType.Bgra8888, SKAlphaType.Unpremul);
            byte* aPtr = (byte*)a.GetPixels().ToPointer();
            byte* bPtr = (byte*)b.GetPixels().ToPointer();
            byte* outputPtr = (byte*)output.GetPixels().ToPointer();
            int len = a.RowBytes * a.Height;
            for (int i = 0; i < len; i++)
            {
                // For alpha use the average of both images (otherwise pixels with the same alpha won't be visible)
                if ((i + 1) % 4 == 0)
                    *outputPtr = (byte)((*aPtr + *bPtr) / 2);
                else
                    *outputPtr = (byte)~(*aPtr ^ *bPtr);

                outputPtr++;
                aPtr++;
                bPtr++;
            }
            return output;
        }
    }
}
