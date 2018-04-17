using UnityEngine;

public class ComponentsView : MonoBehaviour
{
    [SerializeField]
    private GameObject _componentView;

    [SerializeField]
    private GameObject _componentPanelParent;

    [SerializeField]
    private GameObject _componentPanelPrefab;

    [SerializeField]
    private CodingBoxController _codingBoxController;

    private bool _componentViewShown;

    public void ShowComponentView()
    {
        UpdateComponentView();

        _componentView.SetActive(true);

        _componentViewShown = true;
    }

    public void HideComponentView()
    {
        _componentView.SetActive(false);

        _componentViewShown = false;
    }

    public void SwitchComponentView()
    {
        if (_componentViewShown == false)
        {
            ShowComponentView();
        }
        else
        {
            HideComponentView();
        }
    }

    public void RunComponent(string componentName)
    {
        _codingBoxController.EnableComponent(componentName);
    }

    public void StopComponent(string componentName)
    {
        _codingBoxController.DisableComponent(componentName);
    }

    public void DestroyComponent(string componentName)
    {
        _codingBoxController.DestroyComponent(componentName);

        UpdateComponentView();
    }

    private void UpdateComponentView()
    {
        if (_componentPanelParent.transform.childCount > 0)
        {
            foreach (var componentPanelController in _componentPanelParent.GetComponentsInChildren<ComponentPanelController>())
            {
                componentPanelController.RunComponentEvent -= RunComponent;
                componentPanelController.StopComponentEvent -= StopComponent;
                componentPanelController.DestroyComponentEvent -= DestroyComponent;

                Destroy(componentPanelController.gameObject);
            }
        }

        foreach (var dicts in _codingBoxController.GameobjectComponents)
        {
            foreach (var component in dicts.Value)
            {
                var componentPanel = Instantiate(_componentPanelPrefab, _componentPanelParent.transform);

                var componentPanelController = componentPanel.GetComponent<ComponentPanelController>();

                componentPanelController.SetComponentNameText(component.Key);

                // TODO change text field
                componentPanelController.RunComponentEvent += RunComponent;
                componentPanelController.StopComponentEvent += StopComponent;
                componentPanelController.DestroyComponentEvent += DestroyComponent;
            }
        }
    }

    private void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_codingBoxController);

        HideComponentView();
    }
}
