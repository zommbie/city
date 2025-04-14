using UnityEngine;

public class CTMPSizeFitter : CMonoBase
{
    [SerializeField]
    private Vector2          SizeOffset;
    [SerializeField]
    private bool SizeWidth = true;
    [SerializeField]
    private bool SizeHeight = false;


    [SerializeField]
    private CTextMeshProUGUI TMPInstance;

    private RectTransform m_pRectTransform = null;
    //-----------------------------------------------
    protected override void OnUnityAwake()
    {
        m_pRectTransform = transform as RectTransform;
    }

    //-------------------------------------------------
    public void OnEnable()
    {       
        TMPInstance?.SetTMPSubScriptTextEvent(HandleTMPSizeFitterTextEvent);
    }

    public void OnDisable()
    {
        TMPInstance?.SetTMPUnSubScriptTextEvent(HandleTMPSizeFitterTextEvent);
    }

    //-------------------------------------------------------------------
    private void PrivTEMPSizeFitterReSizing()
    {
        Vector2 vecSize = m_pRectTransform.sizeDelta;
        if(SizeWidth)
        {
            vecSize.x = TMPInstance.preferredWidth + SizeOffset.x;
        }
        if (SizeHeight)
        {
            vecSize.y = TMPInstance.preferredHeight + SizeOffset.y;
        }

        m_pRectTransform.sizeDelta = vecSize;
    }

    private void HandleTMPSizeFitterTextEvent()
    {
        PrivTEMPSizeFitterReSizing();
    }
}
