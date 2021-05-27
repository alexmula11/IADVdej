using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeDelegate : MonoBehaviour
{
    [SerializeField]
    private StatsInfo.TIPO_TERRENO tipo = 0;
    internal StatsInfo.TIPO_TERRENO tipoTerreno { get { return tipo; } }
}
