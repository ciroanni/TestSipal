using UnityEngine;
using UnityEngine.UI;
using static TouchManipulator;

public class UIManipulationManager : MonoBehaviour
{
    [SerializeField] private ManipulationMode manipulationMode;
    private TouchManipulator touchManipulator;

    public void SetTouchManipulator(TouchManipulator manipulator)
    {
        touchManipulator = manipulator;
    }

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        switch (manipulationMode)
        {
            case ManipulationMode.Translate:
                touchManipulator.SetTranslateMode();
                break;
            case ManipulationMode.Scale:
                touchManipulator.SetScaleMode();
                break;
            case ManipulationMode.Rotate:
                touchManipulator.SetRotateMode();
                break;
            case ManipulationMode.Auto:
                touchManipulator.SetAutoMode();
                break;
        }
    }
}
