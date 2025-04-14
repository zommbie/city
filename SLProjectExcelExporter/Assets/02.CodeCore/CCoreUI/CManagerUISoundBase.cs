using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CManagerUISoundBase : CManagerUIFrameFocusBase
{
    public enum EFadeState
    {
        None,
        FadeOut,
        FadeIn,
    }


    private CUISoundListBase m_pUISound = null;
	
    //--------------------------------------------------------
    protected override void OnUnityAwake()
    {
        base.OnUnityAwake();
    }

    protected override void OnUnityUpdate()
    {
        base.OnUnityUpdate();
    }
     
    //--------------------------------------------------------
    protected void ProtUISoundPlayEffect(int eSoundID, bool bPlayOnce)
    {

    }

    protected void ProtUISoundPlayBGMPlay(int eSoundID, bool bLoop, float fFadeInOutTime)
    {

    }

    protected void ProtUISoundPlayBGMStop(float fFadeOutTime, bool bPause)
    {
    }

    //--------------------------------------------------------------------

}
