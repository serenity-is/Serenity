namespace Serenity.Web
{
    public interface IScriptContent
    {
        public string Hash { get; }
        public DateTime Time { get; }
        public byte[] Content { get; }
        public bool CanCompress { get; }
        public byte[] CompressedContent { get; }
    }
}