namespace GRM.Models
{
    public class Player
    {

        public Player(){}

        public int Position { get; set; }
        public string? Name { get; set; }
        public int Score { get; set; }
        public List<Player>? PlayedWith { get; set; } = new List<Player>();

        public bool PlayedAll { get; set; }
    }
}
