using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CShapeAnimationSpriteRendererBase : CShapeAnimationBase
{
    [SerializeField]
    private Transform RootBone;
    [SerializeField]
    private int SortOrder = 0;
    
    private List<int> m_listRendererSortOrderOrigin = new List<int>();
    private List<SpriteRenderer> m_listSpriteRenderer = new List<SpriteRenderer>();
    //----------------------------------------------------------------
    protected override void OnShapeAnimationInitialize(CAssistShapeAnimationBase pAssistOwner)
    {
        base.OnShapeAnimationInitialize(pAssistOwner);
        RootBone.gameObject.GetComponentsInChildren(true, m_listSpriteRenderer);
        PrivShapeSkinnedMeshOrginSortOrder();
    }

    protected override void OnShapeAnimationSortOrder(int iSortOffset, bool bReset)
    {
        PrivShapeSpriteSortOrder(iSortOffset, bReset);
    }

    //----------------------------------------------------------------------
    private void PrivShapeSpriteSortOrder(int iSortOffset, bool bReset)
    {
        for (int i = 0; i < m_listSpriteRenderer.Count; i++)
        {
            SpriteRenderer pRenderer = m_listSpriteRenderer[i];
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
        for (int i = 0; i < m_listSpriteRenderer.Count; i++)
        {
            m_listRendererSortOrderOrigin.Add(m_listSpriteRenderer[i].sortingOrder);
        }
    }
}
