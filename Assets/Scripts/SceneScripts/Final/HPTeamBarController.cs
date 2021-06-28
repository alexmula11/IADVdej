using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPTeamBarController : MonoBehaviour
{
    [SerializeField]
    protected RectTransform hpAllyBar, hpEnemyBar;


    protected internal void actualizeHP(int team, float percent)
    {
        if (team == 0)
        {
            hpAllyBar.anchorMin = new Vector2(1 - percent, 0);
            hpAllyBar.sizeDelta = Vector2.zero;
        }
        else
        {
            hpEnemyBar.anchorMax = new Vector2(percent, 1);
            hpEnemyBar.sizeDelta = Vector2.zero;
        }
    }
}
