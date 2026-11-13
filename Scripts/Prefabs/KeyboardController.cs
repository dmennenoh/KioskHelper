using UnityEngine;

public class KeyboardController : MonoBehaviour
{
    [SerializeField] private Sprite standardKeyImage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called in the Editor whenever a serialized value changes in the Inspector
    //Shows the updated transition image without having to hit Play first
    private void OnValidate()
    {
        //if (leftImage != null) leftImage.sprite = transitionImage;
        //if (rightImage != null) rightImage.sprite = transitionImage;
    }
}
