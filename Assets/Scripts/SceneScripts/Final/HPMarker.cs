using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPMarker : MonoBehaviour
{
    [SerializeField]
    protected Image actualHP;
    

    internal void changeActualHP(float percent)
    {
        actualHP.fillAmount = percent;
    }

    internal void setPosition(Vector3 playerPos)
    {
        transform.position = playerPos;
    }

    internal void setHpColor(Color color)
    {
        actualHP.color = color;
    }
}
