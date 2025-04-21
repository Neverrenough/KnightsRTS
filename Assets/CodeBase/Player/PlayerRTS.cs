using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRTS : MonoBehaviour
{
    [SerializeField] private ColorTeam team;
    public ColorTeam Team => team;

    [SerializeField] private int gold;
    [SerializeField] private int wood;
    public void SetPlayerTeam(ColorTeam _playerColor)
    {
        team = _playerColor;
    }
    public void AddGold(int _gold)
    {
        gold += _gold;
    }
    public void AddWood(int _wood)
    {
        wood += _wood;
    }
}
