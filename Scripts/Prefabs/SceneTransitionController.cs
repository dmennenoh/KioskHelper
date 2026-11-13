using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using UnityEngine.Events;

/*
 * Scene Transition Controller
 * 
 * Uses DOTween for animation
 */

namespace KioskHelper
{
    public class SceneTransitionController : MonoBehaviour
    {
        [SerializeField] private bool horizontalTransition = true;//vertical if false

        [SerializeField] private Sprite transitionImage;

        [SerializeField] private RectTransform leftMask;
        [SerializeField] private Image leftImage;
        [SerializeField] private RectTransform rightMask;
        [SerializeField] private Image rightImage;        

        private RectTransform transitionCanvas;
        private float canvasWidth;
        private float canvasHeight;

        [SerializeField] private UnityEvent transitionHidden;
        [SerializeField] private UnityEvent transitionShowing;



        //Called in the Editor whenever a serialized value changes in the Inspector
        //Shows the updated transition image without having to hit Play first
        private void OnValidate()
        {
            if (leftImage != null) leftImage.sprite = transitionImage;
            if (rightImage != null) rightImage.sprite = transitionImage;
        }


        private void Start()
        {
            transitionCanvas = gameObject.GetComponent<RectTransform>();
            canvasWidth = transitionCanvas.rect.width;
            canvasHeight = transitionCanvas.rect.height/2;

            leftImage.sprite = transitionImage;
            rightImage.sprite = transitionImage;

            Hide(false);
        }


        public void Show()
        {
            leftMask.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuint).OnComplete(() => transitionShowing?.Invoke());
            rightMask.DOAnchorPos(Vector2.zero, 0.5f).SetEase(Ease.OutQuint);
        }


        public void Hide(bool animate = true)
        {
            if (animate)
            {
                if(horizontalTransition)
                {
                    leftMask.DOAnchorPos(new Vector2(-canvasWidth, 0), 0.5f).SetEase(Ease.InQuint).OnComplete(() => transitionHidden?.Invoke());
                    rightMask.DOAnchorPos(new Vector2(canvasWidth, 0), 0.5f).SetEase(Ease.InQuint);
                }
                else
                {
                    leftMask.DOAnchorPos(new Vector2(0, canvasHeight), 0.5f).SetEase(Ease.InQuint).OnComplete(() => transitionHidden?.Invoke());
                    rightMask.DOAnchorPos(new Vector2(0, -canvasHeight), 0.5f).SetEase(Ease.InQuint);
                }
                
            }
            else
            {
                //just set the position with no animation   
                if(horizontalTransition)
                {
                    leftMask.anchoredPosition = new Vector2(-canvasWidth, 0);
                    rightMask.anchoredPosition = new Vector2(canvasWidth, 0);
                }
                else
                {
                    leftMask.anchoredPosition = new Vector2(0, canvasHeight);
                    rightMask.anchoredPosition = new Vector2(0, -canvasHeight);
                }                
                transitionHidden?.Invoke();
            }
        }

    }//class
}//namespace
