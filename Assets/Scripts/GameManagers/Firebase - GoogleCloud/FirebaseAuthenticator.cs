using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;

namespace MC.Modules.Firebase 
{
    public class FirebaseAuthenticator : MonoBehaviour
    {
        //private FirebaseAuth auth;
        //private GoogleSignInConfiguration configuration;

        //void Awake()
        //{
        //    auth = FirebaseAuth.DefaultInstance;

        //    // WebClientId: Firebase Console > Project Settings > Web application client ID
        //    configuration = new GoogleSignInConfiguration
        //    {
        //        WebClientId = "YOUR_WEB_CLIENT_ID_HERE",
        //        RequestIdToken = true
        //    };
        //}

        //public void OnGoogleSignInButtonClicked()
        //{
        //    GoogleSignIn.Configuration = configuration;
        //    GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleSignInFinished);
        //}

        //private void OnGoogleSignInFinished(Task<GoogleSignInUser> task)
        //{
        //    if (task.IsFaulted || task.IsCanceled)
        //    {
        //        Debug.LogError("Google Sign-In failed: " + task.Exception);
        //    }
        //    else
        //    {
        //        var googleIdToken = task.Result.IdToken;
        //        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

        //        // Şimdi Firebase'de giriş yapıyoruz
        //        auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
        //        {
        //            if (authTask.IsCanceled || authTask.IsFaulted)
        //            {
        //                Debug.LogError("Firebase Auth failed: " + authTask.Exception);
        //            }
        //            else
        //            {
        //                Firebase.Auth.AuthResult result = authTask.Result;
        //                Debug.LogFormat("Kullanıcı girişi başarılı: {0} ({1})", result.User.DisplayName, result.User.UserId);
        //            }
        //        });
        //    }
        //}
    }
}
