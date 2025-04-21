using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteProjectile;
    [SerializeField] private AudioSource soundEndLife;  // Звук, который будет проигрываться
    [SerializeField] private float speed = 5f;

    private int damage;
    private Transform target;
    private ColorTeam team;

    private void Start()
    {
        Destroy(gameObject, 10f); 
    }
    public void Initialize(Transform target, int damage, ColorTeam team)
    {
        this.target = target;
        this.damage = damage;
        this.team = team;
        Debug.Log(damage);
    }

    void Update()
    {
        if (target == null)
        {
            // Проигрываем звук, если цель пропала, перед уничтожением снаряда
            PlaySoundAndDestroy();
            return;
        }

        // Движение к цели
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Поворот в сторону цели
        Vector3 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnitRTS unit = collision.GetComponent<UnitRTS>();
        BuildingRTS building = collision.GetComponent<BuildingRTS>();

        if (unit != null && unit.Team != team)
        {
            unit.GetDamage(damage);
        }
        else if (building != null && building.Team != team)
        {
            Debug.Log(damage + " archerdamage");
            building.GetDamage(damage);
        }

        spriteProjectile.enabled = false;
        PlaySoundAndDestroy();
    }



    private void PlaySoundAndDestroy()
    {
        // Проверяем, проигрывается ли звук, и если нет — проигрываем его
        if (soundEndLife != null && !soundEndLife.isPlaying)
        {

            soundEndLife.Play();
        }

        // Уничтожаем объект после завершения проигрывания звука
        Destroy(gameObject, soundEndLife.clip.length); // Уничтожаем после завершения звука
    }
}
