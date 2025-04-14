using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// 이미지와 같이 RayCast와 Mask를 가지나 드로우 자체를 하지 않는다. (필레이트 절약)
// 주로 인비지블 버튼과 같이 영역을 가지고 포인트 체크를 하나 이미지가 필요하지 않을때 사용 

[RequireComponent(typeof(CanvasRenderer))]
public class CImageEmpty : MaskableGraphic 
{   
    [SerializeField]
    private RectTransform RaycastPass = null;

    //-----------------------------------------------------
    protected override void OnPopulateMesh(VertexHelper vh)
	{
        vh.Clear();
	}

    public override bool Raycast(Vector2 sp, Camera eventCamera)
    {
        if (isActiveAndEnabled == false)
            return false;

        bool bBlock = false;
        if (RaycastPass != null)
        {
            bBlock = CheckRayCastPassRect(sp, eventCamera);         
        }
        else
        {
            bBlock = base.Raycast(sp, eventCamera);
        }

        return bBlock;
    }

    //----------------------------------------------------------------
    private bool CheckRayCastPassRect(Vector2 vecScreenPosition, Camera eventCamera)
    {
        bool bBlock = false;
        if (eventCamera)
        {
            bBlock = !RectTransformUtility.RectangleContainsScreenPoint(RaycastPass, vecScreenPosition, eventCamera);
        }
        else
        {
            bBlock = !RectTransformUtility.RectangleContainsScreenPoint(RaycastPass, vecScreenPosition);
        }

        return bBlock;
    }
}


 