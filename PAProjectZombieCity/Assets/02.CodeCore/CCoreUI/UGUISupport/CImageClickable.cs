using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 일반적인 이미지는 클릭 메시지를 발생시키지 않기 때문에 새롭게 구성 
// 클릭 메시지 발생과 동시에 스크롤랙트에 전달하여 스크롤링이 되는 구조 
// 스크롤이 되면서도 클릭에 의한 이미지 팝업등을 수행가능 

[AddComponentMenu("UICustom/CImageClickable", 12)]
public class CImageClickable : Image, IPointerDownHandler, IPointerClickHandler,  IPointerUpHandler, IInitializePotentialDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IBeginDragHandler
{
    private UnityAction<Vector2> m_delPointerDrag = null;
    private UnityAction<Vector2> m_delPointerDrop = null;
    private UnityAction<Vector2> m_delPointerDragBegin = null;
    private UnityAction<Vector2> m_delPointerDragEnd = null;
    private UnityAction<Vector2> m_delPointerPotentialDrag = null;
    private UnityAction<Vector2> m_delPointerClick = null;

    private IDragHandler m_pHandlerDrag = null;
	private IBeginDragHandler m_pHandlerDragBegin = null;
	private IEndDragHandler m_pHandlerDragEnd = null;
    private bool m_bDragable = false;
    private bool m_bClickCancle = false;
	//------------------------------------------------------------
	public void SetImageInputEvent(UnityAction<Vector2> delPointerClick,  bool bDragable = false, UnityAction<Vector2> delPointerDrag = null, UnityAction<Vector2> delPointerDragStart = null, UnityAction<Vector2> delPointerDragEnd = null)
    {
        m_delPointerClick = delPointerClick;    
        m_bDragable = bDragable;
        m_delPointerDrag = delPointerDrag;
        m_delPointerDragBegin = delPointerDragStart;
        m_delPointerDragEnd = delPointerDragEnd;
        SearchParentsDragReceiver();
    }
     
    //------------------------------------------------------------
    public void OnPointerDown(PointerEventData eventData)
    {
        m_bClickCancle = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
	{
        if (m_bClickCancle == false)
		{
			m_delPointerClick?.Invoke(eventData.pressPosition);
		}
	}

    public void OnInitializePotentialDrag(PointerEventData eventData)
	{
        m_delPointerPotentialDrag?.Invoke(eventData.position);
	}

    public void OnDrag(PointerEventData eventData)
	{
        m_bClickCancle = true;
        if (m_bDragable)
		{
            m_delPointerDrag?.Invoke(eventData.position);
		}
        else
		{
            m_pHandlerDrag?.OnDrag(eventData);
        }
    }

    public void OnDrop(PointerEventData eventData)
	{
        m_delPointerDrop?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
	{
        if (m_bDragable)
		{
            m_delPointerDragEnd?.Invoke(eventData.position);
        }
        else
		{
            m_pHandlerDragEnd?.OnEndDrag(eventData);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
	{        
        if (m_bDragable)
		{
            m_delPointerDragBegin?.Invoke(eventData.position);
        }
        else
		{
            m_pHandlerDragBegin?.OnBeginDrag(eventData);
        }
    }
    //-----------------------------------------------------------
    private void SearchParentsDragReceiver()
	{
        Transform parents = transform;
        bool bBreak = true;
        while(bBreak)
		{
            parents = parents.parent;
            if (parents == null) break;

            m_pHandlerDragBegin = parents.gameObject.GetComponent<IBeginDragHandler>();
            if (m_pHandlerDragBegin != null) bBreak = false;
            
            m_pHandlerDrag = parents.gameObject.GetComponent<IDragHandler>();
            if (m_pHandlerDrag != null) bBreak = false;

            m_pHandlerDragEnd = parents.gameObject.GetComponent<IEndDragHandler>();
            if (m_pHandlerDragEnd != null) bBreak = false;
        }
	}
}
