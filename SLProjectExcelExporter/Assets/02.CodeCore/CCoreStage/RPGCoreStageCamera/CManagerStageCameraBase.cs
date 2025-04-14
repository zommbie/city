using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

abstract public class CManagerStageCameraBase : CManagerTemplateBase<CManagerStageCameraBase>
{
	private Dictionary<int, CStageCameraBase> m_mapCameraInstance = new Dictionary<int, CStageCameraBase>();
	private List<CStageCameraBase> m_listActiveCamera = new List<CStageCameraBase>();             
    private Camera m_pUICamera = null;                         
	//---------------------------------------------------------------
	protected override void OnManagerUISceneLoaded()
	{
        PrivStageCameraCollectInstance();
    }
	//----------------------------------------------------------------
	internal void InterStageCameraRegist(CStageCameraBase pCamera, bool bRegist)
	{
		if (bRegist)
		{
            PrivStageCameraRegist(pCamera);
		}
		else
		{
            PrivStageCameraUnRegist(pCamera);
		} 
	}

    internal void InterStageCameraActivate(CStageCameraBase pStageCamera)
    {        
        PrivStageCameraActivate(pStageCamera);
    }

	//------------------------------------------------------------------
	public void DoStageCameraRenderEnableAll(bool bEnable)
	{
        Dictionary<int, CStageCameraBase>.Enumerator it = m_mapCameraInstance.GetEnumerator();
        while(it.MoveNext())
        {
            if (bEnable)
            {
                it.Current.Value.InterStageCameraCullingMaskEnable();
            }
            else
            {
                it.Current.Value.InterStageCameraCullingMaskDisable();
            }
        }
    }

	//-----------------------------------------------------------------
    private Camera FindStageUICamera()
	{
        if (m_pUICamera == null)
		{
            if (CManagerUIFrameBase.Instance != null)
			{
                m_pUICamera = CManagerUIFrameBase.Instance.GetUIManagerCamara();
			}
		}

        return m_pUICamera;
	}

	private void PrivStageCameraRegist(CStageCameraBase pStageCamera)
	{
		int CameraID = pStageCamera.GetCameraID();
        if (m_mapCameraInstance.ContainsKey(CameraID) == false)
        {
			pStageCamera.InterStageCameraInitialize();
			m_mapCameraInstance[CameraID] = pStageCamera;
        }
    }

	private void PrivStageCameraUnRegist(CStageCameraBase pCamera)
	{
        int CameraID = pCamera.GetCameraID();
        if (m_mapCameraInstance.ContainsKey(CameraID))
        {
            pCamera.InterStageCameraRemove();
            m_listActiveCamera.Remove(pCamera);
            m_mapCameraInstance.Remove(CameraID);
        }
    }

    private void PrivStageCameraCollectInstance()
    {
        CStageCameraBase [] aStageCamera = FindObjectsByType<CStageCameraBase>(FindObjectsSortMode.None);
        for (int i = 0; i < aStageCamera.Length; i++)
        {           
            PrivStageCameraRegist(aStageCamera[i]);            
            if (i == 0)
            {
                PrivStageCameraActivate(aStageCamera[i]);
			}
        }
    }

    private void PrivStageCameraActivate(CStageCameraBase pStageCamera)
    {
        if (m_listActiveCamera.Find(x => x == pStageCamera) != null) return;
        m_listActiveCamera.Add(pStageCamera);

        PrivStageCameraExclusive(pStageCamera);
        PrivStageCameraOverlayStack(pStageCamera);
    }

    private void PrivStageCameraHide(CStageCameraBase pStageCameraShow)
	{
        for (int i = 0; i < m_listActiveCamera.Count; i++)
		{
            if (m_listActiveCamera[i] != pStageCameraShow)
			{
                m_listActiveCamera[i].InterStageCameraHide();
                OnStageCameraHide(m_listActiveCamera[i]);
			}
		}
	}

    private void PrivStageCameraOverlayStack(CStageCameraBase pStageCameraShow)
	{
        Camera pUICamera = FindStageUICamera();
        if (pUICamera != null)
		{
            pStageCameraShow.InterStageCameraOverlayStack(pUICamera);
		}
	}

    private void PrivStageCameraExclusive(CStageCameraBase pStageCamera)
	{
		bool bExclusive = pStageCamera.GetStageCameraExclusive();
		if (bExclusive)
		{
			PrivStageCameraHide(pStageCamera);
		}
	}

    //-------------------------------------------------------------------------------
    protected virtual void OnStageCameraShow(CStageCameraBase pStageCamera) { }
    protected virtual void OnStageCameraHide(CStageCameraBase cStageCamera) { }
}
