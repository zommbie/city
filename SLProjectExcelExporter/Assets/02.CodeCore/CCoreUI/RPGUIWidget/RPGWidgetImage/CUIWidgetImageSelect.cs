using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIWidgetImageSelect : CUIWidgetBase
{
    [SerializeField]
    private List<CImage> ImageList = null;
    //-----------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
    }

    //----------------------------------------------------
    public void DoImageSelect(int iIndex)
    {
        PrivImageSelect(iIndex);
    }

    //---------------------------------------------------
    private void PrivImageSelect(int iIndex)
    {
        for (int i = 0; i < ImageList.Count; i++)
        {
            if (i == iIndex)
            {
                ImageList[i].gameObject.SetActive(true);
            }
            else
            {
                ImageList[i].gameObject.SetActive(false);
            }
        }
    }
}
