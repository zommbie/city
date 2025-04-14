using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// [개요]기본적인 이펙트의 요구 기능을 표현
// 1. 이펙트는 객체 하나로 독립 실행이 가능한 형태 (유닛 등에 삽입 되는 경우)
// 2. 이펙트의 리소스와 관리를 위해서는 CManagerEffect를 이용해서 삽입해야 함 

public abstract class CEffectBase : CPrefabTemplateItemBase
{
	[SerializeField]
	private bool DefaultHide = true;    

	private  bool  m_bActive = false;						 public bool IsActive { get { return m_bActive; } }
    private  bool  m_bInitialize = false;
    private  float m_fDuration = 0;                          public float p_Duration { get { return m_fDuration; } }
    private  float m_fDurationCurrent = 0;
    private float  m_fTimeScale = 1f;                       
    private  uint  m_hEffectID = 0;                          public uint GetEffectID() { return m_hEffectID; }

    protected Vector3 m_vecOriginPosition = Vector3.zero;
    protected Vector3 m_vecOriginScale = Vector3.zero;
    protected Vector3 m_vecOriginDirection = Vector3.zero;

    protected Vector3 m_vecDest = Vector3.zero;
	protected Vector3 m_vecOffset = Vector3.zero;

    protected Transform m_pTargetTransfrom = null;
    private UnityAction<CEffectBase> m_delEffectFinish = null;

    //-------------------------------------------------------------
    protected override void OnUnityAwake()
	{
		base.OnUnityAwake();
        PrivEffectInitialize();
	}

	private void Update()
	{
		if (m_bActive)
		{
            float fDelta = Time.deltaTime * m_fTimeScale;
			UpdateEffectDuration(fDelta);         
            OnEffectUpdate(fDelta);
		} 
	}

	//---------------------------------------------------------------
	internal void InterEffectInitialize()
	{
		PrivEffectInitialize();
	}

	public void DoEffectStartPosition(Vector3 vecPosition, Vector3 vecDirection = new Vector3(), float fDuration = 0, UnityAction<CEffectBase> delFinish = null, params object[] aParams)
	{
        PrivEffectReset(delFinish, fDuration);

        transform.position = vecPosition;       
        if (vecDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(vecDirection);
        }

		OnEffectStartPosition(vecPosition, vecDirection, aParams);
		OnEffectStartActivate(aParams);
	}

	public void DoEffectStartTarget(Transform pTarget, float fDuration, UnityAction<CEffectBase> delFinish = null, params object[] aParams)
	{
		PrivEffectReset(delFinish, fDuration);
        m_pTargetTransfrom = pTarget;

		OnEffectStartTarget(fDuration, pTarget, aParams);
		OnEffectStartActivate(aParams);
	}

    public void DoEffectStartDest(Vector3 vecDest, float fDuration = 0, UnityAction<CEffectBase> delFinish = null, params object[] aParams)
    {
        PrivEffectReset(delFinish, fDuration);

        m_vecDest = vecDest;

        OnEffectStartDest(vecDest, fDuration, aParams);
		OnEffectStartActivate(aParams);
	}

	public void DoEffectStart(UnityAction<CEffectBase> delFinish = null, float fDuration = 0f, params object[] aParams)
	{
		PrivEffectReset(delFinish, fDuration);
		OnEffectStart(fDuration, aParams);
		OnEffectStartActivate(aParams);
	}

    public void DoEffectStartDirection(Vector3 vecDirection, float fLength, float fDuration = 0, UnityAction<CEffectBase> delFinish = null, params object[] aParams)
    {
        PrivEffectReset(delFinish, fDuration);
        transform.rotation.SetLookRotation(vecDirection); 
        OnEffectStartDirection(vecDirection, fLength, fDuration, aParams);
		OnEffectStartActivate(aParams);
	}

    public void DoEffectEnd(bool bForce = false)
	{
		m_bActive = false;
		SetMonoActive(false);

        if (bForce == false)
        { 
            m_delEffectFinish?.Invoke(this);
        }
        ProtPrefabTemplateReturn();
        OnEffectEnd(bForce);
	}

    public void DoEffectShowHide(bool bShow) // 이펙트의 출력만 정지됨(활성화는 유지)
    {
        OnEffectShowHide(bShow);
    }

    public void DoEffectTimeScale(float fTimeScale) 
    {
        m_fTimeScale = fTimeScale;
        OnEffectTimeScale(fTimeScale);
    }

    public string GetEffectName()
	{
		return gameObject.name;
	}

    public void SetEffectScale(float fScale)
    {
        Vector3 vecScale = m_vecOriginScale * fScale;      
        transform.transform.localScale = vecScale;
    }

    public void SetEffectID(uint hEffectID)
    {
        m_hEffectID = hEffectID;
    }

	//-------------------------------------------------------------

	private void PrivEffectReset(UnityAction<CEffectBase> delFinish, float fDuration)
	{      
        m_vecDest = Vector3.zero; 
		m_vecOffset = Vector3.zero;

        m_pTargetTransfrom = null;

        transform.localPosition = m_vecOriginPosition;
        transform.localRotation = Quaternion.LookRotation(m_vecOriginDirection);
        transform.localScale = m_vecOriginScale;
		
		m_fDuration = fDuration;
		m_fDurationCurrent = 0;

		m_delEffectFinish = delFinish;

        m_bActive = true;
        SetMonoActive(true);
    }

    private void PrivEffectInitialize()
    {
        if (m_bInitialize) return;
        m_bInitialize = true;

		if (DefaultHide && m_bActive == false) SetMonoActive(false);

		m_vecOriginScale = transform.localScale;
        m_vecOriginPosition = transform.localPosition;
        m_vecOriginDirection = transform.rotation * Vector3.forward;
              
        RemoveCloneObjectName(gameObject);       
        OnEffectInitialize();
    }

    //--------------------------------------------------------------
    private void UpdateEffectDuration(float fDelta)
	{
		if (m_fDuration == 0) return;	

		m_fDurationCurrent += fDelta;
		if (m_fDurationCurrent >= m_fDuration)
		{
			m_fDurationCurrent = 0;
			DoEffectEnd();
		}
	} 

	//---------------------------------------------------------------
	protected virtual void OnEffectInitialize() { }
    protected virtual void OnEffectUpdate(float fDelta) { }
    protected virtual void OnEffectEnd(bool bForce) { }
    protected virtual void OnEffectShowHide(bool bShow) { }
    protected virtual void OnEffectStartActivate(params object[] aParams) { }
    protected virtual void OnEffectTimeScale(float fTimeScale) { }
	protected virtual void OnEffectStart(float fDuration, params object[] aParams) { }
	protected virtual void OnEffectStartPosition(Vector3 vecPosition, Vector3 vecDirection, params object[] aParams) { }
	protected virtual void OnEffectStartTarget(float fDuration, Transform pTarget, params object[] aParams) { }
    protected virtual void OnEffectStartDest(Vector3 vecDest, float fDuration, params object[] aParams) { }
    protected virtual void OnEffectStartDirection(Vector3 vecDirection, float fLength, float fDuration, params object[] aParams) { }

}
