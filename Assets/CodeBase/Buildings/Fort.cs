using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fort : BuildingRTS
{
    [SerializeField] private GameObject endGamePanel;
    override protected void Start()
    {
        base.Start();
    }
    protected override void Die()
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        endGamePanel.SetActive(true);
    }
}
