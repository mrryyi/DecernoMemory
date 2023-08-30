namespace DecernoMemory
{
    public interface IMemoryGameService {
        // Vars.
        List<Card> Cards { get; }
        int PairsFound { get; }
        int FailedAttempts { get; }
        bool IsGameOver { get; }
        List<GameRoundResult> GameRoundResults { get; }

        // Funcs.
        List<Card> GenerateFreshCards();
        Task FlipCard(Card card);
        Task CheckForMatch();
        void NewRound();
    }

    public class MemoryGameService : IMemoryGameService {

        // Publically available:
        private List<Card> cards;
        public  List<Card> Cards => cards;

        private int pairs_found = 0;
        public  int PairsFound => pairs_found;

        private bool is_game_over = false;
        public  bool IsGameOver => is_game_over;

        private int failed_attempts = 0;
        public  int FailedAttempts => failed_attempts;

        private List<GameRoundResult> game_round_results;
        public List<GameRoundResult> GameRoundResults => game_round_results;

        // Internal:
        private Card first_selected_card;
        private Card second_selected_card;
        private const int maximal_found_pairs = 8;
        private bool is_flipping = false;
        private GameRoundResult current_game_round_result;

        public MemoryGameService() {
            game_round_results = new List<GameRoundResult>();
            NewRound();
        }

        public void NewRound() {
            cards = GenerateFreshCards();
            current_game_round_result = new GameRoundResult();
            current_game_round_result.datetime_started = DateTime.Now;
        }

        public List<Card> GenerateFreshCards() {
            // Create the list of colors
            // TODO: fix to not rely on colors for card amounts.
            var colors = new List<string> {
                "#e57373", // Red    
                "#64b5f6", // Blue   
                "#81c784", // Green  
                "#fff176", // Yellow 
                "#ffb74d", // Orange 
                "#ba68c8", // Purple 
                "#ff8a80", // Pink   
                "#a1887f" // Brown  
            };

            // assign colors to cards
            var fresh_cards = new List<Card>();
            foreach (var color in colors) {
                // Could be a loop but I find this more readable.
                var card1 = new Card {
                    id = Guid.NewGuid().ToString(),
                    color = color,
                    status = CardStatus.FaceDown
                };

                var card2 = new Card {
                    id = Guid.NewGuid().ToString(),
                    color = color,
                    status = CardStatus.FaceDown
                };
                fresh_cards.Add(card1);
                fresh_cards.Add(card2);
            }

            // Standard function for randomness instead of homegrown solution.
            var random = new Random();
            fresh_cards = fresh_cards.OrderBy(x => random.Next()).ToList();

            return fresh_cards;
        }

        public async Task FlipCard(Card card) {
            // Don't allow user to trigger the flip functionality if it's in progress.
            if (is_flipping)
                return;

            is_flipping = true;

            // Don't flip if found.
            if (card.status == CardStatus.Found) {
                is_flipping = false;
                return;
            }

            card.status = CardStatus.FaceUp;

            if (first_selected_card == null)
                first_selected_card = card;
            else if (second_selected_card == null) {
                second_selected_card = card;
                await CheckForMatch();
            }

            is_flipping = false;
        }

        public async Task CheckForMatch() {
            if (first_selected_card.color == second_selected_card.color) {
                // Two second requirement before cards are considered found.
                await Task.Delay(2000);
                first_selected_card.status = CardStatus.Found;
                second_selected_card.status = CardStatus.Found;
                ++pairs_found;

                if (pairs_found == maximal_found_pairs)
                {
                    is_game_over = true;
                    current_game_round_result.datetime_ended = DateTime.Now;
                    current_game_round_result.score = failed_attempts;
                    game_round_results.Add(current_game_round_result);
                }
            }
            else {
                // Two second requirement before cards are face down again.
                await Task.Delay(2000);
                ++failed_attempts;
                first_selected_card.status = CardStatus.FaceDown;
                second_selected_card.status = CardStatus.FaceDown;
            }

            first_selected_card = null;
            second_selected_card = null;
        }


    }
}
