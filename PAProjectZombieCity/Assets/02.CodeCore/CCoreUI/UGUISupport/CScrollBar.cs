using UnityEngine;
using UnityEngine.UI;

public class CScrollBar : Scrollbar
{
    [SerializeField] [Range(0, 1)]
    private float FixedSize = 0f;       // ScrollRect가 임의로 Size를 조정해 주는 문제를 해결하기 위한 기능
    //------------------------------------------------------
    internal void OnScrollRectRebuild()
    {
        size = FixedSize;
    }

    internal void OnScrollRectLateUpdate()
    {
        size = FixedSize;
    }

    //-------------------------------------------------------
}
 