using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startScene = string.Empty;


        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }


        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }
         private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startScene));
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGO)
        {
            StartCoroutine(Transition(sceneToGo, positionToGO));
        }
       

        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="sceneName">目标场景</param>
        /// <param name="targetPosition">目标位置</param>
        /// <returns></returns>

        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);

            //移动人物坐标
            EventHandler.CallMoveToPosition(targetPosition);

            EventHandler.CallAfterSceneLoadedEvent();

            
        }


        /// <summary>
        /// 加载场景并设置为激活
        /// </summary>
        /// <param name="sceneName">场景名</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newScene);
        }
    }

}
