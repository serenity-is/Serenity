#if !COREFX
using Serenity.Web;
#endif
using System;

namespace Serenity.ComponentModel
{
    public partial class ImageUploadEditorAttribute : CustomEditorAttribute
    {
        protected ImageUploadEditorAttribute(string editorType)
            : base(editorType)
        {
#if !COREFX
            ScaleMode = ImageScaleMode.PreserveRatioNoFill;
            ThumbMode = ImageScaleMode.PreserveRatioNoFill;
#endif
        }

        public ImageUploadEditorAttribute()
            : this("ImageUpload")
        {
        }

        /// <summary>
        /// Should non-image uploads be allowed.
        /// </summary>
        public Boolean AllowNonImage
        {
            get { return GetOption<Boolean>("allowNonImage"); }
            set { SetOption("allowNonImage", value); }
        }

        /// <summary>
        /// Maximum size in bytes of the uploaded file.
        /// </summary>
        public Int32 MaxSize
        {
            get { return GetOption<Int32>("maxSize"); }
            set { SetOption("maxSize", value); }
        }

        /// <summary>
        /// Minimum size in bytes of the uploaded file.
        /// </summary>
        public Int32 MinSize
        {
            get { return GetOption<Int32>("minSize"); }
            set { SetOption("minSize", value); }
        }

#if !COREFX
        /// <summary>
        /// Maximum height in pixels of the uploaded image.
        /// </summary>
        public Int32 MaxHeight
        {
            get { return GetOption<Int32>("maxHeight"); }
            set { SetOption("maxHeight", value); }
        }

        /// <summary>
        /// Maximum width in pixels of the uploaded image.
        /// </summary>
        public Int32 MaxWidth
        {
            get { return GetOption<Int32>("maxWidth"); }
            set { SetOption("maxWidth", value); }
        }

        /// <summary>
        /// Minimum height in pixels of the uploaded image.
        /// </summary>
        public Int32 MinHeight
        {
            get { return GetOption<Int32>("minHeight"); }
            set { SetOption("minHeight", value); }
        }

        /// <summary>
        /// Minimum width in pixels of the uploaded image.
        /// </summary>
        public Int32 MinWidth
        {
            get { return GetOption<Int32>("minWidth"); }
            set { SetOption("minWidth", value); }
        }

        /// <summary>
        /// What width image should be scaled to. Default value of 0 disables it.
        /// </summary>
        public int ScaleWidth { get; set; }

        /// <summary>
        /// What height image should be scaled to. Default value of 0 disables it.
        /// </summary>
        public int ScaleHeight { get; set; }

        /// <summary>
        /// Should image be scaled up to requested size when its smaller
        /// </summary>
        public bool ScaleSmaller { get; set; }

        /// <summary>
        /// What kind of image scaling should be used to generate image.
        /// </summary>
        public ImageScaleMode ScaleMode { get; set; }

        /// <summary>
        /// List of thumbnail sizes requested. Something like
        /// "96x96;128x128;200x200"
        /// </summary>
        public string ThumbSizes { get; set; }

        /// <summary>
        /// What kind of image scaling should be used to generate thumbnails.
        /// </summary>
        public ImageScaleMode ThumbMode { get; set; }
#endif

        /// <summary>
        /// Only useful for MultipleImageUploadeEditor. Specifies
        /// whether to JSON encode value. If your field is a string
        /// field set it to true.
        /// </summary>
        public bool JsonEncodeValue
        {
            get { return GetOption<bool>("jsonEncodeValue"); }
            set { SetOption("jsonEncodeValue", value); }
        }

        /// <summary>
        /// If you want to store original name of the file uploaded,
        /// set this to the name of another string field. Only used
        /// for single image uploads.
        /// </summary>
        public String OriginalNameProperty
        {
            get { return GetOption<String>("originalNameProperty"); }
            set { SetOption("originalNameProperty", value); }
        }

        /// <summary>
        /// If you have no original name property but use original
        /// name in file name with {4} format parameter, set this
        /// to true to force ImageUploadEditor show file name.
        /// </summary>
        public Boolean DisplayFileName
        {
            get { return GetOption<Boolean>("displayFileName"); }
            set { SetOption("displayFileName", value); }
        }

        /// <summary>
        /// Should a copy of file placed in a special history folder on upload. 
        /// This helps preserving old files on update.
        /// </summary>
        public bool CopyToHistory { get; set; }

        /// <summary>
        /// Format of the file name like "ProductImage/{1:00000}/{0:00000000}_{2}".
        /// Parameter 0 is the row identity value, 1 is identity value / 1000, 
        /// and 2 is a random string like 2cxs4bc, 3 is current date/time,
        /// 4 is original file name.
        /// You can also use the shortcut "ProductImage/~" which is equivalent 
        /// to the prior sample.
        /// If you don't specify this, it will default to something like
        /// Product/ProductImage/~ which is generated by combining row class name
        /// and field name.
        /// </summary>
        public string FilenameFormat { get; set; }

        /// <summary>
        /// If you don't want ImageUploadBehavior to process this upload, 
        /// and want to handle it manually, set to true (not recommended)
        /// </summary>
        public bool DisableDefaultBehavior { get; set; }

        /// <summary>
        /// Avoid. For compability with older versions.
        /// This part of filename is used as a prefix to FilenameFormat,
        /// but it is not stored in database.
        /// </summary>
        public string SubFolder { get; set; }
    }
}