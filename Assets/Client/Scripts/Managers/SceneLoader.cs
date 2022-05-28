using UnityEngine;
using UnityEngine.SceneManagement;

namespace Client
{
    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        private float loadingPercent;
        public static bool IsLoading { get; private set; } = false;
        private AsyncOperation loadingSceneOperation;

        public static string MenuSceneName = "Menu";
        public static string SpaceshipSceneName = "Spaceship";

        public static void SwitchToScene(string sceneName)
        {
            var instance = GetInstance();
            instance.loadingSceneOperation = SceneManager.LoadSceneAsync(sceneName);
            IsLoading = true;
            
            // Чтобы сцена не начала переключаться сразу после загрузки
            instance.loadingSceneOperation.allowSceneActivation = false;
            
            instance.loadingPercent = 0;
        }
        
        public void LoadScene()
        {
            if (IsLoading)
            {
                loadingSceneOperation.allowSceneActivation = true;
                IsLoading = false;
            }
        }

        public float GetLoadProgress()
        {
            if (loadingSceneOperation != null && IsLoading)
            {
                return loadingSceneOperation.progress;
            }

            return 0;
        }
    }
}