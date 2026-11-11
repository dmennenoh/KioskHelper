using System;
using UnityEngine;
using UnityEngine.Events;

/**
 * Idle Detector 
 * 
 * listens for user input - raises onIdle if no activity has been detected for the set period (one minute default)
 * raises onWarning prior to onIdle which can be used to show a 'are you still there?' type dialog
 * onActivity fires _after_ idle has fired to allow closing a screen saver, etc. when someone begins interacting again
 * 
 * Uses Unity events for easy hookup in the Inspector
 */
namespace KioskHelper
{
    public class IdleDetector : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float idleTime = 60f;
        [SerializeField] private float warningTime = 10f;//fires warning event n seconds before idle is fired

        [Header("Events")]
        public UnityEvent onWarning;//fired warningTime seconds before onIdle is fired - allows popping a dialog like 'are you still there?'
        public UnityEvent onIdle;//fired when the idleTime is reached with no interaction
        public UnityEvent onActivity;//allows you to dismiss the screensaver or return to the interactive experience              

        private float lastActivityTime;
        private bool isIdle;
        private bool warningTriggered;

        private Vector3 _lastMousePosition;


        private void OnEnable()
        {
            Reset();
        }

        private void Update()
        {
            if (DetectActivity())
            {
                if (isIdle)
                {
                    onActivity?.Invoke();
                }
                Reset();
                return;
            }

            float elapsed = Time.unscaledTime - lastActivityTime;

            //Warning threshold
            if (!warningTriggered && warningTime > 0 && elapsed >= idleTime - warningTime && elapsed < idleTime)
            {
                warningTriggered = true;
                onWarning?.Invoke();
            }

            //Idle threshold
            if (!isIdle && elapsed >= idleTime)
            {
                isIdle = true;
                onIdle?.Invoke();
            }
        }


        private bool DetectActivity()
        {
            //Mouse movement or button click
            if (Input.mousePosition != _lastMousePosition)
            {
                _lastMousePosition = Input.mousePosition;
                return true;
            }

            if (Input.anyKeyDown) return true;

            //Touch input
            if (Input.touchCount > 0) return true;

            return false;
        }


        public void Reset()
        {
            lastActivityTime = Time.unscaledTime;
            isIdle = false;
            warningTriggered = false;
        }

    }//class
}//namespace
