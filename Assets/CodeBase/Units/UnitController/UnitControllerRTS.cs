using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitControllerRTS : MonoBehaviour
{
    [SerializeField] private AudioSource runSound;
    public enum UnitState { Idle, Moving, Holding, Patrolling, Attacking, AutoAttacking, Working, Delivering }
    protected UnitState currentState = UnitState.Idle;

    protected NavMeshAgent agent;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected UnitRTS unitStats;
    public UnitRTS UnitStats => unitStats;
    [SerializeField] protected AnimationController animationController;

    protected Transform attackTarget;
    protected Transform autoAttackTarget;

    private Vector3 patrolStartPoint;
    private Vector3 patrolEndPoint;
    private bool patrolReturning;

    protected Coroutine attackCoroutine;

    virtual protected void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        animationController = GetComponent<AnimationController>();
        animationController.PlayIdle();
    }

    virtual protected void Update()
    {
        HandleAutoAttack();

        if ((currentState == UnitState.AutoAttacking || currentState == UnitState.Attacking) && attackTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);
            if (distanceToTarget > unitStats.AttackRange)
            {
                agent.SetDestination(attackTarget.position);
            }
            else
            {
                agent.ResetPath();
            }
        }

        if (currentState == UnitState.Moving && agent.remainingDistance <= 0.1f && !agent.pathPending)
        {
            Stop();
        }
        else if (currentState == UnitState.Patrolling)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                PatrolNextPoint();
            }
        }
    }
    public void Select()
    {
        unitStats.selectionIndicator.SetActive(true);
    }

    public void Deselect()
    {
        unitStats.selectionIndicator.SetActive(false);
    }

    private void HandleAutoAttack()
    {
        if (currentState == UnitState.Idle || currentState == UnitState.AutoAttacking)
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, unitStats.AutoAttackRange);
            foreach (Collider2D col in enemiesInRange)
            {
                UnitRTS enemy = col.GetComponent<UnitRTS>();
                if (enemy != null && enemy.Team != unitStats.Team && enemy != unitStats)
                {
                    StartAutoAttack(enemy.transform);
                    return;
                }
            }
        }
    }

    virtual protected IEnumerator AttackRoutine()
    {
        return null;
    }

    private void StartAttack()
    {
        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackRoutine());
        }
        runSound.Stop();
    }

    public void Attack(Transform target)
    {
        UnitRTS targetUnit = target.GetComponent<UnitRTS>();
        BuildingRTS targetBuilding = target.GetComponent<BuildingRTS>();

        if ((targetUnit != null && targetUnit.Team == unitStats.Team) ||
            (targetBuilding != null && targetBuilding.Team == unitStats.Team))
            return; // Игнорируем союзников

        StopAttackRoutine();
        attackTarget = target;
        currentState = UnitState.Attacking;
        StartAttack();
        runSound.Stop();
    }


    private void StartAutoAttack(Transform target)
    {
        UnitRTS targetUnit = target.GetComponent<UnitRTS>();
        if (currentState == UnitState.Holding || targetUnit == null || targetUnit.Team == unitStats.Team)
            return;

        autoAttackTarget = target;
        currentState = UnitState.AutoAttacking;
        StartAttack();
        runSound.Stop();
    }

    protected void StopAttackRoutine()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        attackTarget = null;
        autoAttackTarget = null;
        runSound.Stop();
    }

    public void MoveTo(Vector3 destination)
    {
        StopAutoAttack();
        agent.SetDestination(destination);
        currentState = UnitState.Moving;
        animationController.PlayRun();
        RotateTowardsTarget(destination);
        runSound.Play();
    }

    public void Stop()
    {
        StopAutoAttack();
        agent.ResetPath();
        currentState = UnitState.Idle;
        animationController.PlayIdle();
        runSound.Stop();
    }

    public void HoldPosition()
    {
        StopAutoAttack();
        agent.ResetPath();
        currentState = UnitState.Holding;
        animationController.PlayIdle();
        runSound.Stop();
    }

    public void Patrol(Vector3 point)
    {
        StopAutoAttack();
        patrolStartPoint = transform.position;
        patrolEndPoint = point;
        patrolReturning = false;
        agent.SetDestination(patrolEndPoint);
        RotateTowardsTarget(patrolEndPoint);
        currentState = UnitState.Patrolling;
        animationController.PlayRun();
        runSound.Play();
    }

    private void StopAutoAttack()
    {
        autoAttackTarget = null;
        if (currentState == UnitState.AutoAttacking)
        {
            currentState = UnitState.Idle;
            runSound.Stop();
        }
    }

    private void PatrolNextPoint()
    {
        patrolReturning = !patrolReturning;
        Vector3 nextTarget = patrolReturning ? patrolStartPoint : patrolEndPoint;
        agent.SetDestination(nextTarget);
        RotateTowardsTarget(nextTarget);
        animationController.PlayRun();
        runSound.Play();
    }

    private void RotateTowardsTarget(Vector3 targetPosition)
    {
        spriteRenderer.flipX = targetPosition.x < transform.position.x;
    }
}
