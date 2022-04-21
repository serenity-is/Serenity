using Path = System.IO.Path;

namespace Serenity.Web
{
    public static class UploadPathHelper
    {
        public static string GetThumbnailName(string path, string thumbSuffix = "_t.jpg")
        {
            if (string.IsNullOrEmpty(path))
                return path;

            return Path.ChangeExtension(path, null) + thumbSuffix;
        }

        public static void CheckFileNameSecurity(string path)
        {
            if (!PathHelper.IsSecureRelativeFile(path))
                throw new ArgumentOutOfRangeException(nameof(path));
        }

        public static string FindAvailableName(string path, Func<string, bool> exists)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (exists == null)
                throw new ArgumentNullException(nameof(exists));

            var extension = Path.GetExtension(path);
            string baseFileName = null;
            int tries = 0;
            while (exists(path) && ++tries < 10000)
            {
                if (baseFileName == null)
                    baseFileName = Path.ChangeExtension(path, null);

                path = baseFileName + " (" + tries + ")" + (extension ?? "");
            }

            return path;
        }
    }
}
