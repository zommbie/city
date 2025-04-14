using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 오브젝트의 메터리얼에 관련된 작업과 소켓 삽입등의 트랜스폼기능 
// 에니메이션 작동등의 기능 

public interface IShapeAnimationMaterialRefresh
{
    public void IShapeMaterialProperty(MaterialPropertyBlock pRefreshPropertyBlock);
}

public abstract class CShapeAnimationBase : CMonoBase , IShapeAnimationMaterialRefresh
{    
    protected CAssistShapeAnimationBase m_pAssistOwner = null; 
	//------------------------------------------------------
	internal void InterShapeAnimationInitilize(CAssistShapeAnimationBase pAssistOwner)
	{
        m_pAssistOwner = pAssistOwner;
        OnShapeAnimationInitialize(pAssistOwner);
    }

    internal void InterShapeAnimationSort(int iSortOrderOffset, bool bReset)
    {
        OnShapeAnimationSortOrder(iSortOrderOffset, bReset);
    }

    internal void InterShapeAnimationUpdate(float fDelta)
    {
        OnShapeAnimationUpdate(fDelta);
    }

    internal float InterShapeAnimationPlay(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage)
    {   
        return OnShapeAnimationPlay(ref rAniUsage);
    }

    internal void InterShapeAnimationChangeSpeed(float fAniSpeed)
    {
        OnShapeAnimationChangeSpeed(fAniSpeed);
    }
    //-------------------------------------------------------------
    public void IShapeMaterialProperty(MaterialPropertyBlock pRefreshPropertyBlock)
    {
        OnShapeAnimationMaterialRefresh(pRefreshPropertyBlock);
    }
    //-----------------------------------------------------------
    public virtual bool IsPlaying() { return false; }
   
	//------------------------------------------------------------
	protected virtual void OnShapeAnimationInitialize(CAssistShapeAnimationBase pAssistOwner) { }
    protected virtual void OnShapeAnimationMaterialRefresh(MaterialPropertyBlock pRefreshPropertyBlock) { }
    protected virtual void OnShapeAnimationSortOrder(int iSortOrderOffset, bool bReset) { }
    protected virtual void OnShapeAnimationUpdate(float fDelta) { }
    protected virtual float OnShapeAnimationPlay(ref CAssistShapeAnimationBase.SAnimationUsage rAniUsage) { return 0f; }
    protected virtual void OnShapeAnimationChangeSpeed(float fAniSpeed) { }
}
