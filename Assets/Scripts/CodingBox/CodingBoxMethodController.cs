using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodingBoxMethodController : MonoBehaviour
{
    public delegate void CompleteAction();

    private Coroutine _updateCoroutine;

    private List<IEnumerator> _methodList = new List<IEnumerator>();

    private CompleteAction _onCompleteAction;

    public void SetOnCompleteAction(CompleteAction action)
    {
        _onCompleteAction = action;
    }

    public void StartMethodLoop()
    {
        _updateCoroutine = StartCoroutine(MethodLoopCoroutine());
    }

    public void AddMethod(IEnumerator method)
    {
        _methodList.Add(method);
    }

    private IEnumerator MethodLoopCoroutine()
    {
        foreach (var method in _methodList)
        {
            yield return StartCoroutine(method);
        }

        if (_onCompleteAction != null)
        {
            _onCompleteAction();
        }
    }
}
