using UnityEngine;

// 다양한 휴대폰 화면 및 가로 세로 모드에 따른 비율 처리를 자동화 하기 위한 기능
// 직교 백터를 작은 변에 맞추면 큰 변이 비는 경우가 많은데 이때 사이즈를 조정해서 적당한 크기로 맞춘다

public abstract class CStageCameraOrthoGraphicBase : CStageCameraBase
{
    [SerializeField]
    private bool ScreenAutoFit = true;              

    private const float c_OrthoStandardWidth = 1080f;  // 기본 가로 화면비 (16 : 9)
    private const float c_OrthoStandardHeight = 1920f;   
      
    private float m_fOrthoGrpicSize = 0;
    private bool  m_bScreenFit = false;             public bool IsScreenFit { get { return m_bScreenFit; }}
    //----------------------------------------------------------------
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
        float fScreenHeightRate = fHeight / c_OrthoStandardHeight;
        float fScreenWidthRate = fWidth / c_OrthoStandardWidth;
        float fStandardOrthoSize = c_OrthoStandardHeight / 100f / 2f; // 100 = unit per pixel
        float fScreenRate = fScreenHeightRate / fScreenWidthRate;
        m_fOrthoGrpicSize = fScreenRate * fStandardOrthoSize;
        m_pCamera.orthographicSize = m_fOrthoGrpicSize;

        OnCamera2DSideAdjustSize(m_fOrthoGrpicSize, Vector3.zero);
    }


    private bool CheckCameraWideScreen()
    {
        bool bWideScreen = false;
        float fScreenRate = (float)Screen.height / Screen.width;
        float fScreenHeightRate = c_OrthoStandardHeight / c_OrthoStandardWidth;
        if (fScreenRate < fScreenHeightRate)
        {
            bWideScreen = true;
        }

        return bWideScreen;
    }


    //---------------------------------------------------------------
    protected virtual void OnCamera2DSideAdjustSize(float fOrthoSize, Vector3 vecPositionOffset) { }
}
