using UnityEngine;

public class PathNode
{
    private AStarGrid<PathNode> m_grid;
    private int m_x;
    private int m_y;

    // A* Algorithm values 
    private int m_gCost;    // Walking cost from the Start Node
    private int m_hCost;    // Heuristic cost to reach End Node
    private int m_fCost;    // Combine Estimate Cost with Actual Cost

    [SerializeField] public bool isWalkable { get; set; }

    private PathNode m_previousPathNode;    // Node came from

    public PathNode(AStarGrid<PathNode> grid, int x, int y)
    {
        m_grid = grid;
        m_x = x;
        m_y = y;
        isWalkable = true;
    }

    public void CalculateFCost() => m_fCost = m_gCost + m_hCost;

    public void SetIsWalkable(bool newValue)
    {
        isWalkable = newValue;
        m_grid.TriggerGridObjectChanged(m_x, m_y);
    }

    public override string ToString()
    {
        return m_x + ", " + m_y;
    }

    public int GetX() => m_x;
    public int GetY() => m_y;

    public int GetFCost() => m_fCost;
    public int GetGCost() => m_gCost;

    public void SetGCost(int gCost) => m_gCost = gCost;
    public void SetHCost(int hCost) => m_hCost = hCost;

    public PathNode GetPreviousNode() => m_previousPathNode;
    public void SetPreviousNode(PathNode prevNode) => m_previousPathNode = prevNode;
}
