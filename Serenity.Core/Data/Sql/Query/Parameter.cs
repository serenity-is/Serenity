namespace Serenity.Data
{
    public struct Parameter
    {
        private readonly string name;

        public Parameter(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }
    }
}
