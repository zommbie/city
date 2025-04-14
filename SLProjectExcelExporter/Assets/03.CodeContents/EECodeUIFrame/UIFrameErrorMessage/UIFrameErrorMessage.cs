using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFrameErrorMessage : CUIFrameBase
{
    [SerializeField] private CText MessageLog;
    //---------------------------------------------------------
    public void DoUIFrameErrorMessageSetErrorMessage(string strErrorMessage)
    {
        MessageLog.text += strErrorMessage;
    }
    public void DoUIFrameErrorMessageSetErrorMessageEnd()
    {
        MessageLog.text += "\n===================\n";
    }
    //------------------------------------------------------
    public void HandleUIFrameErrorMessageClickConfirm()
    {
        DoUIFrameSelfHide();
    }
}
