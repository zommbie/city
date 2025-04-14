using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUIWidgetNumberSpriteChildBase : CUIWidgetBase
{
    [SerializeField]
    private CImage NumberImage = null;

    private int m_iNumber = 0;      public int GetImgageNumber() { return m_iNumber; }
    //--------------------------------------------------------------
    public void DoUIWidgetNumberImageChildOn(Sprite pSprite, int iNumber)
    {
        NumberImage.sprite = pSprite;
        gameObject.SetActive(false);
        m_iNumber = iNumber;
        OnUINumberImageChildOn();
    }

    public void DoUIWidgetNumberImageChildOff()
    {
        gameObject.SetActive(false);
        OnUINumberImageChildOff();
    }


    //-------------------------------------------------------------
    protected virtual void OnUINumberImageChildOn() { }
    protected virtual void OnUINumberImageChildOff() { }
}
