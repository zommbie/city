using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CEffectBase : CPrefabTemplateItemBase
{
	[SerializeField]
	private bool DefaultHide = true;

	private  bool m_bActive = false;						public bool IsActive { get { return m_bActive; } }
    private  bool m_bInitialize = false;
	private  UnityAction m_delEffectFinish = null;	
    private Transform m_pTransformOriginParent = null;      public Transform GetEffectParentOrigin() { return m_pTransformOriginParent; }

	private Vector3 m_vecOrigin = Vector3.zero;
	private Vector3 m_vecDest = Vector3.zero;
	private Vector3 m_vecOffset = Vector3.zero;
    private Vector3 m_vecScaleOrigin = Vector3.zero;
	private Quaternion m_quaRotationOrigin = Quaternion.identity;

	private float m_fDuration = 0;          public float p_Duration { get { return m_fDuration; } }
	private float m_fDurationCurrent = 0;

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
			UpdateEffectDuration(Time.deltaTime);
			OnEffectUpdate(Time.deltaTime);
		} 
	}

	//---------------------------------------------------------------
	internal void InterEffectInitialize()
	{
		PrivEffectInitialize();
	}

	public void DoEffectStartPosition(Vector3 vecPosition, Vector3 vecRotation, float fDuration = 0, float fLocalScale = 0f , UnityAction delFinish = null, params object[] aParams)
	{
        PrivEffectReset(delFinish, fLocalScale, fDuration);

		m_vecOrigin = vecPosition;
		m_quaRotationOrigin = gameObject.transform.rotation;

		transform.position = vecPosition;

        if (vecRotation != Vector3.zero)
        {
            vecRotation += vecPosition;
            transform.LookAt(vecRotation);
        }

		OnEffectStartSinglePosition(aParams);
		OnEffectActivate();
	}

	public void DoEffectStartTarget(Transform pTarget, float fDuration, UnityAction delFinish = null, params object[] aParams)
	{
		PrivEffectReset(delFinish, 0, fDuration);		
		OnEffectStartTarget(fDuration, pTarget, aParams);
		OnEffectActivate();
	}

    public void DoEffectStartDest(Vector3 vecDest, float fDuration = 0, float fLocalScale = 1f, UnityAction delFinish = null, params object[] aParams)
    {
        PrivEffectReset(delFinish, 0, fDuration);
        OnEffectStartDest(vecDest, fDuration, aParams);
		OnEffectActivate();
	}

	public void DoEffectStart(UnityAction delFinish = null, float fDuration = 0f, params object[] aParams)
	{
		PrivEffectReset(delFinish, 0, fDuration);
		OnEffectStart(fDuration, aParams);
		OnEffectActivate();
	}

    public void DoEffectStartDirection(Vector3 vecDirection, float fLength, float fDuration = 0, UnityAction delFinish = null, params object[] aParams)
    {
        PrivEffectReset(delFinish, 0, fDuration);
        transform.forward = vecDirection;
        OnEffectStartDirection(vecDirection, fLength, fDuration, aParams);
		OnEffectActivate();
	}

    public void DoEffectEnd(bool bForce = false)
	{
		m_bActive = false;
		SetMonoActive(false);
		ProtPrefabTemplateReturn();
		m_delEffectFinish?.Invoke();
		OnEffectEnd(bForce);
	}


	public string GetEffectName()
	{
		return gameObject.name;
	}

    public void SetEffectScale(float fScale)
    {
        Vector3 vecScale = m_vecScaleOrigin;
        vecScale.x *= fScale;
        vecScale.y *= fScale;
        vecScale.z *= fScale;
        transform.transform.localScale = vecScale;
    }

	//-------------------------------------------------------------

	private void PrivEffectReset(UnityAction delFinish, float fLocalScale, float fDuration)
	{
        PrivEffectInitialize();

        m_bActive = true;

		m_vecOrigin = Vector3.zero;
		m_vecDest = Vector3.zero; 
		m_vecOffset = Vector3.zero;
		m_quaRotationOrigin = Quaternion.identity;

		m_fDuration = fDuration;
		m_fDurationCurrent = 0;

		m_delEffectFinish = delFinish;
		SetMonoActive(true);


        if (fLocalScale != 0)
        {
            transform.localScale = new Vector3(fLocalScale, fLocalScale, fLocalScale);
        }
        else
        {
            transform.localScale = m_vecScaleOrigin;
        }
    }

    private void PrivEffectInitialize()
    {
        if (m_bInitialize) return;
        m_bInitialize = true;

		if (DefaultHide) SetMonoActive(false);

		m_vecScaleOrigin = transform.localScale;
        m_vecOffset = transform.position;
        m_pTransformOriginParent = transform.parent;
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
	protected virtual void OnEffectActivate() { }
	protected virtual void OnEffectStart(float fDuration, params object[] aParams) { }
	protected virtual void OnEffectStartSinglePosition(params object[] aParams) { }
	protected virtual void OnEffectStartTarget(float fDuration, Transform pTarget, params object[] aParams) { }
    protected virtual void OnEffectStartDest(Vector3 vecDest, float fDuration, params object[] aParams) { }
	protected virtual void OnEffectEnd(bool bForce) { }
    protected virtual void OnEffectStartDirection(Vector3 vecDirection, float fLength, float fDuration, params object[] aParams) { }
	protected virtual void OnEffectUpdate(float fDelta) { }
}
