using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// 크기와 형태가 동일한 이미지의 토글 

public class CImageToggle : UnityEngine.UI.Image
{

    [SerializeField]
    private Sprite SpriteOff = null;
    [SerializeField]
    private Color ColorOff = Color.white;

    private Sprite m_pSpriteOn = null;
    private Color  m_rColorOn = Color.white;
    private bool m_bToggleOn = true;
    //----------------------------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        m_pSpriteOn = this.sprite;
        m_rColorOn = this.color;
    }

    //-----------------------------------------------------------------------------
    public void DoImageToggle(bool bOn)
    {
        PrivImageToggle(bOn);
    }

    public void DoImageToggle()
    {
        PrivImageToggle(!m_bToggleOn);
    }

    //--------------------------------------------------------------------------
    private void PrivImageToggle(bool bOn)
    {
        m_bToggleOn = bOn;
        if (m_bToggleOn)
        {
            sprite = m_pSpriteOn;
            color = m_rColorOn;
        }
        else
        {
            sprite = SpriteOff;
            color = ColorOff;
        }
    }

} 
