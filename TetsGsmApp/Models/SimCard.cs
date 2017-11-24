namespace GSMapp.Models
{
    public class SimCard
    {
        public OperatorList Operator { get; set; }
        public string Number { get; set; }
    }

    public enum OperatorList : int
    {
        Megafon = 02,
        Mts = 01,
        Tele2 = 20,
        Beeline = 99,
        Yota
    }
}
