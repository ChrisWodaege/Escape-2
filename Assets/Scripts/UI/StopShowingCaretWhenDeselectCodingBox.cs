using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopShowingCaretWhenDeselectCodingBox : MonoBehaviour
{
    [SerializeField]
    private CodingBoxController _codingBoxController;

    public void OnSelectCodingBox()
    {
        _codingBoxController.ShowCaret();
    }

    public void OnDeselectCodingBox()
    {
        _codingBoxController.HideCaret();
    }
}
