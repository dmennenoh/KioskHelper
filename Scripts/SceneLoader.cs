using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

/**
 * Scene Loader
 * 
 * Loads a new scene asynchronously
 * if you provide a callback for onReady in LoadNewScene then you will need
 * to call ActivateScene() on your own. If no callback is provided ActivateScene()
 * is called automatically. In this way you can remove a transition element
 * before starting the new scene.
 * 
 * simple load with no transition element - automatic scene activattion:
 * sceneLoader.LoadNewScene("MainMenu");
 * 
 * Loading with a transition:
 * sceneLoader.LoadNewScene("MainMenu", OnSceneReady);
 * 
 * private void OnSceneReady()
 * {
 *   sceneTransitionController.Hide();
 *   sceneLoader.ActivateScene();
 * }
 */

namespace KioskHelper
{
    public class SceneLoader : MonoBehaviour
    {
        private AsyncOperation asyncLoad;
        public bool IsLoading { get; private set; }
        public float Progress => asyncLoad != null ? asyncLoad.progress / 0.9f : 0f;


        //onReady will be called when scene assets are loaded but before the scene is activated
        //onComplete is called once ActivateScene has been called and the scene is fully activated
        public void LoadNewScene(string sceneName, Action onReady = null, Action onComplete = null)
        {
            if (IsLoading) return;
            StartCoroutine(LoadSceneAsync(sceneName, onReady, onComplete));
        }


        private IEnumerator LoadSceneAsync(string sceneName, Action onReady, Action onComplete)
        {
            IsLoading = true;

            asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            //prevents scene from auto starting once loaded
            asyncLoad.allowSceneActivation = false;

            //Unity splits async progress into two parts:
            //0.0 to 0.9 = Loading assets and data
            //0.9 to 1.0 = Activating the scene
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            onReady?.Invoke();

            //if nobody is handling the ready callback, automatically activate the scene.
            if (onReady == null) ActivateScene();

            //wait for scene activation.
            yield return new WaitUntil(() => asyncLoad.isDone);

            IsLoading = false;
            asyncLoad = null;

            onComplete?.Invoke();
        }

        public void ActivateScene()
        {
            if (asyncLoad == null || !IsLoading) return;
            asyncLoad.allowSceneActivation = true;//asyncLoad.isDone will only be true once this is set to true
        }

    }//class
}//namespace