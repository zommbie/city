using UnityEngine;

public class ZCNetError : MonoBehaviour
{
    public enum EError
    {
        None,
        AppVersionUpdate,               // 마켓에서 최신 앱으로 업데이트 해야됨 
        ServerMaintenance,              // 서버 점검중
        AccountNotExist,                // 계정이 존재하지 않는다
        AccountAccessRestricted,        // 접근이 관리자에 의해 차단되었다.
        AccountAlreadyRegisted,         // 이미 등록된 계정이다
        AccountWithDrawalMember,        // 탈퇴한 계정이 등록시도를 했다
    }

    //--------------------------------------------------------------------------
    public void DoNetworkError(EError eErrorType, string strMessage = null, int iArg = 0, float fArg = 0)
    {

    }
}
