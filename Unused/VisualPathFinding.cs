using UnityEngine;

public class VisualPathFinding : MonoBehaviour
{
    [SerializeField] private AStarGrid<PathNode> m_grid;
    private Mesh m_mesh;
    private bool m_updateMesh;


    private void Awake()
    {
        m_mesh = GetComponent<Mesh>();
        GetComponent<MeshFilter>().mesh = m_mesh;
    }

    public void SetGrid(AStarGrid<PathNode> grid)
    {
        m_grid = grid;
        m_grid.showDebug = true;
        m_grid.ShowDebug();
        m_grid.OnGridObjectChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, AStarGrid<PathNode>.OnGridObjectChangedEventArgs e)
    {
        m_updateMesh = true;
    }

    void LateUpdate()
    {
        if (m_updateMesh)
        {
            m_updateMesh = false;
            UpdateVisual();
        }
    }

    private void UpdateVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(m_grid.GetWidth() * m_grid.GetHeight(), 
            out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < m_grid.GetWidth(); x++)
        {
            for(int y = 0; y < m_grid.GetHeight(); y++)
            {
                int index = x * m_grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * m_grid.GetCellSize();
                PathNode pathNode = m_grid.GetGridObject(x, y);

                if (pathNode.isWalkable)
                    quadSize = Vector3.zero;

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, 
                    m_grid.GetWorldPosition(x, y) + quadSize * 0.5f, 0f, quadSize, Vector2.zero, Vector2.zero);
            }
        }

        m_mesh.vertices = vertices;
        m_mesh.uv = uv;
        m_mesh.triangles = triangles;
    }
}
