using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트의 메터리얼에 관련된 작업과 소켓 삽입등의 트랜스폼기능 
// 에니메이션 작동등의 기능 

public abstract class CShapeAnimationBase : CMonoBase
{
  
    protected IEventAnimationNotify m_pAniNotifyProcessor = null; 
	//------------------------------------------------------
	internal void InterShapeAnimationInitilize(IEventAnimationNotify pAniNotifyProcessor)
	{
        m_pAniNotifyProcessor = pAniNotifyProcessor;
        OnShapeAnimationInitialize(pAniNotifyProcessor);
    }

    internal void InterShapeAnimationSort(int iSortOrder, int iSortOrderOffset)
    {
        OnShapeAnimationSortOrder(iSortOrder, iSortOrderOffset);
    }

    internal void InterShapeAnimationUpdate(float fDelta)
    {
        OnShapeAnimationUpdate(fDelta);
    }

    internal void InterShapeAnimationPlay(ref SAnimationUsage rAniUsage, bool bIdle)
    {   
        OnShapeAnimationPlay(ref rAniUsage, bIdle);
    }

    //-----------------------------------------------------------
   
	//------------------------------------------------------------
	protected virtual void OnShapeAnimationInitialize(IEventAnimationNotify pAniNotifyProcessor) { }
    protected virtual void OnShapeAnimationSortOrder(int iSortOrder, int iSortOrderOffset) { }
    protected virtual void OnShapeAnimationUpdate(float fDelta) { }
    protected virtual void OnShapeAnimationPlay(ref SAnimationUsage rAniUsage, bool bIdle) { }
}
