using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class CUIWidgetTextPrinterBase : CUIWidgetBase
{
    [SerializeField]
    private float PrintSpeed = 0.2f; public float GetTextPrinterSpeed() { return PrintSpeed; }

    [SerializeField]
    private class STextPrintPageInfo
    {
        public List<char> CharList = new List<char>(32);
        public int CharCurrent = 0;
        public int PageIndex = 0;
    }

    private bool m_bUpdate = false;
    private float m_fDurationCurrent = 0;
    private STextPrintPageInfo m_pPageCurrent = null;
    private StringBuilder m_pNote = new StringBuilder();

    private int m_iPageIndexCurrent = 0; public int GetTextPrinterPageCurrent() { return m_iPageIndexCurrent; }
    private List<STextPrintPageInfo> m_listTextPage = new List<STextPrintPageInfo>(); public int GetTextTypeWritePageTotal() { return m_listTextPage.Count; }
    private TextMeshProUGUI m_pTMPControl = null;
    public string Text { set { PrivTextPrinterPageInput(value); PrivTextPrinterPageStart(0); } get { return m_pTMPControl.text; } }
    //----------------------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
        m_pTMPControl = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (m_bUpdate)
        {
            UpdateTextPrinter(Time.deltaTime);
        }
    }

    protected override void OnUIWidgetReset()
    {
        base.OnUIWidgetReset();
        PrivTextPrinterReset();
    }

    //-----------------------------------------------------------------
    protected void ProtTextPrinterPageTextInput(string strText)
    {
        if (strText == null || strText == string.Empty)
        {
            return;
        }

        PrivTextPrinterPageInput(strText);
    }

    protected void SetTextPrinterSpeed(float fPrintSpeed)
    {
        PrintSpeed = fPrintSpeed;
    }

    protected void SetTextPrinterUpdateEnable(bool bEnable)
    {
        m_bUpdate = bEnable;
    }

    protected void ProtTextPrinterPageAll(int iPageIndex)
    {
        m_pPageCurrent = null;
        PrivTextPrinterPageAll(iPageIndex);
    }

    protected void ProtTextPrinterPageStart(int iPageIndex)
    {
        PrivTextPrinterPageStart(iPageIndex);
    }

    protected float ProtTextPrinterPageSizeHeight(string strText)
    {
        float fSizeHeight = 0;
        m_pTMPControl.text = strText;
        fSizeHeight = m_pTMPControl.preferredHeight;
        m_pTMPControl.text = null;
        return fSizeHeight;
    }

    protected Vector2 GetTextPrinterPageLastestRightBottomPosition()
    {
        Vector2 vecCornerPosition = Vector2.zero;
        int iTotalLine = m_pTMPControl.textInfo.lineCount - 1;
        if (iTotalLine >= 0)
        {
            TMP_LineInfo rLineInfo = m_pTMPControl.textInfo.lineInfo[iTotalLine];
            vecCornerPosition.x = rLineInfo.lineExtents.max.x;
            vecCornerPosition.y = rLineInfo.lineExtents.min.y;
        }


        return vecCornerPosition;
    }

    //------------------------------------------------------------------
    private void PrivTextPrinterPageInput(string strText)
    {
        PrivTextPrinterReset();
        PrivTextPrinterMakePage(strText);
        OnTextPrinterInput(strText);
    }

    private void PrivTextPrinterReset()
    {
        m_pTMPControl.text = "";

        m_iPageIndexCurrent = 0;
        m_pPageCurrent = null;
        m_listTextPage.Clear();
    }

    private void PrivTextPrinterMakePage(string strText)
    {
        STextPrintPageInfo pNewPage = new STextPrintPageInfo();
        m_listTextPage.Add(pNewPage);

        for (int i = 0; i < strText.Length; i++)
        {
            m_pTMPControl.text += strText[i];
            if (CheckTextPrinterCharOverBound())
            {
                pNewPage = new STextPrintPageInfo();
                pNewPage.CharList.Add(strText[i]);
                pNewPage.PageIndex = m_listTextPage.Count;
                m_listTextPage.Add(pNewPage);
                m_pTMPControl.text = null;
                m_pTMPControl.text += strText[i];
            }
            else
            {
                pNewPage.CharList.Add(strText[i]);
            }
        }

        m_pTMPControl.text = null;
    }

    private void UpdateTextPrinter(float fDelta)
    {
        if (m_pPageCurrent == null) return;

        m_fDurationCurrent -= fDelta;
        if (m_fDurationCurrent <= 0)
        {
            m_fDurationCurrent = PrintSpeed;
            RecursiveTextPrinterPageChar(m_pPageCurrent);
        }
    }

    private void RecursiveTextPrinterPageChar(STextPrintPageInfo pTextPage)
    {
        if (pTextPage.CharCurrent < pTextPage.CharList.Count)
        {
            char cChar = pTextPage.CharList[pTextPage.CharCurrent];
            pTextPage.CharCurrent++;
            m_pTMPControl.text += cChar;

            if (cChar == ' ') // 공백일 경우 타이핑을 할 경우 답답하므로 즉시 출력한다
            {
                RecursiveTextPrinterPageChar(pTextPage);
            }
        }
        else
        {
            PrivTextPrinterPageEnd(pTextPage.PageIndex);
        }
    }

    private void PrivTextPrinterPageAll(int iIndex)
    {
        if (iIndex < 0 || iIndex >= m_listTextPage.Count) return;

        m_pNote.Clear();
        STextPrintPageInfo pPage = m_listTextPage[iIndex];
        for (int i = 0; i < pPage.CharList.Count; i++)
        {
            m_pNote.Append(pPage.CharList[i]);
        }
        string strPageAll = m_pNote.ToString();
        m_pTMPControl.GetTextInfo(strPageAll);  // 해주지 않으면 텍스트를 넣어도 포지션 정보가 갱신되지 않는다.
        m_pTMPControl.text = strPageAll;
        
        OnTextPrinterPageStart(iIndex, GetTextTypeWritePageTotal());
        OnTextPrinterPageEnd(iIndex, GetTextTypeWritePageTotal());
    }

    private void PrivTextPrinterPageStart(int iIndex)
    {
        if (iIndex < 0 || iIndex >= m_listTextPage.Count)
        {
            OnTextPrinterPageOver();       
            return;
        }
        m_bUpdate = true;
        m_pTMPControl.text = null;
        m_pPageCurrent = m_listTextPage[iIndex];
        m_pPageCurrent.CharCurrent = 0;
        m_iPageIndexCurrent = m_pPageCurrent.PageIndex;
        OnTextPrinterPageStart(iIndex, GetTextTypeWritePageTotal());
    }

    private void PrivTextPrinterPageEnd(int iIndex)
    {
        m_pPageCurrent = null;
        OnTextPrinterPageEnd(iIndex, GetTextTypeWritePageTotal());
    }
  
    private bool CheckTextPrinterCharOverBound()
    {
        bool bOverBound = false;

        Vector2 vecBound = GetUISize();
        if (vecBound.y < m_pTMPControl.preferredHeight)
        {
            bOverBound = true;           
        }
        return bOverBound;
    }

    //---------------------------------------------------------------------
    protected virtual void OnTextPrinterInput(string strText) { }
    protected virtual void OnTextPrinterPageStart(int iPageIndex, int iTotalPage) { }
    protected virtual void OnTextPrinterPageEnd(int iPageIndex, int iTotalPage) { }
    protected virtual void OnTextPrinterPageOver() { }
   
}
