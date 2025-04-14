using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(CImage))]
public abstract class CUIWidgetGridImageItemBase : CUIWidgetTemplateItemBase
{
    private CImage m_ItemImage;
    //----------------------------------------------------------------------------

    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        m_ItemImage = GetComponent<CImage>();
        m_ItemImage.type = Image.Type.Tiled;
        
    }

}