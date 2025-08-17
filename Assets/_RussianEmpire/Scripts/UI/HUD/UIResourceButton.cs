using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIResourceButton : MonoBehaviour
{
    [SerializeField] private string resourceName;

    public void ShowTooltip()
    {
        Tooltip.ShowTooltip_Static(resourceName, Mouse.current.position.value);
    }

    public void HideTooltip()
    {
        Tooltip.HideTooltip_Static();
    }
}
