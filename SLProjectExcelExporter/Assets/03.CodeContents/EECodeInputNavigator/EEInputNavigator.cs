using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EEInputNavigator : CMonoBase
{
    [SerializeField] private Selectable DefaultSelectUI;

    //-----------------------------------------------------------
    protected override void OnUnityStart()
    {
        base.OnUnityStart();

        if (DefaultSelectUI != null)
            DefaultSelectUI.Select();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable pSelectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
            if (pSelectable != null)
            {
                pSelectable.Select();
            }
        }
    }
}
