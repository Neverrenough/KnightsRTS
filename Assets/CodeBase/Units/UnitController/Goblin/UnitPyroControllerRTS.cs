using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPyroControllerRTS : UnitControllerRTS
{
    [SerializeField] private AudioSource attackSound;
    protected override IEnumerator AttackRoutine()
    {
        while (currentState == UnitState.Attacking || currentState == UnitState.AutoAttacking)
        {
            if (attackTarget == null)
            {
                StopAttackRoutine();
                Stop();
                attackSound.Stop();
                yield break;
            }

            float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
            if (distanceToTarget <= unitStats.AttackRange)
            {
                agent.ResetPath();
                animationController.PlayAttack(attackTarget.position, transform.position);

                UnitRTS enemyUnit = attackTarget.GetComponent<UnitRTS>();
                BuildingRTS enemyBuilding = attackTarget.GetComponent<BuildingRTS>();

                if (enemyUnit != null && enemyUnit.Team != unitStats.Team)
                {
                    attackSound.Play();
                    enemyUnit.GetDamage(unitStats.Damage);
                }
                else if (enemyBuilding != null && enemyBuilding.Team != unitStats.Team)
                {
                    attackSound.Play();
                    enemyBuilding.GetDamage(unitStats.Damage);
                }
            }

            yield return new WaitForSeconds(unitStats.AttackSpeed);
        }
        animationController.PlayIdle();
        attackCoroutine = null;
    }

}
