using UnityEngine;
using UnityEngine.UI;

public class BoxSlider : MonoBehaviour
{
    [SerializeField]
    private Button toggleButton;

    [SerializeField]
    private Animator _codingBoxAnimator;

    [SerializeField]
    private Animator _cameraAnimator;

    private bool _isOpen = true;

    void Start()
    {
        toggleButton.onClick.AddListener(toggleBox);
    }

    public void toggleBox()
    {
        if (_isOpen)
        {
            _isOpen = false;
            FadeBoxOut();
        }
        else
        {
            _isOpen = true;
            FadeBoxIn();
        }
    }

    //function to pause the game
    public void FadeBoxIn()
    {
        _codingBoxAnimator.Play("box-fade-in");

        _cameraAnimator.SetBool("FadeOut", false);
    }
    //function to unpause the game
    public void FadeBoxOut()
    {
        //play the Slidein animation
        _codingBoxAnimator.Play("box-fade-out");

        _cameraAnimator.SetBool("FadeOut", true);
    }
}