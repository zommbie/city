using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.Serialization;

[AddComponentMenu("UICustom/CButton", 1)]
public class CButton : Button
{
	
	//---------------------------------------------------------
	public void DoButtonClick()
    {
        OnSubmit(null);
    }
}  
