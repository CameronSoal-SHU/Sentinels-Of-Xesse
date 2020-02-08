using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding2D
{
    private const int m_MOVE_STRAIGHT_COST = 10;
    private const int m_MOVE_DIAGONAL_COST = 14;

    public static PathFinding2D m_instance { get; private set; }

    private AStarGrid<PathNode> m_grid;
    private List<PathNode> m_openList;
    private List<PathNode> m_closeList;

    public PathFinding2D(int width, int height)
    {
        m_instance = this;
        m_grid = new AStarGrid<PathNode>(width, height, 1f, Vector3.zero, 
            (AStarGrid<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
        m_grid.showDebug = true;
    }

    public AStarGrid<PathNode> GetGrid() => m_grid;

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        m_grid.GetXY(startWorldPosition, out int startX, out int startY);
        m_grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<PathNode> path = FindPath(startX, startY, endX, endY);

        if (path == null) return null;
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();

            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.GetX(), pathNode.GetY()) * m_grid.GetCellSize() + Vector3.one * m_grid.GetCellSize() * 0.5f);
            }

            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = m_grid.GetGridObject(startX, startY);
        PathNode endNode = m_grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
            return null;    // Invalid path

        m_openList = new List<PathNode> { startNode };
        m_closeList = new List<PathNode>();

        for (int x = 0; x < m_grid.GetWidth(); x++)
        {
            for (int y = 0; y < m_grid.GetHeight(); y++)
            {
                PathNode pathNode = m_grid.GetGridObject(x, y);

                pathNode.SetGCost(int.MaxValue);
                pathNode.CalculateFCost();
                pathNode.SetPreviousNode(null);
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistanceCost(startNode, endNode));
        startNode.CalculateFCost();

        while (m_openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(m_openList);

            // Reached final node
            if (currentNode == endNode)
                return CalculatePath(endNode);

            m_openList.Remove(currentNode);
            m_closeList.Add(currentNode);

            foreach (PathNode neighbouringNode in GetNeighbourList(currentNode))
            {
                if (m_closeList.Contains(neighbouringNode))
                    continue;
                if (!neighbouringNode.isWalkable)
                {
                    m_closeList.Add(neighbouringNode);
                    continue;
                }

                int tentativeGCost = currentNode.GetGCost() + CalculateDistanceCost(currentNode, neighbouringNode);

                if (tentativeGCost < neighbouringNode.GetGCost())
                {
                    neighbouringNode.SetPreviousNode(currentNode);
                    neighbouringNode.SetGCost(tentativeGCost);
                    neighbouringNode.SetHCost(CalculateDistanceCost(neighbouringNode, endNode));
                    neighbouringNode.CalculateFCost();

                    if (!m_openList.Contains(neighbouringNode))
                        m_openList.Add(neighbouringNode);
                }
            }
        }

        // Ran out of nodes on the m_openList
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (currentNode.GetX() - 1 >= 0)
        {
            // Left node
            neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY()));
            // Lower left diagonal
            if (currentNode.GetY() - 1 >= 0)
                neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() - 1));
            // Upper left diagonal
            if (currentNode.GetY() + 1 < m_grid.GetHeight())
                neighbourList.Add(GetNode(currentNode.GetX() - 1, currentNode.GetY() + 1));
        }

        if (currentNode.GetX() + 1 < m_grid.GetWidth())
        {
            // Right node
            neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY()));
            // Lower right diagonal
            if (currentNode.GetY() - 1 >= 0)
                neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY() - 1));
            // Upper right diagonal
            if (currentNode.GetY() + 1 < m_grid.GetHeight())
                neighbourList.Add(GetNode(currentNode.GetX() + 1, currentNode.GetY() + 1));
        }

        // Upwards node
        if (currentNode.GetY() + 1 < m_grid.GetHeight())
            neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() + 1));

        // Downwards node
        if (currentNode.GetY() - 1 >= 0)
            neighbourList.Add(GetNode(currentNode.GetX(), currentNode.GetY() - 1));

        return neighbourList;
    }

    public PathNode GetNode(int x, int y) => m_grid.GetGridObject(x, y);

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodes = new List<PathNode>();

        pathNodes.Add(endNode);

        PathNode currentNode = endNode;

        while (currentNode.GetPreviousNode() != null)
        {
            pathNodes.Add(currentNode.GetPreviousNode());
            currentNode = currentNode.GetPreviousNode();
        }

        pathNodes.Reverse();

        return pathNodes;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.GetX() - b.GetX());
        int yDistance = Mathf.Abs(a.GetY() - b.GetY());
        int remaining = Mathf.Abs(xDistance - yDistance);

        return (m_MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance)) + (m_MOVE_STRAIGHT_COST * remaining);
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodes)
    {
        PathNode lowestFCostNode = pathNodes[0];

        for (int i = 1; i < pathNodes.Count; i++)
        {
            if (pathNodes[i].GetFCost() < lowestFCostNode.GetFCost())
                lowestFCostNode = pathNodes[i];
        }

        return lowestFCostNode;
    }
}
