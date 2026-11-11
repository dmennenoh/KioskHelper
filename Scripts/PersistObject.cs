using UnityEngine;

/**
 * Persist Object
 * Just keeps any object, and any children, alive between scene changes
 * 
 * Tip:Use a bootstrap scene that this is in, along with anything else
 * you need to persist - and then never go back there. 
 */

namespace KioskHelper
{
    public class PersistObject : MonoBehaviour
    {
        public static PersistObject Instance { get; private set; }

        private void Awake()
        {
            //If an instance already exists, destroy the duplicate
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            //set this as the definitive instance and persist it
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}