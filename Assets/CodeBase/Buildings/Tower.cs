using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : BuildingRTS
{
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private Transform firePoint;

    [SerializeField] private int damage;
    public int Damage => damage;

    [SerializeField] private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    [SerializeField] private float attackRange;
    public float AttackRange => attackRange;
    private Transform target;
    private Coroutine attackCoroutine;


    override protected void Start()
    {
        base.Start();
        StartCoroutine(FindTargetRoutine());
    }

    private IEnumerator FindTargetRoutine()
    {
        while (true)
        {
            FindTarget();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void FindTarget()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, attackRange);
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D col in enemiesInRange)
        {
            UnitRTS enemy = col.GetComponent<UnitRTS>();
            if (enemy != null && enemy.Team != Team)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }

        if (closestEnemy != target)
        {
            target = closestEnemy;
            if (target != null && attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
            Debug.Log("target");
        }
    }

    private IEnumerator AttackRoutine()
    {
        while (target != null)
        {
            Debug.Log("Fire");
            FireProjectile();
            yield return new WaitForSeconds(attackSpeed);
        }
        attackCoroutine = null;
    }

    private void FireProjectile()
    {
        Debug.Log("Created");
        if (target == null) return;

        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        newProjectile.GetComponent<Projectile>().Initialize(target, damage, Team);
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}
