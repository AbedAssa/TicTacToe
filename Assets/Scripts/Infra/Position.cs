
namespace Infra
{
    /// <summary>
    /// Custom position struct
    /// </summary>
    public struct Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsEqual(Position position)
        {
            return X == position.X && Y == position.Y;
        }
    }
}

