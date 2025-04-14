#if (UNITY_ANDROID)
 
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using Firebase.Auth;
//using UnityEngine.Events;
//using UnityEngine;

//public abstract class COAuth2GooglePlayFirebase : CLoginBase
//{
//    private FirebaseAuth m_pFireBaseAuth = null;
//    private bool m_bLoginTry = false;

//    private string m_strUserGoogleID;
//    private string m_strUserGoogleToken;
//    private string m_strUserFirebaseToken;

//    //----------------------------------------------------------------------------

//    protected override void OnOAuthInitialize()
//    {
//        PlayGamesPlatform.DebugLogEnabled = true;
//        m_pFireBaseAuth = FirebaseAuth.DefaultInstance;
//    }

//    protected override void OnOAuthLogIn(UnityAction<bool, string, string> delFinish)
//    {
//        PrivAuthGooglePlayStart(delFinish);
//    }

//    protected override void OnOAuthLogOut()
//    {
//        m_pFireBaseAuth.SignOut();

//    }

//    //------------------------------------------------------------------
//    private void PrivAuthGooglePlayStart(UnityAction<bool, string, string> delFinish)
//    {
//        m_bLoginTry = false;
//        PlayGamesPlatform.Instance.Authenticate((SignInStatus status) =>
//        {
//            PrivAuthGoogleLoginResult(status, delFinish);
//        });
//    }

//    private void PrivAuthGoogleLoginResult(SignInStatus status, UnityAction<bool, string, string> delFinish)
//    {
//        switch (status)
//        {
//            case SignInStatus.Success:
//                PlayGamesPlatform.Instance.RequestServerSideAccess(true, (string strToken) =>
//                {
//                    PrivFireBaseLogin(strToken, delFinish);
//                });
//                break;
//            case SignInStatus.InternalError:
//                delFinish?.Invoke(false, "[Login] Google Login Fail", string.Empty);
//                break;
//            case SignInStatus.Canceled:
//                if (m_bLoginTry == false)
//                {
//                    m_bLoginTry = true;
//                    PlayGamesPlatform.Instance.ManuallyAuthenticate((SignInStatus status) => {
//                        PrivAuthGoogleLoginResult(status, delFinish);
//                    });
//                    return;
//                }
//                delFinish?.Invoke(false, "[Login] Google Login Canceled", string.Empty);
//                break;
//        }
//    }

//    private void PrivFireBaseLogin(string strToken, UnityAction<bool, string, string> delFinish)
//    {
//        m_pFireBaseAuth = FirebaseAuth.DefaultInstance;
//        Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(strToken);
//        Firebase.Extensions.TaskExtension.ContinueWithOnMainThread(m_pFireBaseAuth.SignInWithCredentialAsync(credential), task =>
//        {
//            if (task.IsCanceled)
//            {
//                Debug.LogError("[Login] FireBase Login Cancle");
//                delFinish?.Invoke(false, "FireBase Login Cancle", string.Empty);
//                return;
//            }
//            if (task.IsFaulted)
//            {
//                Debug.LogError("[Login] FireBase Login Error");
//                Debug.Log(task.Exception);
//                delFinish?.Invoke(false, "FireBase Login Error", string.Empty);
//                return;
//            }

//            Debug.Log("[Login] FireBase Login Sucess");
//            m_strUserGoogleID = PlayGamesPlatform.Instance.GetUserId();
//            m_strUserGoogleToken = strToken;
//            m_strUserFirebaseToken = task.Result.UserId;

//            delFinish?.Invoke(true, task.Result.UserId, strToken);
//        });
//    }
//}
#else
public class COAuth2GooglePlayFirebase : CLoginBase
{
}

#endif