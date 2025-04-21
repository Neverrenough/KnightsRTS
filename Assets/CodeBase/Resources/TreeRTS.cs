using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeRTS : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FallDown()
    {
        animator.SetTrigger("Chopped"); // Запускаем анимацию падения
    }
}
