using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Client
{
    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        public UnityEvent SceneLoadedEvent;
        public UnityEvent BlackoutClosedEvent;
        
        private float loadingPercent;
        public static bool IsLoading { get; private set; } = false;
        private AsyncOperation loadingSceneOperation; 
        
        public static string MenuSceneName = "Menu";
        public static string SpaceshipSceneName = "Spaceship";

        private static bool shouldPlayOpeningAnimation = true;
        private Animator blackoutAnimator;
        private static readonly int sceneOpening = Animator.StringToHash("SceneOpening");
        private static readonly int sceneClosing = Animator.StringToHash("SceneClosing");

        protected override void Awake() { }
        
        private void Start() 
        {
            var instance = GetInstance();
            instance.blackoutAnimator = GetComponent<Animator>();
            
            if (shouldPlayOpeningAnimation)
            {
                instance.blackoutAnimator.SetTrigger(sceneOpening);
                shouldPlayOpeningAnimation = false;
                SceneLoadedEvent.Invoke();
            }
        }

        public static void SwitchToScene(string sceneName, bool forceBlackoutRequired = false)
        {
            var instance = GetInstance();
            instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            IsLoading = true;
            
            // Чтобы сцена не начала переключаться сразу после загрузки
            instance.loadingSceneOperation.allowSceneActivation = false;
            
            instance.loadingPercent = 0;

            if (forceBlackoutRequired)
            {
                instance.blackoutAnimator.SetTrigger(sceneClosing);
            }
        }
        
        public static void StartLoadScene()
        {
            if (IsLoading)
            {
                GetInstance().blackoutAnimator.SetTrigger(sceneClosing);
                IsLoading = false;
            }
        }

        public static bool IsSceneLoaded()
        {
            var loadingSceneOperation = GetInstance().loadingSceneOperation;
            return loadingSceneOperation != null && loadingSceneOperation.isDone;
        }

        public float GetLoadProgress()
        {
            if (loadingSceneOperation != null && IsLoading)
            {
                return loadingSceneOperation.progress;
            }

            return 0;
        }
        
        public void OnAnimationOver()
        {
            shouldPlayOpeningAnimation = true;
            loadingSceneOperation.allowSceneActivation = true;
        }

        public void SceneLoadContinue()
        {
            BlackoutClosedEvent.Invoke();
        }
    }
}