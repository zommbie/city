using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UIScrollExportContents : CUIScrollRectBase // <CScroll Rect>�� ������ ��ũ�ѹ� ������Ʈ
{
    private List<UIScrollItemExportContent> m_listScrollItems = new List<UIScrollItemExportContent>();

    //--------------------------------------------------------------------------
    public void DoScrollExportContentsSetItemData(string strFileName, bool bHasChanged)
    {
        UIScrollItemExportContent pScrollItem = DoUITemplateRequestItem<UIScrollItemExportContent>(m_pScrollRect.content);
        pScrollItem.DoScrollItemExportContentSetData(strFileName, bHasChanged);
        m_listScrollItems.Add(pScrollItem);
    }

    public List<string> ExtractScrollExportContentsSelectedFileList() // ����ڰ� UI���� üũ�� ���� ���� ����Ʈ�� ��ȯ
    {
        List<string> pListSelectedExcelFile = new List<string>();
        for (int i = 0; i < m_listScrollItems.Count; i++)
        {
            if (m_listScrollItems[i].IsRefresh() == false)
                continue;
            string strFileName = m_listScrollItems[i].GetName();
            pListSelectedExcelFile.Add(strFileName);
        }
        return pListSelectedExcelFile;
    }

    public void DoScrollExportContentsShowNewOnly()
    {
        for (int i = 0; i < m_listScrollItems.Count; i++)
        {
            m_listScrollItems[i].DoScrollItemExportContentNewOnly();
        }
    }

    public void DoScrollExportContentsShowAllOff()
    {
        for (int i = 0; i < m_listScrollItems.Count; i++)
        {
            m_listScrollItems[i].DoScrollItemExportContentAllOff();
        }
    }

    public void DoScrollExportContentsShowAllOn()
    {
        for (int i = 0; i < m_listScrollItems.Count; i++)
        {
            m_listScrollItems[i].DoScrollItemExportContentAllOn();
        }
    }
}
