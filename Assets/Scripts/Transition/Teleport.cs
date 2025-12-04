using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SFarm.Transition
{
    /// <summary>
    /// Íæ¼Ò´«ËÍ
    /// </summary>
    public class Teleport : MonoBehaviour
    {
        [SceneName]
        public string sceneToGo;

        public Vector3 positionToGo;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}
