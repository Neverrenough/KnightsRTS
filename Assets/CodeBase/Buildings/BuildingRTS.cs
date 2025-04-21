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

        Debug.Log($"{gameObject.name} ������� ����: {_damage} (� ������ ����� {armor}, ����������� ����: {effectiveDamage}). �������� HP: {currentHitPoints}");

        if (currentHitPoints <= 0)
        {
            Die();
        }
    }

    public virtual void GetHeal(int _heal)
    {
        currentHitPoints = Mathf.Min(currentHitPoints + _heal, maxHitPoints);

        Debug.Log($"{gameObject.name} ����������� {_heal} HP. ������� HP: {currentHitPoints}");
    }

    virtual protected void Die()
    {
        Debug.Log($"{gameObject.name} ����������!");

        if (DeathObject != null)
        {
            Instantiate(DeathObject, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
