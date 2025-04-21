using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeadSkool : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private void Start()
    {
        animator.Play("Dead");
    }
}
