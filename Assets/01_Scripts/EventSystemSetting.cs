using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UnityEngine.EventSystems.EventSystem))]
public class EventSystemSetting : MonoBehaviour
{
    private const float inchToCm = 2.54f;

    [SerializeField]
    private UnityEngine.EventSystems.EventSystem eventSytem;
    [SerializeField]
    private float dragThresholdCM = 0.5f;

    private void Awake()
    {
        if (eventSytem == null) eventSytem = GetComponent<UnityEngine.EventSystems.EventSystem>();

        SetDragThreshold();
    }

    void SetDragThreshold()
    {
        if(eventSytem != null)
        {
            eventSytem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }
    }
}
