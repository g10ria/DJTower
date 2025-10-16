using UnityEngine;
using UnityEngine.UI;

public class Player1DJInterface : MonoBehaviour
{

    private Button[] buttons;
    private CanvasGroup canvasGroup;

    public void Hide()
    {
        canvasGroup.alpha = 0f;          // Hide visuals
        canvasGroup.interactable = false; // Disable buttons
        canvasGroup.blocksRaycasts = false; // Disable clicks
    }

    public void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }
    void Start()
    {
    buttons = this.GetComponentsInChildren<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        KeyCode[] keyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
            KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R,
            KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F,
            KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V };

        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKey(keyCodes[i]))
            {
                buttons[i].image.color = buttons[i].colors.pressedColor;
            }
            else
            {
                buttons[i].image.color = buttons[i].colors.normalColor;
            }
        }
    }
    }
