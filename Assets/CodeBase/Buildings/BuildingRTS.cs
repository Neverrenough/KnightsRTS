using System.Collections;
using UnityEngine;

public abstract class BuildingRTS : MonoBehaviour
{
    [SerializeField] private GameObject DeathObject;

    [SerializeField] private ColorTeam team;
    public ColorTeam Team => team;

    [SerializeField] private int armor;
    public float Armor => armor;

    [SerializeField] private int maxHitPoints;
    public int MaxHitPoints => maxHitPoints;

    protected int currentHitPoints;

    virtual protected void Start()
    {
        currentHitPoints = maxHitPoints; 
    }

    public virtual void GetDamage(int _damage)
    {
        int effectiveDamage = Mathf.Max(0, _damage - armor); 
        currentHitPoints -= effectiveDamage;

        Debug.Log($"{gameObject.name} получил урон: {_damage} (с учётом брони {armor}, фактический урон: {effectiveDamage}). Осталось HP: {currentHitPoints}");

        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public virtual void GetHeal(int _heal)
    {
        currentHitPoints = Mathf.Min(currentHitPoints + _heal, maxHitPoints);

        Debug.Log($"{gameObject.name} восстановил {_heal} HP. Текущее HP: {currentHitPoints}");
    }

    virtual protected void Die()
    {
        Debug.Log($"{gameObject.name} уничтожено!");

        if (DeathObject != null)
        {
            Instantiate(DeathObject, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
