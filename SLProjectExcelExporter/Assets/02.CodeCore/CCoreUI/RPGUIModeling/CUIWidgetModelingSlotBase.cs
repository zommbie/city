using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetModelingSlotBase : CUIWidgetBase
{
    private CUIModelingItemBase m_pModelingItem = null;
    //-------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        m_pModelingItem = GetComponentInChildOneDepth<CUIModelingItemBase>();
    }

   
}
