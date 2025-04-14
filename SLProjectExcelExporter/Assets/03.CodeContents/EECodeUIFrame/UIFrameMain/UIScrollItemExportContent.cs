using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollItemExportContent : CUIWidgetTemplateItemBase // 스크롤바 내용물 프리팹 오브젝트
{
    [SerializeField] private UIToggleCheck  ItemToggle;
    [SerializeField] private CText          ItemName;

    private bool    m_bNewFile; // 갱신된 파일인가?

    public bool IsRefresh() { return ItemToggle.IsToggleOn(); }
    public string GetName() { return ItemName.text; }
    
    //-----------------------------------------------------------
    public void DoScrollItemExportContentSetData(string strFileName, bool bHasChanged)
    {
        ItemName.text       = strFileName;          // UI에 표시될 파일명
        m_bNewFile          = bHasChanged;          // 파일 갱신 여부

        PrivScrollItemExportContentToggleItem(m_bNewFile);  // UI에 처음 표시되는건 New Only
    }
    public void DoScrollItemExportContentNewOnly()
    {
        PrivScrollItemExportContentToggleItem(m_bNewFile);
    }
    public void DoScrollItemExportContentAllOff()
    {
        PrivScrollItemExportContentToggleItem(false);
    }
    public void DoScrollItemExportContentAllOn()
    {
        PrivScrollItemExportContentToggleItem(true);
    }

    //-----------------------------------------------------------
    private void PrivScrollItemExportContentToggleItem(bool bToggleOn)
    {
        if (bToggleOn == true)
        {
            ItemToggle.DoButtonToggleOn();          // 스크롤바 내용물 프리펩 체크박스 체크 On
        }
        else
        {
            ItemToggle.DoButtonToggleOff();         // 스크롤바 내용물 프리펩 체크박스 체크 Off
        }
    }
}
