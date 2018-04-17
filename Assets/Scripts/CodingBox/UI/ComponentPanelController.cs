using TMPro;
using UnityEngine;

public class ComponentPanelController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _componentNameText;

    [SerializeField]
    private TMP_Text _stopComponentButtonText;

    private bool _componentRunning = true;

    public event System.Action<string> RunComponentEvent;
    public event System.Action<string> StopComponentEvent;
    public event System.Action<string> DestroyComponentEvent;

    public void SetComponentNameText(string componentName)
    {
        _componentNameText.text = componentName;
    }

    public void RunComponent()
    {
        _componentRunning = true;

        _stopComponentButtonText.text = "Stop";

        if (RunComponentEvent != null)
        {
            RunComponentEvent(_componentNameText.text);
        }
    }

    public void StopComponent()
    {
        _componentRunning = false;

        _stopComponentButtonText.text = "Run";

        if (StopComponentEvent != null)
        {
            StopComponentEvent(_componentNameText.text);
        }
    }

    public void DestroyComponent()
    {
        if (DestroyComponentEvent != null)
        {
            DestroyComponentEvent(_componentNameText.text);
        }
    }

    public void SwitchRunComponent()
    {
        if (_componentRunning == false)
        {
            RunComponent();
        }
        else
        {
            StopComponent();
        }
    }
}
