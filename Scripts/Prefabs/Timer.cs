using UnityEngine;
using TMPro;
using System;
using UnityEngine.Events;

/**
 * Timer
 * 
 * Simple on-screen timer component
 * 
 * This implementation uses Time.deltaTime so if the app is paused by setting timeScale = 0
 * the timer will also pause. For a timer to keep running use Time.unscaledTime
 * 
 */

namespace KioskHelper
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        //If both of these are false then just minutes and seconds will be displayed
        [SerializeField] private bool displayHundreths = false;//show hundreths
        [SerializeField] private bool displayTenths = true;//show tenths instead of hundreths

        private float elapsedTime;//current timer time, in seconds. Call SetTimer(seconds) to initialize
        private bool isRunning;//true if the timer is running, call PauseTimer() or StopTimer() to 
        private bool isCountingUp;//true if StartTimer() is called, false if StartCountdown() is called
        private float countDownTime;//used to reset the elapsedTime when ResetTimer() is called and isCountingUp is false

        public float ElapsedTime => elapsedTime;
        public bool IsRunning => isRunning;

        public UnityEvent timerComplete;//event is raised if counting down and the timer reaches 0


        void Update()
        {
            if (!isRunning) return;

            if (isCountingUp)
            {
                elapsedTime += Time.deltaTime;
            }
            else
            {
                elapsedTime -= Time.deltaTime;

                if (elapsedTime <= 0)
                {
                    elapsedTime = 0;
                    isRunning = false;
                    timerComplete?.Invoke();
                }
            }
            UpdateDisplay();
        }

        //Call SetTimer(seconds) to initialize the timer before starting
        public void SetTimer(float numSeconds = 0)
        {
            elapsedTime = numSeconds;
            countDownTime = numSeconds;
            UpdateDisplay();
        }

        //Starts the timer counting Up
        public void StartTimer()
        {
            isCountingUp = true;
            isRunning = true;
        }

        //Starts the timer counting Down to zero - it will stop when it reaches zero.
        public void StartCountdown()
        {            
            isCountingUp = false;
            isRunning = true;
        }

        //Pauses the timer
        public void PauseTimer()
        {
            isRunning = false;
        }

        //Resumes the timer
        public void ResumeTimer()
        {
            isRunning = true;
        }

        //Resets the timer and display
        //Shows either zero if counting up, or the initial countdown time if counting down
        public void ResetTimer()
        {
            if (isCountingUp)
            {
                elapsedTime = 0f;
            }
            else
            {
                elapsedTime = countDownTime;
            }            
            UpdateDisplay();
        }
       
        //Updates the text component
        private void UpdateDisplay()
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int tenths = Mathf.FloorToInt((elapsedTime * 10f) % 10f);
            int hundredths = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

            if (displayHundreths)
            {
                timerText.text = $"{minutes:00}:{seconds:00}.{hundredths:00}";
            }
            else 
            {
                if (displayTenths)
                {
                    timerText.text = $"{minutes:00}:{seconds:00}.{tenths:0}";
                }
                else
                {
                    timerText.text = $"{minutes:00}:{seconds:00}";
                }
                
            }
            
        }

    }//class
}//namespace