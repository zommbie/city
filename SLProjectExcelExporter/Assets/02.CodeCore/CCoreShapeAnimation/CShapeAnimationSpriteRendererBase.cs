using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CShapeAnimationSpriteRendererBase : CShapeAnimationBase
{
    [SerializeField]
    private Transform RootBone;

    private List<int> m_listRendererSortOrderOrigin = new List<int>();
    private List<SpriteRenderer> m_listSpriteRenderer = new List<SpriteRenderer>();
    //----------------------------------------------------------------
    protected override void OnShapeAnimationInitialize(IEventAnimationNotify pAniNotifyProcessor)
    {
        base.OnShapeAnimationInitialize(pAniNotifyProcessor);
        RootBone.gameObject.GetComponentsInChildren(true, m_listSpriteRenderer);
        PrivShapeSkinnedMeshOrginSortOrder();
    }

    protected override void OnShapeAnimationSortOrder(int iSortOrder, int iSortOffset)
    {
        PrivShapeSkinnedMeshSortOrder(iSortOrder);
    }

    //----------------------------------------------------------------------
    private void PrivShapeSkinnedMeshSortOrder(int iSortOffset)
    {
        for (int i = 0; i < m_listSpriteRenderer.Count; i++)
        {
            SpriteRenderer pRenderer = m_listSpriteRenderer[i];
            pRenderer.sortingOrder = m_listRendererSortOrderOrigin[i] + iSortOffset;
        }
    }

    private void PrivShapeSkinnedMeshOrginSortOrder()
    {
        for (int i = 0; i < m_listSpriteRenderer.Count; i++)
        {
            m_listRendererSortOrderOrigin.Add(m_listSpriteRenderer[i].sortingOrder);
        }
    }
}
