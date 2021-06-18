using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Imap
{
    public float _cellSize { get; private set; }

    /// <summary>
    /// X Value = Influence
    /// Y Value = 0 is Walkable, 1 is Obstacle
    /// </summary>
    public Vector2[,] _map;

    public Vector2 _offset;
    public Vector2Int _mapSize;

    public Vector2Int highestInfluencePos { get; set; }
    public Vector2Int lowestInfluencePos { get; set; }

    public List<Transform> allyPosition;
    public List<Transform> enemyPosition;

    public float[] pointAverageInfluence;

    public Imap(float cellSize, Vector2Int mapSize, Vector2 offset)
    {
        _cellSize = cellSize;
        _mapSize = mapSize;
        _offset = offset;
        pointAverageInfluence = new float[3] { 0, 0, 0 };

        InitList();
    }

    public Vector2 getWorldPositionFromCell(int x, int y)
    {
        return new Vector2(_cellSize * x + _offset.x, _cellSize * y + _offset.y);
    }

    public void setWorldPositionToCell(float x, float y, float value)
    {
        int _x = Mathf.RoundToInt((x + Mathf.Abs(_offset.x)) / _cellSize);
        int _y = Mathf.RoundToInt((y + Mathf.Abs(_offset.y)) / _cellSize);

        _map[_x,_y].x = value;
    }

    private void InitList()
    {
        _map = new Vector2[(int)(_mapSize.x / _cellSize), (int)(_mapSize.y / _cellSize)];

        for (int x = 0; x < (int)(_mapSize.x / _cellSize); x++)
        {
            for (int y = 0; y < (int)(_mapSize.y / _cellSize); y++)
            {
                RaycastHit2D hit = Physics2D.BoxCast(getWorldPositionFromCell(x, y), Vector2.one * _cellSize, 0, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Tilemap"))
                    _map[x, y] = new Vector2(0, 1);
                else
                    _map[x, y] = new Vector2(0, 0);
            }
        }
    }
}
