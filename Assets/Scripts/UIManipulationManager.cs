using System;
using UnityEngine;
using UnityEngine.UI;
using static TouchManipulator;

public class UIManipulationManager : MonoBehaviour
{
    [SerializeField] private ManipulationMode manipulationMode;
    private TouchManipulator touchManipulator;
    private Button button;

    public void SetTouchManipulator(TouchManipulator manipulator)
    {
        touchManipulator = manipulator;
    }

    void Awake()
    {
        if (TryGetComponent<Button>(out Button buttonComponent))
        {
            button = buttonComponent;
            button.onClick.AddListener(OnButtonClick);
        }
        else if (TryGetComponent<Toggle>(out Toggle toggle))
        {
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    void Update()
    {
        if(touchManipulator != null && touchManipulator.currentMode != manipulationMode)
        {
            if (button != null)
            {
                button.interactable = true; // Re-enable button if mode changes
            }
        }
    }

    private void OnButtonClick()
    {
        switch (manipulationMode)
        {
            case ManipulationMode.Translate:
                touchManipulator.SetTranslateMode();
                button.interactable = false; // Disable button after click
                break;
            case ManipulationMode.Scale:
                touchManipulator.SetScaleMode();
                button.interactable = false; // Disable button after click
                break;
            case ManipulationMode.Rotate:
                touchManipulator.SetRotateMode();
                button.interactable = false; // Disable button after click
                break;
            case ManipulationMode.Auto:
                touchManipulator.SetAutoMode();
                break;
        }
    }

    private void OnToggleValueChanged(bool value)
    {
        OnButtonClick();
    }
}
