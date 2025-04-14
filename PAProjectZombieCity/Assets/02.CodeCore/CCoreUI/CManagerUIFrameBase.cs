using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(EventSystem))]
[RequireComponent(typeof(StandaloneInputModule))]
public abstract class CManagerUIFrameBase : CManagerTemplateBase<CManagerUIFrameBase>
{ 
	private Canvas					m_pRootCanvas = null; 
	private CanvasScaler			m_pRootCanvasScaler = null;
	private EventSystem				m_pEventSystem = null;
	private StandaloneInputModule	m_pStandAloneInputModule = null;
    private Vector3 m_vecRootCanverPosition = Vector3.zero;
	private Vector2 m_vecScreenSize = Vector3.zero;							public Vector2 GetUIScreenSize() { return m_vecScreenSize; }
	private bool m_bInitialize = false;										public bool IsInitialize { get { return m_bInitialize; } }
    private bool m_bScriptDataLoaded = false;
    private float m_fPixelPerUnit = 0;                                      public float GetUIPixelPerUnit() { return m_fPixelPerUnit; }
	private Dictionary<string, CUIFrameBase> m_mapUIFrameInstance = new Dictionary<string, CUIFrameBase>();
	//-------------------------------------------------------------------
	protected override void OnUnityStart()
	{
		base.OnUnityStart();		
		PrivMgrUIFrameInitialize();
	}

    protected override void OnManagerScriptDataLoaded()
    {
        PrivMgrUIFrameScriptDataLoaded();
    }

    //-------------------------------------------------------------------
    internal void InterMgrUIFrameResetScreenPosition()
    {
        transform.localPosition = Vector3.zero;
    }

    //-----------------------------------------------------------------
    public bool IsPointOver()
	{
		if (m_bInitialize == false) return false;

		return m_pEventSystem.IsPointerOverGameObject();
	}

    //----------------------------------------------------------------
    public Camera GetUIManagerCamara() 
	{
		if (m_pRootCanvas == null) return null;
		return m_pRootCanvas.worldCamera; 
	}

    public Vector3 GetUIRootPosition()
    {
        return m_vecRootCanverPosition;
    }

    public float GetUIRootPixelPerfect()
    {
        return m_pRootCanvas.referencePixelsPerUnit;
    }

    public Vector3 GetUIPositionScreenToWorld(Vector3 vecScreenPosition)
    {
        return GetUIManagerCamara().ScreenToWorldPoint(vecScreenPosition);
    }

    //-----------------------------------------------------------------
    public void SetUICameraEnable(bool bEnable)
    {
		if (m_pRootCanvas == null) return;
        
		if (bEnable)
        {
			m_pRootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        }
		else
        {
			m_pRootCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
    }

    public void SetUICameraOrthpGraphicSize(float fOrthoSize)
    {
        m_pRootCanvas.worldCamera.orthographicSize = fOrthoSize;
    }

	//-------------------------------------------------------------------
	protected void ProtMgrUIFrameLoad(CUIFrameBase pUIFrame)
	{
		string strUIFrameName = pUIFrame.GetType().Name;
		if (m_mapUIFrameInstance.ContainsKey(strUIFrameName))
		{
			//Error!
		}
		else
		{
			m_mapUIFrameInstance[strUIFrameName] = pUIFrame;
            pUIFrame.InterUIFrameInitialize();
        }

        if (pUIFrame.GetUIFrameFocusType() == CManagerUIFrameFocusBase.EUIFrameFocusType.Invisible)
		{
			pUIFrame.InterUIFrameShow(0);
		}
    }

    protected void ProtMgrUIFrameLoadDynamic(CUIFrameBase pUIFrame)
    {
        ProtMgrUIFrameLoad(pUIFrame);
        PrivMgrUIFrameInitializeChildPost();
    }

    protected CUIFrameBase FindUIFrame(string strUIFrame)
	{
		CUIFrameBase pFindUIFrame = null;
		if (m_mapUIFrameInstance.ContainsKey(strUIFrame))
		{
			pFindUIFrame = m_mapUIFrameInstance[strUIFrame];
		}
		return pFindUIFrame;
	}

    protected void ProtMgrUICanvasScreenMatchMode(CanvasScaler.ScreenMatchMode eScreenMatchMode)
    {
        m_pRootCanvasScaler.screenMatchMode = eScreenMatchMode;
    }

	//--------------------------------------------------------------------
	private void PrivMgrUIFrameInitialize()
	{
        // [주의] 이 단계에서 전체 UI의 사이즈가 디바이스에 맞게 픽스된다. 따라서 m_bInitialize 호출되기 이전에 UIFrame을 호출하면 안된다.
        if (m_bInitialize) return;
		m_bInitialize = true;

		PrivMgrUIFrameInitilizeDefaultComponent();
		PrivMgrUIFrameInitializeChild();
		PrivMgrUIFrameInitializeChildPost();

        RectTransform UIScreenSize = transform as RectTransform;
        m_vecScreenSize = UIScreenSize.sizeDelta;
        m_vecRootCanverPosition = transform.position;
        
		OnUIMgrInitializeCanvas(m_pRootCanvas);
		GlobalManagerUISceneLoaded();
        PrivMgrUIFrameScriptDataLoaded();
    }

	private void PrivMgrUIFrameInitilizeDefaultComponent()
	{
		m_pRootCanvas = GetComponent<Canvas>();
		m_pRootCanvasScaler = GetComponent<CanvasScaler>();
		m_pEventSystem = GetComponent<EventSystem>();
		m_pStandAloneInputModule = GetComponent<StandaloneInputModule>();
        m_fPixelPerUnit = m_pRootCanvasScaler.referencePixelsPerUnit;
	}

	private void PrivMgrUIFrameInitializeChild()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			CUIFrameBase pUIFrame = transform.GetChild(i).gameObject.GetComponentInChildren<CUIFrameBase>(true);
			if (pUIFrame)
			{
				ProtMgrUIFrameLoad(pUIFrame);
			}
		}
	}

	private void PrivMgrUIFrameInitializeChildPost()
    {
        Dictionary<string, CUIFrameBase>.ValueCollection.Enumerator it = m_mapUIFrameInstance.Values.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.InterUIFrameInitializePost();
        }
    }

    private void PrivMgrUIFrameScriptDataLoaded()
    {
        if (m_bScriptDataLoaded == false && IsLoadedScriptData)
        {
            m_bScriptDataLoaded = true;
            Dictionary<string, CUIFrameBase>.ValueCollection.Enumerator it = m_mapUIFrameInstance.Values.GetEnumerator();
            while (it.MoveNext())
            {
                it.Current.InterUIFrameScriptDataLoaded();
            }
        }
    }

	//--------------------------------------------------------------------
	protected virtual void OnUIMgrInitializeCanvas(Canvas pRootCanvas) { }
} 
