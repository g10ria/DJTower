using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
    private RelayManager relayManager;

    [Header("UI Screens")]
    public GameObject splashScreen;
    public GameObject hostScreen;
    public GameObject joinScreen;
    public GameObject baseScreen;

    [Header("Splash Screen Buttons")]
    public Button hostGameButton;
    public Button joinGameButton;
    public Button settingsButton;

    [Header("Host Game Screen Inputs")]
    public Button player1StartGameButton;
    public TMP_Text  gamecodeTemp;
    public TMP_InputField player1UsernameInputField;

    [Header("Join Game Screen Inputs")]
    public GameObject player2CenterGameInfo; // hidden until a game is joined
    public Button player2StartGameButton;
    public TMP_InputField player2UsernameInputField;
    public Button player2JoinButton;
    public TMP_InputField gamecodeInputField;

    [Header("Level Select")]
    public GameObject levelSelectParent;

    public void Start()
    {
        baseScreen.SetActive(true);
        splashScreen.SetActive(true);
        hostScreen.SetActive(false);
        joinScreen.SetActive(false);
        levelSelectParent.SetActive(false);

        player2CenterGameInfo.SetActive(false);
    }

    public void OnEnable()
    {
        // Hook up all the listeners
        hostGameButton.onClick.AddListener(OnHostButtonPress);
        joinGameButton.onClick.AddListener(OnJoinButtonPressed);

        player1StartGameButton.onClick.AddListener(OnPlayer1StartButtonPress);
        player2StartGameButton.onClick.AddListener(OnPlayer2StartButtonPress);
        player2JoinButton.onClick.AddListener(OnPlayer2JoinPressed);
    }

    // Splash screen routing
    public async void OnHostButtonPress()
    {
        // Blocks and returns the resulting game ID
        string gamecode = await RelayManager.Instance.CreateRelay();
        gamecodeTemp.text = gamecode;

        splashScreen.SetActive(false);
        hostScreen.SetActive(true);
        joinScreen.SetActive(false);
        Debug.Log("hosting new game with gamecode " + gamecode);

        joinGameButton.interactable = false;
    }
    public void OnJoinButtonPressed()
    {
        splashScreen.SetActive(false);
        hostScreen.SetActive(false);
        joinScreen.SetActive(true);
        Debug.Log("join button was pressed");

        hostGameButton.interactable = false;
    }

    private void ShowLevels()
    {
        Button[] buttons = levelSelectParent.GetComponentsInChildren<Button>();

        string[] levels = RelayManager.Instance.levels;
        for (int i = 0; i < levels.Length; i++)
        {
            int index = i; // prevent closure issue
            Debug.Log("hooked up level " + levels[i] + " to button idx " + i);
            buttons[i].GetComponentInChildren<TMP_Text>().text = levels[i];
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() =>
            {
                RelayManager.Instance.LoadLevel(index);
            });
        }
        // set other buttons to non-interactable. proper way to do this later is to
        // just spawn in the required # of buttons
        for (int i = levels.Length; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
            buttons[i].GetComponentInChildren<TMP_Text>().text = " ";
        }

        levelSelectParent.SetActive(true);
    }

    // Host game screen routing
    private void OnPlayer1StartButtonPress()
    {
        Debug.Log("start game button pressed by player 1! TODO: check that a player 2 has actually joined, then retrieve the usernames");
        // Debug.Log("player 1 username: " + player1UsernameInputField.text);

        RelayManager.Instance.SetPlayerUsernames("player1Username", "player2Username");
        // Start game as host
        ShowLevels();
    }

    // Join game screen routing
    private void OnPlayer2StartButtonPress()
    {
        Debug.Log("start game button pressed by player 2! TODO: retrieve the players' usernames properly");
        // Debug.Log("player 2 username: " + player2UsernameInputField.text);

        RelayManager.Instance.SetPlayerUsernames("player1Username", "player2Username");
        // Start game as player
        ShowLevels();
    }

    public async void OnPlayer2JoinPressed()
    {
        // TODO later: disable UI while this function is blocking
        string gamecode = gamecodeInputField.text;

        // Blocks and returns the resulting success/failure
        bool joinSuccess = await RelayManager.Instance.JoinRelay(gamecode);

        if (joinSuccess)
        {
            Debug.Log("successfully joined game with ID " + gamecode);
            player2CenterGameInfo.SetActive(true);
            player2StartGameButton.interactable = false; // TODO: reevaluate this. is host the only one who can hit start?
        }
        else
        {
            Debug.Log("game with code " + gamecode + " not found");
            // TODO handle this case later
        }
    }
}
