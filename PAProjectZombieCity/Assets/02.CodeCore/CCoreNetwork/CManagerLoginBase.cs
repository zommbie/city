using UnityEngine;
using UnityEngine.Events;

// 게임의 로그인 기준으로 네트워크 타입을 결정한다.
// 로그인 타입에 따라 로컬 DB 와 세션등이 설정된다. 

public abstract class CManagerLoginBase : CManagerTemplateBase<CManagerLoginBase>
{
    private const string c_RegistLoginTypeName  = "Lastest Login";
    private const string c_RegistLoginToken     = "Lastest Token";
    //--------------------------------------------------------------
    public enum ELoginType
	{
        None,
        LoginVirtual,            // 버추얼 백앤드가 활성화 되며 로컬 DB가 개발자용으로 셋팅된다.
        LoginDevelop,            // 테스트 서버등 개발자용 접속이 시도된다. 

        LoginGuestLocal,         // 논 서버 솔루션(로컬 DB 로 게임 진행)으로 오프라인 진행이 가능한 타입
        LoginGuest,              // 백앤드에서 게스트 계정을 발급받아서 연결한다.
        LoginGoogle,             // OAuth2 접속용
        LoginAppleStore,         
        LoginOneStore,       
	}

    private enum ELoginState
    {
        None,
        TryLogin,
        LoginSucess,
        LoginFail,
        LogOut,
    }

    public enum ELoginResult
	{
        None,
        Sucess,
        Fail,
        FailAutoLogin,
	}

    private ELoginState m_eLoginState = ELoginState.None;
    private ELoginType  m_eLoginType = ELoginType.None;   
    private UnityAction<ELoginResult> m_delLoginFinish = null;
    private CLoginBase m_pLoginInstance = null;
    //--------------------------------------------------------
    public void DoMgrLoginStart(ELoginType eLoginType, UnityAction<ELoginResult> delFinish)
	{
        PrivMgrLoginStart(eLoginType, string.Empty, delFinish);
    }

    public void DoMgrLoginAuto(UnityAction<ELoginResult, string, long> delFinish)
	{
    //    PrivMgrLoginStartAuto(delFinish);
    }

    public void DoMgrLogOut()
	{
        if (m_pLoginInstance != null)
		{
            m_pLoginInstance.DoOAuthLogOut();
            m_eLoginState = ELoginState.LogOut;
        }
	}

    //------------------------------------------------------------
    private void PrivMgrLoginStart(ELoginType eLoginType, string strReserveToken, UnityAction<ELoginResult> delFinish)
	{
        if (m_eLoginState != ELoginState.None) return;
        
        m_eLoginState = ELoginState.TryLogin;
        m_eLoginType = eLoginType;
        m_delLoginFinish = delFinish;

        switch(eLoginType)
		{
            case ELoginType.LoginVirtual:
                m_pLoginInstance = OnMgrLoginVirtual(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginDevelop:
                m_pLoginInstance = OnMgrLoginDevelop(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginGuestLocal:
                m_pLoginInstance = OnMgrLoginGuestLocal(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginGuest:
                m_pLoginInstance = OnMgrLoginGuest(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginGoogle:
                m_pLoginInstance = OnMgrLoginGoogle(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginAppleStore:
                m_pLoginInstance = OnMgrLoginAppleStore(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginOneStore:
                m_pLoginInstance = OnMgrLoginOneStore(strReserveToken, HandleMgrLoginFinish);
                break;
		}       
	}

    private void HandleMgrLoginFinish(ELoginResult eLoginResult, string strToken, long iTokenID)
	{
        if (eLoginResult == ELoginResult.Sucess)
		{
            m_eLoginState = ELoginState.LoginSucess;
        //    PrivMgrLoginTypeSave(m_eLoginType, strToken);
        }
        else
		{
            m_eLoginState = ELoginState.LoginFail;
		}
        m_delLoginFinish?.Invoke(eLoginResult);
	}
    //-----------------------------------------------------------
    private void PrivMgrLoginTypeSave(ELoginType eLoginType, string strToken)
	{
        PlayerPrefs.SetInt(c_RegistLoginTypeName, (int)eLoginType);
		PlayerPrefs.SetString(c_RegistLoginToken, strToken);
	}

	private void PrivMgrLoginStartAuto(UnityAction<ELoginResult> delFinish)
	{
        if (PlayerPrefs.HasKey(c_RegistLoginTypeName))
		{
            ELoginType eLoginType = (ELoginType)(PlayerPrefs.GetInt(c_RegistLoginTypeName));
            string strToken = PlayerPrefs.GetString(c_RegistLoginToken);
            PrivMgrLoginStart(eLoginType, strToken, delFinish);
		}
        else
		{
            delFinish?.Invoke(ELoginResult.FailAutoLogin);
		}
    }

    //------------------------------------------------------------
    protected virtual CLoginBase OnMgrLoginGuest(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginGuestLocal(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginVirtual(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginDevelop(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginGoogle(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginAppleStore(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginOneStore(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    
}
