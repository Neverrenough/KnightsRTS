using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitRTS : MonoBehaviour
{
    [SerializeField] public GameObject selectionIndicator;
    [SerializeField] private GameObject DeathObject;

    [SerializeField] private ColorTeam team;
    public ColorTeam Team => team;

    [SerializeField] private float moveSpeed;
    public float MoveSpeed => moveSpeed;
    
    [SerializeField] private int armor;
    public float Armor => armor;
    
    [SerializeField] private int maxHitPoints;
    public int MaxHitPoints => maxHitPoints;

    [SerializeField] private int damage;
    public int Damage => damage;
    
    [SerializeField] private float attackSpeed;
    public float AttackSpeed => attackSpeed;

    [SerializeField] private float attackRange;
    public float AttackRange => attackRange;

    [SerializeField] private float autoAttackRange;
    public float AutoAttackRange => autoAttackRange;

    private int currentHitPoints;
    private void Start()
    {
        currentHitPoints = maxHitPoints;
    }
    public virtual void GetDamage( int _damage)
    {
        if (armor < _damage)
            currentHitPoints -= (_damage - armor);
        if (currentHitPoints <= 0)
        {
            Instantiate(DeathObject, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        Debug.Log(currentHitPoints);
    }
    public virtual void GetHeal(int _heal)
    {
        currentHitPoints += _heal;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, autoAttackRange);
    }
#endif
}
