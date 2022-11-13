
namespace Infra
{
    /// <summary>
    /// Interface for SymbolView
    /// </summary>
    public interface ISymbolView
    { 
        public void SetPosition(Position symbolPosition);
        public void Reset();
        public Position Position { get;}
        public void DrawSymbol(Symbol? symbol);
        public void SetInteractable(bool isInteractable);
        public void PlayWinningAnimation();
        public void HighlightView();
    }
}

