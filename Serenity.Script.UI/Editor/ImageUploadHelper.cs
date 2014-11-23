using jQueryApi;
using Serenity.ComponentModel;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported, Serializable]
    public class UploadInputOptions
    {
        public jQueryObject Container;
        public jQueryObject Progress;
        public string InputName;
        public bool AllowMultiple;
        public Action<UploadResponse, string, dynamic> FileDone;
    }

    public static class UploadHelper
    {
        public static jQueryObject AddUploadInput(UploadInputOptions options)
        {
            options.Container.AddClass("fileinput-button");

            var uploadInput = jQuery.FromHtml("<input/>").Attribute("type", "file").Attribute("name", options.InputName + "[]")
                .Attribute("data-url", Q.ResolveUrl("~/File/TemporaryUpload"))
                .Attribute("multiple", "multiple")
                .AppendTo(options.Container);

            if (options.AllowMultiple)
                uploadInput.Attribute("multiple", "multiple");

            uploadInput.As<dynamic>().fileupload(new
            {
                dataType = "json",
                done = new Action<jQueryEvent, dynamic>((e, data) =>
                {
                    var response = (UploadResponse)data.result;
                    if (options.FileDone != null)
                        options.FileDone(response, data.files[0].name, data);
                }),
                start = new Action(() =>
                {
                    Q.BlockUI();
                    if (options.Progress != null)
                        options.Progress.Show();
                }),
                stop = new Action(() =>
                {
                    Q.BlockUndo();
                    if (options.Progress != null)
                        options.Progress.Hide();
                }),
                progress = new Action<jQueryEvent, dynamic>((e, data) => {
                    if (options.Progress != null)
                    {
                        double percent = (double)data.loaded / (double)data.total * 100;
                        options.Progress.Children().CSS("width", percent.ToString() + "%");
                    }
                })
            });

            return uploadInput;
        }

        public static bool CheckImageConstraints(UploadResponse file, ImageUploadEditorOptions opt)
        {
            if (!file.IsImage)
            {
                Q.Alert("Yüklemeye çalıştığınız dosya bir resim değil!");
                return false;
            }

            if (opt.MinSize > 0 && file.Size < opt.MinSize)
            {
                Q.Alert(String.Format("Yükleyeceğiniz dosya en az {0} boyutunda olmalı!", opt.MinSize));
                return false;
            }

            if (opt.MaxSize > 0 && file.Size > opt.MaxSize)
            {
                Q.Alert(String.Format("Yükleyeceğiniz dosya en çok {0} boyutunda olabilir!", opt.MaxSize));
                return false;
            }

            if (opt.MinWidth > 0 && file.Width < opt.MinWidth)
            {
                Q.Alert(String.Format("Yükleyeceğiniz resim en az {0} genişliğinde olmalı!", opt.MinWidth));
                return false;
            }

            if (opt.MaxWidth > 0 && file.Width > opt.MaxWidth)
            {
                Q.Alert(String.Format("Yükleyeceğiniz dosya en çok {0} genişliğinde olabilir!", opt.MaxWidth));
                return false;
            }

            if (opt.MinHeight > 0 && file.Height < opt.MinHeight)
            {
                Q.Alert(String.Format("Yükleyeceğiniz resim en az {0} yüksekliğinde olmalı!", opt.MinHeight));
                return false;
            }

            if (opt.MaxHeight > 0 && file.Height > opt.MaxHeight)
            {
                Q.Alert(String.Format("Yükleyeceğiniz dosya en çok {0} yüksekliğinde olabilir!", opt.MaxHeight));
                return false;
            }

            return true;
        }

        public static string FileNameSizeDisplay(string name, long bytes)
        {
            return name + " (" + FileSizeDisplay(bytes) + ")";
        }

        public static string FileSizeDisplay(long bytes)
        {
            var byteSize = (Math.Round((Decimal)bytes * 100m / 1024m) * 0.01m);
            var suffix = "KB";
            if (byteSize > 1000)
            {
                byteSize = (Math.Round((Decimal)byteSize * 0.001m * 100m) * 0.01m);
                suffix = "MB";
            }
            var sizeParts = byteSize.ToString().Split('.');
            string value;
            if (sizeParts.Length > 1)
            {
                value = sizeParts[0] + "." + sizeParts[1].Substring(0, 2);
            }
            else
            {
                value = sizeParts[0];
            }
            return value + " " + suffix;
        }

        public static bool HasImageExtension(string filename)
        {
            if (filename == null)
                return false;

            filename = filename.ToLower();

            return
                filename.EndsWith(".jpg") ||
                filename.EndsWith(".jpeg") ||
                filename.EndsWith(".gif") ||
                filename.EndsWith(".png");
        }

        public static string ThumbFileName(string filename)
        {
            filename = filename ?? "";
            var idx = filename.LastIndexOf(".");
            if (idx >= 0)
                filename = filename.Substr(0, idx);

            return filename + "_t.jpg";
        }

        public static string DbFileUrl(string filename)
        {
            filename = (filename ?? "").Replace("\\", "/");
            return Q.ResolveUrl("~/upload/") + filename;
        }

        public static void ColorBox(jQueryObject link, dynamic options)
        {
            link.As<dynamic>().colorbox(new 
            {
                current = "resim {current} / {total}",
                previous = "önceki",
                next = "sonraki",
                close = "kapat"
            });
        }

        public static void PopulateFileSymbols(
            jQueryObject container,
            List<UploadedFile> items,
            bool displayOriginalName = false,
            string urlPrefix = null)
        {
            items = items ?? new List<UploadedFile>();
            container.Html("");

            for (var index = 0; index < items.Count; index++)
            {
                var item = items[index];
                var li = jQuery.FromHtml("<li/>")
                    .AddClass("file-item")
                    .Data("index", index);

                bool isImage = HasImageExtension(item.Filename);

                if (isImage)
                    li.AddClass("file-image");
                else
                    li.AddClass("file-binary");

                var editLink = "#" + index;

                var thumb = jQuery.FromHtml("<a/>")
                    .AddClass("thumb")
                    .AppendTo(li);

                string originalName = item.OriginalName ?? "";

                if (isImage)
                {
                    string fileName = item.Filename;
                    if (urlPrefix != null && fileName != null && !fileName.StartsWith("temporary/"))
                        fileName = urlPrefix + fileName;

                    thumb.Attribute("href", DbFileUrl(fileName));
                    thumb.Attribute("target", "_blank");

                    if (!originalName.IsEmptyOrNull())
                        thumb.Attribute("title", (originalName));

                    thumb.CSS("backgroundImage", "url(" + DbFileUrl(ThumbFileName(item.Filename)) + ")");
                    ColorBox(thumb, new object());
                }

                if (displayOriginalName)
                {
                    jQuery.FromHtml("<div/>")
                        .AddClass("filename")
                        .Text(originalName)
                        .Attribute("title", originalName)
                        .AppendTo(li);
                }

                li.AppendTo(container);
            };
        }
    }

    [Imported, Serializable, PreserveMemberCase]
    public class UploadResponse
    {
        public string TemporaryFile { get; set; }
        public long Size { get; set; }
        public bool IsImage { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}