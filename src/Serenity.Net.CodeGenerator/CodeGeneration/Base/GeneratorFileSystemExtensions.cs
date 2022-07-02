namespace Serenity
{
    public static class GeneratorFileSystemExtensions
    {
        public static string ChangeExtension(this IGeneratorFileSystem _, string path, string extension)
        {
            return System.IO.Path.ChangeExtension(path, extension);
        }

        public static string Combine(this IGeneratorFileSystem _, string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }

        public static string Combine(this IGeneratorFileSystem _, string path1, string path2, string path3)
        {
            return System.IO.Path.Combine(path1, path2, path3);
        }

        public static string Combine(this IGeneratorFileSystem _, params string[] paths)
        {
            return System.IO.Path.Combine(paths);
        }

        public static string GetDirectoryName(this IGeneratorFileSystem _, string path)
        {
            return System.IO.Path.GetDirectoryName(path);
        }

        public static string GetFileName(this IGeneratorFileSystem _, string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(this IGeneratorFileSystem _, string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

        public static string GetExtension(this IGeneratorFileSystem _, string path)
        {
            return System.IO.Path.GetExtension(path);
        }

        public static bool IsPathRooted(this IGeneratorFileSystem _, string path)
        {
            return System.IO.Path.IsPathRooted(path);
        }
    }
}