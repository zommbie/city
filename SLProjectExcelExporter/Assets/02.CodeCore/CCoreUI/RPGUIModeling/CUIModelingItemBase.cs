using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIModelingItemBase : CMonoBase
{
    [SerializeField]
    private int SortingOffset = 1;

    //private CModelShapeBase m_pModelSkinnedMesh = null;
    ////-----------------------------------------------------
    //protected override void OnUnityAwake()
    //{
    //    base.OnUnityAwake();
    //    m_pModelSkinnedMesh = GetComponent<CModelShapeBase>();
    //    m_pModelSkinnedMesh.InterModelShapeInitialize();
    //    m_pModelSkinnedMesh.InterModelShapeLODEnable(false);
    //}

    //internal void InterModelingItemSortingLayer(int iSortingLayer, int iSortingOrder)
    //{
    //    gameObject.layer = iSortingLayer;
    //    m_pModelSkinnedMesh.InterModelShapeSortingLayer(iSortingLayer, iSortingOrder + SortingOffset);
    //}

}
