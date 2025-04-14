using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CManagerLoginBase : CManagerTemplateBase<CManagerLoginBase>
{
    private const string c_RegistLoginTypeName  = "Lastest Login";
    private const string c_RegistLoginToken     = "Lastest Token";
    //--------------------------------------------------------------
    public enum ELoginType
	{
        None,
        LoginGuest,         // 논 서버 솔루션(로컬 DB 로 게임 진행) / 서버 솔루션 (서버에서 게스트 계정을 생성하여 진행)
        LoginVirtual,       // 로컬 DB 로 진행한다. 개발용 
        LoginDevelop,       // 테스트 서버등 백엔드 접속 테스트용
        LoginGoogle,
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
    private UnityAction<ELoginResult, string, long> m_delLoginFinish = null;
    private CLoginBase m_pLoginInstance = null;
    //--------------------------------------------------------
    public void DoMgrLoginStart(ELoginType eLoginType, UnityAction<ELoginResult, string, long> delFinish)
	{
        PrivMgrLoginStart(eLoginType, string.Empty, delFinish);
    }

    public void DoMgrLoginAuto(UnityAction<ELoginResult, string, long> delFinish)
	{
        PrivMgrLoginStartAuto(delFinish);
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
    private void PrivMgrLoginStart(ELoginType eLoginType, string strReserveToken, UnityAction<ELoginResult, string, long> delFinish)
	{
        if (m_eLoginState != ELoginState.None) return;
        
        m_eLoginState = ELoginState.TryLogin;
        m_eLoginType = eLoginType;
        m_delLoginFinish = delFinish;

        switch(eLoginType)
		{
            case ELoginType.LoginGuest:
                m_pLoginInstance = OnMgrLoginGuest(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginVirtual:
                m_pLoginInstance = OnMgrLoginVirtual(strReserveToken, HandleMgrLoginFinish);
                break;
            case ELoginType.LoginDevelop:
                m_pLoginInstance = OnMgrLoginDevelop(strReserveToken, HandleMgrLoginFinish);
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
            PrivMgrLoginTypeSave(m_eLoginType, strToken);
        }
        else
		{
            m_eLoginState = ELoginState.LoginFail;
		}
        m_delLoginFinish?.Invoke(eLoginResult, strToken, iTokenID);
	}
    //-----------------------------------------------------------
    private void PrivMgrLoginTypeSave(ELoginType eLoginType, string strToken)
	{
        PlayerPrefs.SetInt(c_RegistLoginTypeName, (int)eLoginType);
		PlayerPrefs.SetString(c_RegistLoginToken, strToken);
	}

	private void PrivMgrLoginStartAuto(UnityAction<ELoginResult, string, long> delFinish)
	{
        if (PlayerPrefs.HasKey(c_RegistLoginTypeName))
		{
            ELoginType eLoginType = (ELoginType)(PlayerPrefs.GetInt(c_RegistLoginTypeName));
            string strToken = PlayerPrefs.GetString(c_RegistLoginToken);
            PrivMgrLoginStart(eLoginType, strToken, delFinish);
		}
        else
		{
            delFinish?.Invoke(ELoginResult.FailAutoLogin, string.Empty, 0);
		}
    }

    //------------------------------------------------------------
    protected virtual CLoginBase OnMgrLoginGuest(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginVirtual(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginDevelop(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginGoogle(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginAppleStore(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    protected virtual CLoginBase OnMgrLoginOneStore(string strReserveToken, UnityAction<ELoginResult, string, long> delFinish) { return null; }
    
}
