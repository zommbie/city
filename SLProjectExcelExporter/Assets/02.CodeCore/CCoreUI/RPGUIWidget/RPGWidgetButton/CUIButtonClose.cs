using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIButtonClose : CUIButtonSingleBase
{

    //-----------------------------------------------------------------
    protected override void OnButtonClick()
    {
        base.OnButtonClick();
        GetUIEntryParentsUIFrame().DoUIFrameSelfHide();
    }
     
    
}
