namespace DecernoMemory
{
    public enum CardStatus
    {
        FaceDown,
        FaceUp,
        Found
    }

    public class Card
    {
        public string id { get; set; }
        public string color { get; set; }
        public CardStatus status { get; set; }
    }

}
