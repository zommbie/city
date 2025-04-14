using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public abstract class CUIWidgetTextDisplayBase : CUIWidgetTemplateBase, CUIWidgetTextDisplayBase.IEventTextDisplay
{
    public interface IEventTextDisplay
    {
        public void ITextDisplayNext(CUIWidgetTextDisplayItemBase pInstance, int iIndex);  // 다음 텍스트를 요청. 객체는 아직 디스플레이중
        public void ITextDisplayShowEnd(CUIWidgetTextDisplayItemBase pInstance, int iIndex);   // 객체의 디스플레이가 완전히 종료
    }

    public enum ETextDisplayType
    {
        None,
        MoveSlideLeft,      // 왼쪽으로 간격만큼 이동한다.
        StopSlidePingPong,  // 텍스트 크기가 클 경우 좌우로 왔다 갔다 한다. 좁은 공간에 긴 텍스트를 다 보여주는 역활
        StopNoMove,         // 아무것도 하지 않는다. 타이머등 객체 자체가 종료를 결정한다.  
    }

    private class STextDisplayData
    {
        public ETextDisplayType TextDisplayType = ETextDisplayType.None;
        public string TextMessage;
    }

    [SerializeField]
    private RectTransform RootDisplay = null;

    [SerializeField]
    private bool DisplayLoop = true;    // 모든 메시지를 출력하면 루프한다. false일 경우     

    [SerializeField]
    private float MoveSpeed = 60f;   // 초당 X픽셀만큼 이동 
    [SerializeField]
    private float MessageOffset = 10;  // 각 메시지의 간격 

    private bool  m_bDisplayStart = false;
    private int   m_iLastedDisplayIndex = -1;
    private Vector2 m_vecDisplaySizeWidth = Vector2.zero;
    private List<STextDisplayData>                      m_listTextDisplayData   = new List<STextDisplayData>();
    private CLinkedList<CUIWidgetTextDisplayItemBase>   m_listTextDisplayUpdate = new CLinkedList<CUIWidgetTextDisplayItemBase>();
    //-------------------------------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {       
        base.OnUIEntryInitialize(pParentFrame);
        m_vecDisplaySizeWidth = RootDisplay.rect.size;
    }

    private void Update()
    {
        if (m_bDisplayStart)
        {
            float fDelta = Time.deltaTime;
            UpdateTextDisplayItem(fDelta);
            OnTextDisplayUpdate(fDelta);
        }
    }
    //------------------------------------------------------------------------------------
    public void ITextDisplayNext(CUIWidgetTextDisplayItemBase pInstance, int iIndex)
    {
        PrivTextDisplayNextItem(iIndex + 1);
    }

    public void ITextDisplayShowEnd(CUIWidgetTextDisplayItemBase pInstance, int iIndex)
    {
        pInstance.DoUITemplateItemReturn();
        OnTextDisplayShowEnd(pInstance);
    }

    //-------------------------------------------------------------------------------------
    protected void ProtTextDisplayAddData(ETextDisplayType eTextDisplayType, string strTextMessage)
    {
        STextDisplayData pTextDisplayData = new STextDisplayData();
        pTextDisplayData.TextDisplayType  = eTextDisplayType;
        pTextDisplayData.TextMessage = strTextMessage;

        m_listTextDisplayData.Add(pTextDisplayData);
    }

    protected void ProtTextDisplayReset()
    {
        DoUITemplateReturnAll();
        m_bDisplayStart = false;
        m_iLastedDisplayIndex = -1;
        m_listTextDisplayData.Clear();
        m_listTextDisplayUpdate.Clear();
    }

    protected void ProtTextDisplayStart()
    {
        if (m_listTextDisplayData.Count == 0) return;
        if (m_bDisplayStart) return;

        m_bDisplayStart = true;
        PrivTextDisplayNextItem(0);
    }

    //------------------------------------------------------------------------------------
    private void PrivTextDisplayNextItem(int iNextIndex)
    {
        if (iNextIndex < 0) return;
     
        if (iNextIndex < m_listTextDisplayData.Count)
        {
            STextDisplayData pTextDisplayData = m_listTextDisplayData[iNextIndex];
            PrivTextDisplayStartItem(pTextDisplayData, iNextIndex);
        }
        else
        {
            if (DisplayLoop)
            {
                PrivTextDisplayNextItem(0);
            }
            else
            {
                PrivTextDisplayLoopEnd();
            }
        }
    }

    private void PrivTextDisplayStartItem(STextDisplayData pTextDisplayData, int iItemIndex)
    {
        Vector2 vecStartPosition = ExtractTextDisplayStartPosition();
        m_iLastedDisplayIndex = iItemIndex;
        CUIWidgetTextDisplayItemBase pDisplayItem = DoUITemplateRequestItem(RootDisplay.transform) as CUIWidgetTextDisplayItemBase;
        m_listTextDisplayUpdate.AddLast(pDisplayItem);
        pDisplayItem.InterTextDisplayStart(this, pTextDisplayData.TextDisplayType, pTextDisplayData.TextMessage, iItemIndex, MoveSpeed, vecStartPosition, m_vecDisplaySizeWidth);
    }

    private Vector2 ExtractTextDisplayStartPosition()
    {
        Vector2 vecStartPosition = Vector2.zero;
        if (m_listTextDisplayUpdate.Count > 0)
        {
            CUIWidgetTextDisplayItemBase pLastDisplayItem = m_listTextDisplayUpdate.Last.Value;
            vecStartPosition = pLastDisplayItem.GetUIPosition();
            vecStartPosition.x += pLastDisplayItem.GetTextDisplayTextWidth() + MessageOffset;
        }

        return vecStartPosition;
    }


    private void PrivTextDisplayLoopEnd()
    {
        m_bDisplayStart = false;
        OnTextDisplayLoopEnd();
    }

    //-------------------------------------------------------------------------------------
    private void UpdateTextDisplayItem(float fDelta)
    {
        CLinkedList<CUIWidgetTextDisplayItemBase>.Enumerator it = m_listTextDisplayUpdate.GetEnumerator();
        while (it.MoveNext())
        {
            CUIWidgetTextDisplayItemBase pDisplayItem = it.Current;
            if (pDisplayItem.UpdateTextDisplayItem(fDelta))
            {
                it.Remove();
            }
        }
    }

    //-------------------------------------------------------------------------------------
    protected virtual void OnTextDisplayUpdate(float fDelta) { }
    protected virtual void OnTextDisplayStart(int iItemIndex) { }
    protected virtual void OnTextDisplayLoopEnd() { }  // 마지막 객체가 Next를 호출하면 호출
    protected virtual void OnTextDisplayShowEnd(CUIWidgetTextDisplayItemBase pDisplayItem) { }   // 모든 객체가 화면에서 사라지면 호출
}
