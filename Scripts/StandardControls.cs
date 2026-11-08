using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Standard Controls 
 * for a kiosk - uses the new input system
 * 
 * F1 - Show / Hide mouse cursor
 * Esc - quit application
 */

namespace KioskHelper
{
    public class StandardControls : MonoBehaviour
    {
        private bool isCursorHidden;

        private void Awake()
        {
            isCursorHidden = true;
            HideCursor();
        }

        private void Update()
        {
            //F1 - toggles mouse visibility
            if (Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
            {
                isCursorHidden = !isCursorHidden;

                if (isCursorHidden)
                {
                    HideCursor();
                }
                else
                {
                    ShowCursor();
                }
            }

            //Esc - quits app
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Application.Quit();

                //For the Unity Editor - escape stops in play mode
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
        }

        private void HideCursor()
        {
            Cursor.visible = false;
        }

        private void ShowCursor()
        {
            Cursor.visible = true;
        }

    }//class
}//namespace