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
        [SerializeField] private Sprite transitionImage;

        [SerializeField] private RectTransform leftMask;
        [SerializeField] private Image leftImage;
        [SerializeField] private RectTransform rightMask;
        [SerializeField] private Image rightImage;

        private RectTransform transitionCanvas;
        private float canvasWidth;

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

            leftImage.sprite = transitionImage;
            rightImage.sprite = transitionImage;

            Hide(true);
        }


        public void Show()
        {
            leftMask.DOAnchorPos(Vector2.zero, 0.5f).OnComplete(() => transitionShowing?.Invoke());
            rightMask.DOAnchorPos(Vector2.zero, 0.5f);
        }


        public void Hide(bool animate = true)
        {
            if (animate)
            {
                leftMask.DOAnchorPos(new Vector2(-canvasWidth, 0), 0.5f).OnComplete(() => transitionHidden?.Invoke());
                rightMask.DOAnchorPos(new Vector2(canvasWidth, 0), 0.5f);
            }
            else
            {
                //just set the position with no animation            
                leftMask.anchoredPosition = new Vector2(-canvasWidth, 0);
                rightMask.anchoredPosition = new Vector2(canvasWidth, 0);
                transitionHidden?.Invoke();
            }
        }

    }//class
}//namespace
