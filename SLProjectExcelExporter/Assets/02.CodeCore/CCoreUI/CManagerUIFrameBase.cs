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
	private Vector2 m_vecScreenSize = Vector3.zero;							public Vector2 GetUIScreenSize() { return m_vecScreenSize; }
	private bool m_bInitialize = false;										public bool IsInitialize { get { return m_bInitialize; } }
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
	public void InterUIManagerInitialize()
	{
		PrivMgrUIFrameInitialize();
	}

    //-----------------------------------------------------------------
    public bool IsPointOverUIWidget()
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

		OnUIMgrInitializeCanvas(m_pRootCanvas);
		GlobalManagerUISceneLoaded();       
    }

	private void PrivMgrUIFrameInitilizeDefaultComponent()
	{
		m_pRootCanvas = GetComponent<Canvas>();
		m_pRootCanvasScaler = GetComponent<CanvasScaler>();
		m_pEventSystem = GetComponent<EventSystem>();
		m_pStandAloneInputModule = GetComponent<StandaloneInputModule>();

		//ToDo : 스크린 사이즈 대응 
		//m_pRootCanvasScaler.matchWidthOrHeight
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
		Dictionary<string, CUIFrameBase>.ValueCollection.Enumerator it = m_mapUIFrameInstance.Values.GetEnumerator();
		while (it.MoveNext())
		{
			it.Current.InterUIFrameScriptDataLoaded();
		}
	}

	//--------------------------------------------------------------------
	protected virtual void OnUIMgrInitializeCanvas(Canvas pRootCanvas) { }
} 
