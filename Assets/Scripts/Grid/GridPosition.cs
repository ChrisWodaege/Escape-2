//TODO: access mod?
using System;

public struct GridPosition: IEquatable<GridPosition>
{
    public int X
    {
        get;
        private set;
    }
    public int Y
    {
        get;
        private set;
    }

    public GridPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public GridPosition GetNeighborGridPosition(GridDirection direction)
    {
        switch (direction)
        {
            case GridDirection.Up:
                return new GridPosition(X, Y - 1);
            case GridDirection.UpRight:
                if (X % 2 != 0)
                {
                    return new GridPosition(X + 1, Y);
                }
                else
                {
                    return new GridPosition(X + 1, Y - 1);
                }                
            case GridDirection.DownRight:
                if (X % 2 != 0)
                {
                    return new GridPosition(X + 1, Y + 1);
                }
                else
                {
                    return new GridPosition(X + 1, Y);
                }
            case GridDirection.Down:
                return new GridPosition(X, Y + 1);
            case GridDirection.DownLeft:
                if (X % 2 != 0)
                {
                    return new GridPosition(X - 1, Y + 1);
                }
                else
                {
                    return new GridPosition(X - 1, Y);
                }
            case GridDirection.UpLeft:
                if (X % 2 != 0)
                {
                    return new GridPosition(X - 1, Y);
                }
                else
                {
                    return new GridPosition(X - 1, Y - 1);
                }
            default:
                throw new NotImplementedException("Unknown direction.");
        }
    }

    public bool Equals(GridPosition other)
    {
        return (X == other.X && Y == other.Y);
    }
}