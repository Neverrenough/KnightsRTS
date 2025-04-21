using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGamePanel : MonoBehaviour
{
    [SerializeField] private SceneLoader loader;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            loader.LoadScene(0);
        }
    }
}
