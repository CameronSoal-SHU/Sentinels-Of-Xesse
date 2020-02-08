using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxHandler : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] GameObject[] m_backgroundLayers = null;
    [SerializeField] private float m_parallaxAddedPerLayer = 0.33f;

    List<float> m_spriteLengths = new List<float>();
    private List<float> m_spriteStartPositions = new List<float>();
    private List<float> m_parallaxEffects = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();

        int parallaxEffectMultiplier = 0;
        foreach (GameObject layer in m_backgroundLayers)
        {
            m_spriteStartPositions.Add(layer.transform.position.x);
            m_spriteLengths.Add(GetComponentInChildren<SpriteRenderer>().bounds.size.x);
            m_parallaxEffects.Add(0f + (m_parallaxAddedPerLayer * parallaxEffectMultiplier));
            ++parallaxEffectMultiplier;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int layerIndex = 0;

        foreach(GameObject layer in m_backgroundLayers)
        {
            float distanceFromStart = m_camera.transform.position.x * m_parallaxEffects[layerIndex];
            layer.transform.position = new Vector3(m_spriteStartPositions[layerIndex] + distanceFromStart, layer.transform.position.y);
            ++layerIndex;
        }
    }
}
