using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDemomanControllerRTS : UnitControllerRTS
{
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private GameObject projectilePrefab;
    protected override IEnumerator AttackRoutine()
    {
        Debug.Log("started");
        while (currentState == UnitState.Attacking || currentState == UnitState.AutoAttacking)
        {
            Transform target = (currentState == UnitState.Attacking) ? attackTarget : autoAttackTarget;

            if (target == null)
            {
                StopAttackRoutine();
                attackSound.Stop();
                yield break;
            }

            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= unitStats.AttackRange)
            {
                agent.ResetPath();
                animationController.PlayAttack(target.position, transform.position);

                UnitRTS enemyUnit = target.GetComponent<UnitRTS>();
                BuildingRTS enemyBuilding = target.GetComponent<BuildingRTS>();

                if (enemyUnit != null && enemyUnit.Team != unitStats.Team)
                {
                    GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    Projectile projectile = projectileObject.GetComponent<Projectile>();
                    attackSound.Play();
                    projectile.Initialize(enemyUnit.transform, unitStats.Damage, unitStats.Team);
                }
                else if (enemyBuilding != null && enemyBuilding.Team != unitStats.Team)
                {
                    GameObject projectileObject = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                    Projectile projectile = projectileObject.GetComponent<Projectile>();
                    attackSound.Play();
                    projectile.Initialize(enemyBuilding.transform, unitStats.Damage, unitStats.Team);
                }
            }

            yield return new WaitForSeconds(unitStats.AttackSpeed);
        }
        animationController.PlayIdle();
        attackCoroutine = null;
    }

}
