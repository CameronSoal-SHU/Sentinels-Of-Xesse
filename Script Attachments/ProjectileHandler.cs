using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileHandler
{
    [SerializeField] private GameObject m_projectileObject = null;
    [SerializeField] private float m_projectileDamage = 5.0f;
    [SerializeField] private float m_projectileSpeed = 5.0f;
    [SerializeField] private float m_projectileFireRate = 1.0f;
    [SerializeField] private float m_projectileDuration = 1.0f;
    public float fireRateDelayRemaining { get; set; }
    private AnimatorController m_animatorController;

    public void Init(AnimatorController animatorController)
    {
        m_animatorController = animatorController;
        fireRateDelayRemaining = m_projectileFireRate;
    }

    public Vector2 CalculateVelocity(Vector2 origin, Vector2 target)
    {
        return target - origin;
    }

    public void CreateProjectile(Vector2 origin, Vector2 target)
    {
        Vector2 direction = (target - origin).normalized;
        float rotation = Vector2.Angle(origin, target) + 90f;

        GameObject projectileInstance = Object.Instantiate(m_projectileObject);
        Projectile projectileReference = projectileInstance.GetComponent<Projectile>();

        projectileReference.projectileDamage = m_projectileDamage;
        projectileInstance.transform.position = origin;
        projectileInstance.transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);
        projectileInstance.GetComponent<Rigidbody2D>().velocity = direction * m_projectileSpeed;
        Object.Destroy(projectileInstance, m_projectileDuration);
    }
    public GameObject GetProjectileObject() => m_projectileObject;
    public float GetProjectileSpeed() => m_projectileSpeed;

    public float GetFireRate() => m_projectileFireRate;
}
