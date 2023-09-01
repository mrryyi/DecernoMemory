namespace DecernoMemory {
    public class GameRoundResult {
        public DateTime datetime_started { get; set; }
        public DateTime datetime_ended { get; set; }
        public TimeSpan duration { get { return datetime_ended - datetime_started; } }
        public int score;
    }
}
