using System;
using UnityEngine;
using GameUtilities;

[System.Serializable]
public class AStarGrid<TGridObject>
{
    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {
        public int m_x;
        public int m_y;
    }

    [SerializeField] private int m_width;
    [SerializeField] private int m_height;
    [SerializeField] private float m_cellSize;
    [SerializeField] private Vector3 m_gridOriginPosition;
    [SerializeField] private TGridObject[,] m_gridArray;

    public bool showDebug { get; set; } = false;

    // This constructor's parameters go from 0-100 really quick
    public AStarGrid(int width, int height, float cellSize, Vector3 gridOrigin,
        Func<AStarGrid<TGridObject>, int, int, TGridObject> createGridObj)
    {
        m_width = width;
        m_height = height;
        m_gridOriginPosition = gridOrigin;
        m_cellSize = cellSize;

        m_gridArray = new TGridObject[width, height];

        for (int x = 0; x < m_gridArray.GetLength(0); x++)
        {
            for(int y = 0; y < m_gridArray.GetLength(1); y++)
            {
                m_gridArray[x, y] = createGridObj(this, x, y);
            }
        }
    }

    /// <summary>
    /// Draws out a visual representation of the A* algorthm in game to view statistics.
    /// </summary>
    public void ShowDebug()
    {
        if (showDebug)
        {
            TextMesh[,] debugGridArray = new TextMesh[m_width, m_height];

            for (int x = 0; x < m_gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < m_gridArray.GetLength(1); y++)
                {
                    Vector3 textOffset = new Vector3(m_cellSize, m_cellSize) * 0.5f;    // Just makes text appear in the middle of the grid cell

                    //debugGridArray[x, y] = GameUtils.CreateWorldText(m_gridArray[x, y]?.ToString(), null, GetWorldPosition(x, y) + textOffset,
                    //    30, Color.white, TextAnchor.MiddleCenter);

                    // Draw in scene
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.red, float.PositiveInfinity);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.red, float.PositiveInfinity);
                }
            }
            // Draw the rest of the grid
            Debug.DrawLine(GetWorldPosition(0, m_height), GetWorldPosition(m_width, m_height), Color.red, float.PositiveInfinity);
            Debug.DrawLine(GetWorldPosition(m_width, 0), GetWorldPosition(m_width, m_height), Color.red, float.PositiveInfinity);

            OnGridObjectChanged += (object sender, OnGridObjectChangedEventArgs eventArgs) =>
            {
                debugGridArray[eventArgs.m_x, eventArgs.m_y].text = m_gridArray[eventArgs.m_x, eventArgs.m_y]?.ToString();
            };
        }
    }

    // Public Accessors
    public int GetWidth() => m_width;

    public int GetHeight() => m_height;

    public float GetCellSize() => m_cellSize;

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * m_cellSize + m_gridOriginPosition;
    }

    /// <summary>
    /// Convert world position into a grid position
    /// </summary>
    /// <param name="worldPosition">Position in world</param>
    /// <returns>Converted to a Vector2Int for grid position</returns>
    public Vector2Int GetXY(Vector3 worldPosition)
    {
        int flooredXPos = Mathf.FloorToInt((worldPosition - m_gridOriginPosition).x / m_cellSize);
        int flooredYPos = Mathf.FloorToInt((worldPosition - m_gridOriginPosition).y / m_cellSize);

        return new Vector2Int(flooredXPos, flooredYPos);
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y) 
    {
        x = Mathf.FloorToInt(worldPosition.x / m_cellSize);
        y = Mathf.FloorToInt(worldPosition.y / m_cellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        // Ignore any out-of-range coordinates
        if (PosIsValid(x, y))
        {
            m_gridArray[x, y] = value;
            TriggerGridObjectChanged(x, y);
        }
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        if (OnGridObjectChanged != null)
            OnGridObjectChanged(this, new OnGridObjectChangedEventArgs { m_x = x, m_y = y });
    }

    public TGridObject GetGridObject(int x, int y) => PosIsValid(x, y) ? m_gridArray[x, y] : default;
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        Vector2Int worldPosition2D = GetXY(worldPosition);
        return GetGridObject(worldPosition2D.x, worldPosition2D.y);
    }
    
    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        Vector2Int worldPosition2D = GetXY(worldPosition);
        SetGridObject(worldPosition2D.x, worldPosition2D.y, value);
    }
    
    // Check that the position is within the grid boundaries
    private bool PosIsValid(int x, int y) => (x >= 0 && y >= 0) && (x < m_width && y < m_height);
}
