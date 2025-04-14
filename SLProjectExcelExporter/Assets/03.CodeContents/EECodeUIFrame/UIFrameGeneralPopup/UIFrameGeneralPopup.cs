using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFrameGeneralPopup : CUIFrameBase
{
    public void HandleConfirmGeneralPopup()
    {
        UIManager.Instance.UIHide<UIFrameGeneralPopup>();
    }
}
