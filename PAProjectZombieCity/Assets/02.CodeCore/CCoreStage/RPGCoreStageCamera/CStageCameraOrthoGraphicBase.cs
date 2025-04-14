using UnityEngine;

// 다양한 휴대폰 화면 및 가로 세로 모드에 따른 비율 처리를 자동화 하기 위한 기능
// 직교 백터를 작은 변에 맞추면 큰 변이 비는 경우가 많은데 이때 사이즈를 조정해서 적당한 크기로 맞춘다

public abstract class CStageCameraOrthoGraphicBase : CStageCameraMovementBase
{
    [SerializeField]
    private bool ScreenAutoFit = true;              

    private const float c_OrthoScreenWidth = 1280f;  // 기본 가로 화면비 (16 : 9)
    private const float c_OrthoScreenHeight = 720f;
    private const float c_OrthoScreenRate = 6.4f;     // 1280 기준 6.4는 하프 사이즈
    private const float c_OrthoScreenBaseRate = 1.77777777f;  // 가로 세로 비율
   
    private float m_fOrthoGrpicSize = 0;
    private bool  m_bScreenFit = false;             public bool IsScreenFit { get { return m_bScreenFit; }}
    //----------------------------------------------------------------
    protected override void OnUnityAwake()
    {
        base.OnUnityAwake();

    }

    protected override void OnStageCameraInitialize()
    {
        base.OnStageCameraInitialize();
        m_fOrthoGrpicSize = m_pCamera.orthographicSize;
        if (CheckCameraWideScreen() == false && ScreenAutoFit == true)
        {
            PrivCamera2DSideAdjustPortrait(Screen.width, Screen.height);
        }
        else
        {
            OnCamera2DSideAdjustSize(m_pCamera.orthographicSize, Vector3.zero);
        }
    }

    protected override void OnStageCameraUISceneLoaded()
    {
        base.OnStageCameraUISceneLoaded();
        CManagerUIFrameBase.Instance.SetUICameraOrthpGraphicSize(m_pCamera.orthographicSize);
    }

    //---------------------------------------------------------------
    private void PrivCamera2DSideAdjustPortrait(float fWidth, float fHeight)
    {
        m_bScreenFit = true;
        // 높이 사이즈에 맞춰 직교 백터 조정 
        float fScreenWidthRate = fHeight / c_OrthoScreenWidth;
        float fScreenHeightRate = fWidth / c_OrthoScreenHeight;

        float fScreenRate = (fScreenWidthRate / fScreenHeightRate);
        m_fOrthoGrpicSize = fScreenRate * c_OrthoScreenRate;
        m_pCamera.orthographicSize = m_fOrthoGrpicSize;

        OnCamera2DSideAdjustSize(m_fOrthoGrpicSize, Vector3.zero);
    }

    private void PrivCamera2dSideAdjustTablet(float fWidth, float fHeight)
    {
        float fScreenRatio = (fHeight / fWidth);
        float fScreenAdjust = c_OrthoScreenRate / c_OrthoScreenBaseRate;
        float fScreenRate = fScreenAdjust * fScreenRatio;
        float fScreenOffset = c_OrthoScreenBaseRate - fScreenRatio;
        m_fOrthoGrpicSize = fScreenRate + fScreenOffset;
        m_pCamera.orthographicSize = m_fOrthoGrpicSize;

        Vector3 vecPosition = m_pCamera.transform.position;
        vecPosition.y -= fScreenOffset;
        m_pCamera.transform.position = vecPosition;

        Vector3 vecBackGroundPosition = Vector3.zero;
        vecBackGroundPosition.y = -fScreenOffset;


        OnCamera2DSideAdjustSize(m_fOrthoGrpicSize, vecBackGroundPosition);
    }

    private bool CheckCameraWideScreen()
    {
        bool bWideScreen = false;
        float fScreenRate = (float)Screen.height / Screen.width;
        if (fScreenRate < c_OrthoScreenBaseRate)
        {
            bWideScreen = true;
        }

        return bWideScreen;
    }



    //---------------------------------------------------------------
    protected virtual void OnCamera2DSideAdjustSize(float fOrthoSize, Vector3 vecPositionOffset) { }
}
