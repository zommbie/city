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
    protected override void OnShapeAnimationInitialize(CAssistShapeAnimationBase pAssistOwner)
    {
        base.OnShapeAnimationInitialize(pAssistOwner);
        RootBone.gameObject.GetComponentsInChildren(true, m_listSkinnedMeshRenderer);
        PrivShapeSkinnedMeshOrginSortOrder();
    }

    protected override void OnShapeAnimationSortOrder(int iSortOffset, bool bReset)
    {
        PrivShapeSkinnedMeshSortOrder(iSortOffset, bReset);
    }

    //----------------------------------------------------------------------
    private void PrivShapeSkinnedMeshSortOrder(int iSortOffset, bool bReset)
    {
        for (int i = 0; i < m_listSkinnedMeshRenderer.Count; i++)
        {
            Renderer pRenderer = m_listSkinnedMeshRenderer[i];
            if (bReset)
            {
                pRenderer.sortingOrder = iSortOffset;
            }
            else
            {
                pRenderer.sortingOrder = m_listRendererSortOrderOrigin[i] + iSortOffset;
            }
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
