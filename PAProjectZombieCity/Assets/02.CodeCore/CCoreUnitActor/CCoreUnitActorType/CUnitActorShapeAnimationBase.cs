using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitActorShapeAnimationBase : CUnitActorBase
{
    private CAssistShapeAnimationBase m_pAssistShapeAnimation = null;
    private CAssistUnitActorShapeSocketBase    m_pAssistSocketBase = null;   
    //-----------------------------------------------------------------------
    protected override void OnUnitActorAssistRegist(CAssistUnitActorBase pAssistInstance)
    {
        if (pAssistInstance is CAssistUnitActorBase)
        {
            m_pAssistShapeAnimation = pAssistInstance as CAssistShapeAnimationBase;
        }

        if (pAssistInstance is CAssistUnitActorShapeSocketBase)
        {
            m_pAssistSocketBase = pAssistInstance as CAssistUnitActorShapeSocketBase;
        }      
    }
    //-----------------------------------------------------------------------
  
}
