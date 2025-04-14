using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitShapeAnimationBase : CUnitCollisionBase, IEventAnimationNotify
{
	private CAssistShapeAnimationBase m_pAssistShapeAnimation;
	private CAssistShapeSocketBase    m_pAssistShapeSocket;
    //-------------------------------------------------------------
    public void IAnimationEnd(string strAniName)
    {
        OnUnitAnimationEnd(strAniName);
    }
    public void IAnimationKeyEvent(string strAniName, string strKeyName, int iArgument, float fArgument)
    {
        OnUnitAnimationEvent(strAniName, strKeyName, iArgument, fArgument);
    }

    public void IAnimationSound(string strSoundName, int iArgument)
    {
        OnUnitAnimationSound(strSoundName, iArgument);
    }

    public CShapeSocketBase GetUnitSocket(string strSocketName)
    {
        return m_pAssistShapeSocket.GetAssistShapeSocket(strSocketName);
    }

    //-------------------------------------------------------------
    protected virtual void OnUnitAnimationEnd(string strAniName) { }
    protected virtual void OnUnitAnimationEvent(string strAniName, string strKeyName, int iArgument, float fArgument) { }
    protected virtual void OnUnitAnimationSound(string strSoundName, int iArgument) { }
    //------------------------------------------------------------
    protected virtual void InstanceUnitShapeAnimation(CAssistShapeAnimationBase pAssistShapeAnimation, CAssistShapeSocketBase pAssistShapeSocket) { m_pAssistShapeAnimation = pAssistShapeAnimation; m_pAssistShapeSocket = pAssistShapeSocket; } 
}
