using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollItemExportContent : CUIWidgetTemplateItemBase // ��ũ�ѹ� ���빰 ������ ������Ʈ
{
    [SerializeField] private UIToggleCheck  ItemToggle;
    [SerializeField] private CText          ItemName;

    private bool    m_bNewFile; // ���ŵ� �����ΰ�?

    public bool IsRefresh() { return ItemToggle.IsToggleOn(); }
    public string GetName() { return ItemName.text; }
    
    //-----------------------------------------------------------
    public void DoScrollItemExportContentSetData(string strFileName, bool bHasChanged)
    {
        ItemName.text       = strFileName;          // UI�� ǥ�õ� ���ϸ�
        m_bNewFile          = bHasChanged;          // ���� ���� ����

        PrivScrollItemExportContentToggleItem(m_bNewFile);  // UI�� ó�� ǥ�õǴ°� New Only
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
            ItemToggle.DoButtonToggleOn();          // ��ũ�ѹ� ���빰 ������ üũ�ڽ� üũ On
        }
        else
        {
            ItemToggle.DoButtonToggleOff();         // ��ũ�ѹ� ���빰 ������ üũ�ڽ� üũ Off
        }
    }
}
