using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    [SerializeField] private SceneLoader loader;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<UnitRTS>() != null) 
        {
            loader.LoadScene(2);
        }
    }
}
