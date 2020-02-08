using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector2[] m_platformDestinations = null;
    [SerializeField] private float m_platformSpeed = 0.0f;
    [SerializeField] private float m_platformPauseDuration = 0.0f;
    private Vector2 m_targetDestination;
    private bool platformStopped { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        if (m_platformDestinations.Length != 0)
            m_targetDestination = m_platformDestinations[0];
    }

    // Update is called once per frame
    void Update()
    {
    }
}
