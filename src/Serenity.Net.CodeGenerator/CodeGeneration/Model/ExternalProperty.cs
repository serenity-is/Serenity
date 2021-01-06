namespace Serenity.CodeGeneration
{
    public class ExternalProperty : ExternalMember
    {
        public ExternalProperty()
        {
        }

        public string GetMethod { get; set; }
        public string SetMethod { get; set; }
    }
}