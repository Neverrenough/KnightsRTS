using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseStartPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private void Start()
    {
        panel.SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))  
        {
            if (panel.activeSelf)  
            {
                panel.SetActive(false);
            }
        }
    }
}
