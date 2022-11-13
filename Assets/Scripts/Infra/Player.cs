
namespace Infra
{
    /// <summary>
    /// Responsible of holding player data.
    /// </summary>
    public class Player
    {
        public string PlayerName { get; private set; }
        public Symbol PlayerSymbol { get; private set; }
        public PlayerType PlayerType { get; private set; }
        
        public Player(string playerName, Symbol playerSymbol,PlayerType playerType)
        {
            PlayerName = playerName;
            PlayerSymbol = playerSymbol;
            PlayerType = playerType;
        }
    }
}

