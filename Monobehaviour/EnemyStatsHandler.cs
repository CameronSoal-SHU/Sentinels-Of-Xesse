using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameUtilities;


public class EnemyStatsHandler : MonoBehaviour
{
	private List<Vector2> m_enemyScreenPositions = new List<Vector2>();
	[SerializeField] private GameObject m_player = null;
	[SerializeField] private float m_showStatsRadius = 0.5f;    // How close an enemy needs to be to a player to show their stats

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		GameObject.FindGameObjectsWithTag("Enemy");
		//CreateRadius();
		//UpdateStatDisplayPositions();
	}
    private void LateUpdate()
    {

    }

    //private void CreateRadius()
    //{
    //	RaycastHit2D[] raycastHits2D = Physics2D.CircleCastAll(m_player.transform.position, m_showStatsRadius, Vector2.zero, m_showStatsRadius);

    //	foreach(RaycastHit2D raycastHit2D in raycastHits2D)
    //	{
    //		if (raycastHit2D.collider.CompareTag("Enemy"))
    //		{
    //			// Only add to the list if it's not in there
    //			if (!m_enemyStatsToDisplay.Contains(raycastHit2D))
    //			{
    //				m_enemyStatsToDisplay.Add(raycastHit2D);

    //				GameObject statsDisplay = Instantiate(m_enemyStatsDisplayPrefab, m_playerUICanvas.transform);
    //				//m_enemyStatDisplays.Add(statsDisplay);
    //			}
    //		}
    //	}
    //}

    //private void UpdateCollisionList(List<RaycastHit2D> raycastHit2Ds)
    //{
    //	foreach(RaycastHit2D raycastHit2D in m_enemyStatsToDisplay)
    //	{
    //		if (!raycastHit2Ds.Contains(raycastHit2D))
    //			m_enemyStatsToDisplay.Remove(raycastHit2D);
    //	}
    //}

    //private void UpdateStatDisplayPositions()
    //{
    //	for (int i = 0; i < m_enemyStatsToDisplay.Count; ++i)
    //	{
    //		Vector2 canvasPosition = GameUtils.GetWorldPositionCanvas(m_enemyStatsToDisplay[i].transform.position);

    //		m_enemyStatDisplays[i].GetComponent<RectTransform>().position = canvasPosition;
    //		m_enemyStatDisplays[i].GetComponentInChildren<Text>().text = m_enemyStatsToDisplay[i].collider.name;
    //	}
    //}
}