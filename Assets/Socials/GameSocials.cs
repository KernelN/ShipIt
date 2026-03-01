using UnityEngine;

namespace Universal.SocialNet
{
    public class GameSocials : Singleton<GameSocials>
    {
        internal override bool DoNotDestroyOnLoad => true;
        
        public System.Action SignedAndLinked;
        bool isSignedIn;
        
        string leaderboardID = "";
        
        //Unity Events


        [SerializeField] GPManager googlePlayManager;


        void Start()
        {
            #if UNITY_ANDROID && PLATFORM_ANDROID
                googlePlayManager.SignedIn += OnSignedIn;
            #endif
        }
        void OnSignedIn()
        {
            isSignedIn = true;
            SignedAndLinked?.Invoke();
        }

        public void LogIn()
        {
            if(isSignedIn) return;
#if UNITY_ANDROID && PLATFORM_ANDROID
            googlePlayManager.Awake();
            googlePlayManager.Start();
#endif
        }

        public void AddScoreToLeaderboard(int score)
        {
#if UNITY_EDITOR || PLATFORM_STANDALONE
            return;
#endif
            if (score % 5 != 0)
            {
                Debug.Log("GameSocials ERROR - Score must be a multiple of 5");
                return;
            }
            if (Social.localUser.authenticated)
            {
                Social.ReportScore(score, leaderboardID, success => { });
            }
        }
        public void ShowLeaderboard()
        {
#if UNITY_EDITOR || PLATFORM_STANDALONE
            return;
#endif
            if (isSignedIn)
            {
#if UNITY_ANDROID || PLATFORM_ANDROID
                googlePlayManager.ShowLeaderboardUI();
#endif
            }
        }
        public void ShowAchievements()
        {
#if UNITY_EDITOR || PLATFORM_STANDALONE
            return;
#endif
            if (isSignedIn)
            {
#if UNITY_ANDROID || PLATFORM_ANDROID
                googlePlayManager.ShowAchievementsUI();
#endif
            }
        }
        public void UnlockAchievement(string id, bool unlockFromPlatform = false)
        {
#if UNITY_EDITOR || PLATFORM_STANDALONE
            return;
#endif
            if (unlockFromPlatform)
            {
#if UNITY_ANDROID || PLATFORM_ANDROID
                googlePlayManager.UnlockAchievement(id);
#endif
                return;
            }
            
            if (Social.localUser.authenticated)
                Social.ReportProgress(id, 100f, success => { });
            else
                Debug.Log("GameSocials - Failed to unlock achievement");
            
        }
    }
}