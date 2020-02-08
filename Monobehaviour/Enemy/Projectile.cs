using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
public class Projectile : MonoBehaviour
{
    public float projectileDamage { get; set; }
    private CapsuleCollider2D capsuleCollider;

    [SerializeField] private string m_targetColliderTag;

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(m_targetColliderTag))
        {
            collision.GetComponent<BaseCharacter>().m_healthHandler.Damage(projectileDamage);
            Destroy(gameObject);
        }
    }
}
