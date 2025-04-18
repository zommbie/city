using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine;

abstract public class CStageCameraBase : CMonoBase
{
	[SerializeField]
	private bool Exclusive = true;   public bool IsStageCameraExclusive() { return Exclusive; } // 독점 카메라로 활성화 되면 다른 카메라는 꺼진다.
	[SerializeField]
	private bool StartShow = true;

    protected Camera m_pCamera = null;	
	private UniversalAdditionalCameraData m_pURPCameraData = null;
    private bool m_bInitialize = false;
	private int m_iCameraCullingMask = 0;		
	//--------------------------------------------------------------------
    internal void InterStageCameraInitialize()
    {
        if (m_bInitialize) return;

        m_bInitialize = true;
        m_pCamera = GetComponent<Camera>();
        m_iCameraCullingMask = m_pCamera.cullingMask;
        m_pURPCameraData = GetComponent<UniversalAdditionalCameraData>();
        PrivStageCameraMountUICamera();

        OnStageCameraInitialize();
    }

	internal void InterStageCameraHide()
	{
		SetMonoActive(false);
		OnStageCameraHide();
	}

	internal void InterStageCameraShow()
	{
		SetMonoActive(true);
		OnStageCameraShow();
	}

	internal void InterStageCameraCullingMaskDisable()  // 카메라는 존재하나 랜더를 하지 않는다 (오버레이만 랜더하고 싶을때)
	{
		m_iCameraCullingMask = m_pCamera.cullingMask;
		m_pCamera.cullingMask = 0;
		OnStageCameraCullingMaskDisable();
	}

	internal void InterStageCameraCullingMaskEnable()
	{
		m_pCamera.cullingMask = m_iCameraCullingMask;
		OnStageCameraCullingMaskEnable();
	}

    internal void InterStageCameraUISceneLoaded()
    {
        OnStageCameraUISceneLoaded();
    }

	//----------------------------------------------------------------
	private void PrivStageCameraMountUICamera()
	{
        if (StartShow)
        {
            Camera pUICamera = CManagerUIFrameBase.Instance.GetUIManagerCamara();
            if (pUICamera != null)
            {
                PrivStageCameraOverlayCamera(pUICamera);
            }
        }		
    }

    private void PrivStageCameraOverlayCamera(Camera pOverlayCamera)
    {
        if (m_pURPCameraData.cameraStack.Contains(pOverlayCamera) == true) return;

        List<Camera> StackCameraList = new List<Camera>();
        StackCameraList.Add(pOverlayCamera);

        for (int i = 0; i < m_pURPCameraData.cameraStack.Count; i++)
        {
            StackCameraList.Add(m_pURPCameraData.cameraStack[i]);
        }

        m_pURPCameraData.cameraStack.Clear();

        for (int i = 0; i < StackCameraList.Count; i++)
        {
            m_pURPCameraData.cameraStack.Add(StackCameraList[i]);
        }
        OnStageCameraOverlayStack(pOverlayCamera);
    }

    //------------------------------------------------------------------
    public Camera GetCamera()
    {
        if (m_pCamera == null)
        {
            m_pCamera = GetComponent<Camera>();
        }
        return m_pCamera; 
    }

    public int GetCameraID() 
    {
        return GetCamera().GetInstanceID(); 
    }
	
    //-----------------------------------------------------------------------
	protected virtual void OnUnityUpdate(float fDeltaTime) { }
	protected virtual void OnUnityLateUpdate() { }
    protected virtual void OnUnityFixedUpdate(float fFixedDeltaTime) { }
   
	protected virtual void OnStageCameraHide() { }
	protected virtual void OnStageCameraShow() { }
	protected virtual void OnStageCameraOverlayStack(Camera pOverlayCamera) { }	
	protected virtual void OnStageCameraCullingMaskEnable() { }
	protected virtual void OnStageCameraCullingMaskDisable() { }
    protected virtual void OnStageCameraRemove() { }
    protected virtual void OnStageCameraInitialize() { }
    protected virtual void OnStageCameraUISceneLoaded() { }
}
