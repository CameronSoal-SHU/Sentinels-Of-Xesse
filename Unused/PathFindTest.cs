using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtilities;

public class PathFindTest : MonoBehaviour
{
    [SerializeField] private VisualPathFinding m_visualPathFinding = null;
    private PathFinding2D m_pathFinding2D;

    [SerializeField] int m_gridWidth = 0;
    [SerializeField] int m_gridHeight = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_pathFinding2D = new PathFinding2D(m_gridWidth, m_gridHeight);
        m_visualPathFinding.SetGrid(m_pathFinding2D.GetGrid());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = GameUtils.GetMouseWorldPosition();

            m_pathFinding2D.GetGrid().GetXY(mouseWorldPos, out int x, out int y);
            Debug.Log("x: " + x + " y: " + y);

            // Path starting from origin to mouse position on grid
            List<PathNode> path = m_pathFinding2D.FindPath(0, 0, x, y);

            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].GetX(), path[i].GetY()) * 10f + Vector3.one * 5f, 
                        new Vector3(path[i + 1].GetX(), path[i + 1].GetY()), Color.green);
                }
            }
        }
    }
}
 