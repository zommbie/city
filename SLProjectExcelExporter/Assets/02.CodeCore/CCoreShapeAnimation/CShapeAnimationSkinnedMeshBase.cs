using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CShapeAnimationSkinnedMeshBase : CShapeAnimationAnimatorBase
{
    [SerializeField]
	private Transform RootBone;

    private List<int> m_listRendererSortOrderOrigin = new List<int>();
    private List<Renderer> m_listSkinnedMeshRenderer = new List<Renderer>();
    //----------------------------------------------------------------------
    protected override void OnShapeAnimationInitialize(IEventAnimationNotify pAniNotifyProcessor)
    {
        base.OnShapeAnimationInitialize(pAniNotifyProcessor);
        RootBone.gameObject.GetComponentsInChildren(true, m_listSkinnedMeshRenderer);
        PrivShapeSkinnedMeshOrginSortOrder();
    }

    protected override void OnShapeAnimationSortOrder(int iSortOrder, int iSortOffset)
    {
        PrivShapeSkinnedMeshSortOrder(iSortOrder);
    }

    //----------------------------------------------------------------------
    private void PrivShapeSkinnedMeshSortOrder(int iSortOffset)
    {
        for (int i = 0; i < m_listSkinnedMeshRenderer.Count; i++)
        {
            Renderer pRenderer = m_listSkinnedMeshRenderer[i];
            pRenderer.sortingOrder = m_listRendererSortOrderOrigin[i] + iSortOffset;
        }
    }

    private void PrivShapeSkinnedMeshOrginSortOrder()
    {
        for (int i = 0; i < m_listSkinnedMeshRenderer.Count; i++)
        {
            m_listRendererSortOrderOrigin.Add(m_listSkinnedMeshRenderer[i].sortingOrder);
        }
    }
}
