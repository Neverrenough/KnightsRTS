using System.Collections;
using System.Linq;
using UnityEngine;

public class UnitWorkerControllerRTS : UnitControllerRTS
{
    [SerializeField] private float chopDuration = 5f; // Время рубки дерева
    [SerializeField] private int woodAmount = 10; // Количество древесины за одно дерево
    [SerializeField] private GameObject woodIndicator; // Индикатор переноса дерева

    private Transform treeTarget;
    private Transform fortTarget;
    private bool carryingWood = false;

    private AnimationControllerWorker workerAnimation;

    [SerializeField] private AudioSource attackSound;

    protected override void Start()
    {
        base.Start();
        workerAnimation = GetComponent<AnimationControllerWorker>();
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == UnitState.Working && treeTarget != null)
        {
            float distanceToTree = Vector3.Distance(transform.position, treeTarget.position);

            if (distanceToTree <= unitStats.AttackRange)
            {
                agent.ResetPath();
                StartCoroutine(ChopTreeRoutine());
            }
        }
    }

    public void Chop(Transform tree)
    {
        treeTarget = tree;
        currentState = UnitState.Working;

        if (Vector3.Distance(transform.position, tree.position) <= unitStats.AttackRange)
        {
            agent.ResetPath();
            StartCoroutine(ChopTreeRoutine());
        }
        else
        {
            MoveTo(tree.position);
            StartCoroutine(WaitAndChop(tree));
        }
    }

    private IEnumerator WaitAndChop(Transform tree)
    {
        while (Vector3.Distance(transform.position, tree.position) > unitStats.AttackRange)
        {
            yield return null; // Ждём, пока юнит дойдёт
        }

        StartCoroutine(ChopTreeRoutine());
    }

    private IEnumerator ChopTreeRoutine()
    {
        workerAnimation.PlayChopAnimation();
        yield return new WaitForSeconds(chopDuration);

        if (treeTarget != null)
        {
            TreeRTS tree = treeTarget.GetComponent<TreeRTS>();
            if (tree != null)
            {
                tree.FallDown(); // Анимация падения дерева
            }
        }

        carryingWood = true;
        workerAnimation.PlayCarryAnimation();
        woodIndicator.SetActive(true);

        MoveToNearestFort();
    }

    private void MoveToNearestFort()
    {
        Fort[] forts = FindObjectsOfType<Fort>();
        fortTarget = forts.OrderBy(f => Vector3.Distance(transform.position, f.transform.position)).FirstOrDefault()?.transform;

        if (fortTarget != null)
        {
            MoveTo(fortTarget.position);
            currentState = UnitState.Delivering;
            StartCoroutine(CheckFortArrival());
        }
    }

    private IEnumerator CheckFortArrival()
    {
        while (carryingWood)
        {
            if (Vector3.Distance(transform.position, fortTarget.position) <= unitStats.AttackRange)
            {
                DeliverWood();
                yield break;
            }
            yield return null;
        }
    }

    private void DeliverWood()
    {
        carryingWood = false;
        woodIndicator.SetActive(false);
        workerAnimation.PlayIdleAnimation();

        PlayerRTS player = FindObjectOfType<PlayerRTS>();
        if (player != null)
        {
            player.AddWood(woodAmount);
        }
    }

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
