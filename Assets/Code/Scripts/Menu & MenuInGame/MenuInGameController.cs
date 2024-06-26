using UnityEngine;

public class MenuInGameController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Canvas menuInGameCanvas;

    private void OnEnable()
    {
        PauseManager.IsPaused += MenuEnable;
    }

    private void OnDisable()
    {
        PauseManager.IsPaused -= MenuEnable;
    }

    private void MenuEnable(bool value)
    {
        if (!menuInGameCanvas)
        {
            Debug.LogWarning("MenuInGameCanvas not assigned");
            return;
        }

        menuInGameCanvas.gameObject.SetActive(value);
    }
}
