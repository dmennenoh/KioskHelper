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
        public PersistObject instance;

        private void Awake()
        {
            //If instance already exists and it's not this one, destroy the duplicate
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            //Set this object as the definitive instance and persist it
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}