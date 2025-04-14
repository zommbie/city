using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetGridImageBase : CUIWidgetTemplateBase
{
    [SerializeField]
    private float LineThickness = 1f;
    [SerializeField]
    private bool UseVerticalImage = true; //세로로 된 이미지 사용하는 경우 true

    private int m_iCountX;
    private int m_iCountY;

    private List<CUIWidgetGridImageItemBase> m_listUIGridItems = new List<CUIWidgetGridImageItemBase>();

    //--------------------------------------------------------------------------

    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
    }

    //--------------------------------------------------------------------------

    protected void ProtGridImageSetting(int iCountX, int iCountY)
    {
        m_iCountX = iCountX;
        m_iCountY = iCountY;

        PrivGridImageReset();
        PrivGridImageAddItems();
    }

    //--------------------------------------------------------------------------

    private void PrivGridImageReset()
    {
        for (int i = 0; i < m_listUIGridItems.Count; i++)
        {
            m_listUIGridItems[i].DoUITemplateItemReturn();
        }
        m_listUIGridItems.Clear();
    }

    private void PrivGridImageAddItems()
    {
        RectTransform pRect = GetUIEntryRectTransform();
        float fSizeOfHorizontal = pRect.rect.width - LineThickness;
        float fSizeOfVertical = pRect.rect.height - LineThickness;
        float fSpaceHorizontal = fSizeOfHorizontal / m_iCountX;
        float fSpaceVertical = fSizeOfVertical / m_iCountY;

        if (UseVerticalImage)
        {
            for (int i = 0; i <= m_iCountX; i++)
            {
                CUIWidgetGridImageItemBase pGridItem = PrivGridImageMakeItem();

                Vector2 vecPositionOfItem = new Vector2(i * fSpaceHorizontal, 0);
                Vector2 vecSizeOfItem = new Vector2(LineThickness, fSizeOfVertical);
                pGridItem.SetUISize(vecSizeOfItem);
                pGridItem.SetUIPosition(vecPositionOfItem);

                m_listUIGridItems.Add(pGridItem);
            }

            for (int i = 0; i <= m_iCountY; i++)
            {
                CUIWidgetGridImageItemBase pGridItem = PrivGridImageMakeItem();

                Vector2 vecPositionOfItem = new Vector2(0, -i * fSpaceVertical - LineThickness);
                Vector2 vecSizeOfItem = new Vector2(LineThickness, fSizeOfHorizontal);
                pGridItem.transform.localRotation = Quaternion.Euler(0, 0, 90);
                pGridItem.SetUISize(vecSizeOfItem);
                pGridItem.SetUIPosition(vecPositionOfItem);

                m_listUIGridItems.Add(pGridItem);

            }
        }
        else
        {
            for (int i = 0; i <= m_iCountX; i++)
            {
                CUIWidgetGridImageItemBase pGridItem = PrivGridImageMakeItem();

                Vector2 vecPositionOfItem = new Vector2(i * fSpaceHorizontal + LineThickness, 0);
                Vector2 vecSizeOfItem = new Vector2(fSizeOfVertical, LineThickness);
                pGridItem.transform.localRotation = Quaternion.Euler(0, 0, -90);
                pGridItem.SetUISize(vecSizeOfItem);
                pGridItem.SetUIPosition(vecPositionOfItem);

                m_listUIGridItems.Add(pGridItem);
            }

            for (int i = 0; i <= m_iCountY; i++)
            {
                CUIWidgetGridImageItemBase pGridItem = PrivGridImageMakeItem();

                Vector2 vecPositionOfItem = new Vector2(0, -i * fSpaceVertical);
                Vector2 vecSizeOfItem = new Vector2(fSizeOfHorizontal, LineThickness);
                pGridItem.SetUISize(vecSizeOfItem);
                pGridItem.SetUIPosition(vecPositionOfItem);

                m_listUIGridItems.Add(pGridItem);

            }
        }
    }

    private CUIWidgetGridImageItemBase PrivGridImageMakeItem()
    {
        CUIWidgetGridImageItemBase pGridItem = DoUITemplateRequestItem<CUIWidgetGridImageItemBase>(transform);
        pGridItem.SetUIAnchor(EAnchorPresets.TopLeft);
        pGridItem.SetUIPivot(EPivotPresets.TopLeft);
        return pGridItem;
    }
}