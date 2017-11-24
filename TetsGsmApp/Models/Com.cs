namespace GSMapp.Models
{
    public class Com
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString() => $"{Description} {Name}";
    }
}
