using UnityEngine;

public class ChunkPosition
{
    public Vector2Int Position { get; private set; }
    public Vector2Int WorldPosition { get; private set; }
    public int x => Position.x;
    public int y => Position.y;
    
    public ChunkPosition(Vector2Int position)
    {
        this.Position = position;
        this.WorldPosition = position * Globals.ChunkSize;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(Position.x, 0, Position.y);
    }
    public override bool Equals(object obj)
    {
        if (obj is ChunkPosition other)
        {
            return Position.Equals(other.Position);
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return Position.GetHashCode();
    }
    
    public override string ToString()
    {
        return $"({Position.x}, {Position.y})";
    }
}