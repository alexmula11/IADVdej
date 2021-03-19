using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseOptionDelegate : MonoBehaviour
{
    [SerializeField]
    private Color highlightedColor;

    [SerializeField]
    private UIManager ui;

    [SerializeField]
    private RawImage icon;

    [SerializeField]
    private Image frame;


    public void highlight(bool selected)
    {
        if (selected)
        {
            icon.color = highlightedColor;
        }
        else
        {
            icon.color = Color.white;
        }
    }

    internal void select(bool selected)
    {
        frame.gameObject.SetActive(selected);
    }

}
