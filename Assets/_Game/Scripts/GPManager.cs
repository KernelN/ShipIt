using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Universal.SocialNet
{
    public class GPManager : MonoBehaviour
    {
        public string Token;
        public string Error;

        public System.Action SignedIn;
        
        PlayGamesPlatform platform;
        
        public void Awake()
        {
            #if UNITY_EDITOR || !UNITY_ANDROID
            Destroy(this);
            return;
            #endif
            
            PlayGamesPlatform.Activate();
            platform = PlayGamesPlatform.Instance;
        }

        public async void Start()
        {
            await UnityServices.InitializeAsync();
            await LoginGooglePlayGames();
            await SignInWithGooglePlayGamesAsync(Token);
        }

        //Fetch the Token / Auth code
        public Task LoginGooglePlayGames()
        {
            var tcs = new TaskCompletionSource<object>();
            platform.Authenticate((success) =>
            {
                if (success == SignInStatus.Success)
                {
                    Debug.Log("Login with Google Play games successful.");
                    platform.RequestServerSideAccess(true, code =>
                    {
                        Debug.Log("Authorization code: " + code);
                        Token = code;
                        // This token serves as an example to be used for SignInWithGooglePlayGames
                        tcs.SetResult(null);
                    });
                }
                else
                {
                    Error = "Failed to retrieve Google play games authorization code";
                    Debug.Log("Login Unsuccessful");
                    tcs.SetException(new Exception("Failed"));
                }
            });
            return tcs.Task;
        }


        async Task SignInWithGooglePlayGamesAsync(string authCode)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}"); //Display the Unity Authentication PlayerID
                Debug.Log("SignIn is successful.");
                SignedIn?.Invoke();
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }
        public void ShowLeaderboardUI() => platform.ShowLeaderboardUI();
        public void ShowAchievementsUI() => platform.ShowAchievementsUI();
        public void UnlockAchievement(string id) => platform.UnlockAchievement(id);
    }
}