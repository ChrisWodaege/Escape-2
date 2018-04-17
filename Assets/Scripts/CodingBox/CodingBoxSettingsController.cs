using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodingBoxSettingsController : MonoBehaviour
{
    [SerializeField]
    private Color _mainColor = Color.grey;

    [SerializeField]
    private Image[] _mainColorImages;

    [SerializeField]
    private Color _buttonColor = Color.grey;
    [SerializeField]
    private Color _buttonPressedColor = Color.white;
    [SerializeField]
    private Color _buttonHighlightedColor = Color.red;

    [SerializeField]
    private Button[] _buttons;

    public Color MainColor { get { return _mainColor; } private set { _mainColor = value; } }

    public Color ButtonColor { get { return _buttonColor; } private set { _buttonColor = value; } }

    public Color ButtonPressedColor { get { return _buttonPressedColor; } private set { _buttonPressedColor = value; } }

    public Color ButtonHighlightedColor { get { return _buttonHighlightedColor; } private set { _buttonHighlightedColor = value; } }


    public void Reset()
    {
        MainColor = Color.gray;

        UpdateColors();
    }

    public void UpdateColors()
    {
        foreach (var img in _mainColorImages)
        {
            img.color = MainColor;
        }

        foreach (var button in _buttons)
        {
            var color = button.colors;

            color.normalColor = _buttonColor;
            color.pressedColor = _buttonPressedColor;
            color.highlightedColor = _buttonHighlightedColor;

            button.colors = color;
        }
    }

    private void Start()
    {
        UpdateColors();
    }
}
