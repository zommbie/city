using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// CSerializableDictionary �̿��Ͽ� �ھ�� ������ ���̾��� �ν��Ͻ��� �ν����Ϳ� �����ϵ��� ����

public abstract class CUIWidgetTemplateListBase<TEMPLATE> : CUIEntryBase where TEMPLATE : IDictionary, new()
{
    [SerializeField]
    private TEMPLATE TemplateList = new TEMPLATE();
    //---------------------------------------------------
    protected override void OnUIEntryInitialize(CUIFrameBase pParentFrame)
    {
        base.OnUIEntryInitialize(pParentFrame);
       
    }
    //---------------------------------------------------
    protected CUIWidgetTemplateItemBase ProtTemplateListCloneInstance<TEnum>(TEnum eEnum, Transform pParent = null) where TEnum : Enum
    {
        CUIWidgetTemplateItemBase pClone = null;

        IDictionaryEnumerator it = TemplateList.GetEnumerator();
        while(it.MoveNext())
        {
            if ((int)it.Key == eEnum.GetHashCode())
            {
                CUIWidgetTemplateBase pTemplate = it.Value as CUIWidgetTemplateBase;
                if (pTemplate != null)
                {
                    pClone = pTemplate.DoUITemplateRequestItem(pParent);
                }
                break;
            }
        }
        return pClone;
    }

    protected void ProtTemplateListCloneReturnAll()
    {   

    }
  
    //-----------------------------------------------------
}
