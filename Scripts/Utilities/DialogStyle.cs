using UnityEngine;

namespace KioskHelper
{
    [CreateAssetMenu(fileName = "Dialog Style", menuName = "KioskHelper/Dialog Style")]

    public class DialogStyle : ScriptableObject
    {
        [Header("Appearance")]
        public Sprite icon;
        public Color iconColor = Color.white;

        public Sprite backgroundImage;
        public Color backgroundColor = new Color(.168f, .168f, .168f);//dark gray

        [Header("Button Appearance")]
        public Color primaryButtonColor = new Color(.423f, .423f, .423f);
        public Color secondaryButtonColor = new Color(.423f, .423f, .423f);

        [Header("Close Button")]
        public bool showCloseButton = true;
        public Sprite closeButtonIcon;
        public Color closeButtonColor = new Color(.38f, .38f, .38f);

        [Header("Background Overlay")]
        public bool closeOnBackgroundClick = false;
        [Range(0f, 1f)]
        public float backgroundOverlayAlpha = .5f;
    }
}