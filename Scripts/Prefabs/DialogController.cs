using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KioskHelper
{
    public enum DialogResult
    {
        Primary,
        Secondary,
        Closed,
        BackgroundClicked
    }

    public class DialogController : MonoBehaviour
    {
        [SerializeField] private GameObject modalBG;
        [SerializeField] private GameObject dialogRoot;
        [SerializeField] private Button closeButton;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private TMP_Text primaryButtonText;
        [SerializeField] private TMP_Text secondaryButtonText;
        [SerializeField] private Button primaryButton;
        [SerializeField] private Button secondaryButton;
        [SerializeField] private Image iconImage;

        private Action<DialogResult> onComplete;

        private bool closeOnBackgroundClick = false;

        private void Awake()
        {
            Close();
        }

        public void Show(DialogStyle style, string title, string mess, string priText = "OK", string secText = "",  Action<DialogResult> callback = null)
        {    
            onComplete = callback;
            
            dialogRoot.GetComponent<Image>().color = style.backgroundColor;
            if (style.backgroundImage != null) dialogRoot.GetComponent<Image>().sprite = style.backgroundImage;

            closeButton.gameObject.SetActive(style.showCloseButton);           
            if(style.closeButtonIcon != null) closeButton.image.sprite = style.closeButtonIcon;
            closeButton.image.color = style.closeButtonColor;

            titleText.text = title;
            messageText.text = mess;

            primaryButtonText.text = priText;
            secondaryButtonText.text = secText;

            primaryButton.image.color = style.primaryButtonColor;
            secondaryButton.image.color = style.secondaryButtonColor;

            secondaryButton.gameObject.SetActive(secText == "" ? false : true);

            iconImage.sprite = style.icon;
            iconImage.color = style.iconColor;
            iconImage.gameObject.SetActive(style.icon != null);

            modalBG.GetComponent<Image>().color = new Color(0, 0, 0, style.backgroundOverlayAlpha);
            closeOnBackgroundClick = style.closeOnBackgroundClick;

            gameObject.SetActive(true);
        }


        //These are called from the dialog's buttons
        public void OnModalClicked()
        {
            if (closeOnBackgroundClick)
            {
                Close(DialogResult.BackgroundClicked);
            }
        }

        public void OnPrimaryClicked()
        {
            Close(DialogResult.Primary);
        }

        public void OnSecondaryClicked()
        {
            Close(DialogResult.Secondary);
        }

        public void OnCloseClicked()
        {
            Close();
        }

        public void Close(DialogResult result = DialogResult.Closed)
        {
            gameObject.SetActive(false);

            var callback = onComplete;
            onComplete = null;

            callback?.Invoke(result);
        }
    }
}