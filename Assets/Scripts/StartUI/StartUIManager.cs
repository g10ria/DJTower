using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartUIManager : MonoBehaviour
{
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
    public TMP_InputField player1UsernameInputField;

    [Header("Join Game Screen Inputs")]
    public GameObject player2CenterGameInfo; // hidden until a game is joined
    public Button player2StartGameButton;
    public TMP_InputField player2UsernameInputField;
    public Button player2JoinButton;
    public TMP_InputField gamecodeInputField;

    public void Start()
    {
        baseScreen.SetActive(true);
        splashScreen.SetActive(true);
        hostScreen.SetActive(false);
        joinScreen.SetActive(false);

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
    public void OnHostButtonPress()
    {
        splashScreen.SetActive(false);
        hostScreen.SetActive(true);
        joinScreen.SetActive(false);
        Debug.Log("host button was pressed");
    }
    public void OnJoinButtonPressed()
    {
        splashScreen.SetActive(false);
        hostScreen.SetActive(false);
        joinScreen.SetActive(true);
        Debug.Log("join button was pressed");
    }

    // Host game screen routing
    public void OnPlayer1StartButtonPress()
    {
        // TODO: Check for player 2 info here
        Debug.Log("start game button pressed by player 1! TODO: attempt to start");
        Debug.Log("player 1 username: " + player1UsernameInputField.text);
    }

    // Join game screen routing
    public void OnPlayer2StartButtonPress()
    {
        // TODO: Check for player 1 info here
        Debug.Log("start game button pressed by player 2! TODO: attempt to start");
        Debug.Log("player 2 username: " + player2UsernameInputField.text);
    }
    public void OnPlayer2JoinPressed()
    {
        string gamecode = gamecodeInputField.text;
        // TODO: Search for game with specified gamecode here and attempt to join
        Debug.Log("join with gamecode pressed by player 2 with input " + gamecode + ". TODO: look for lobby");
        Debug.Log("assuming game found successfully, show center panel:");
        player2CenterGameInfo.SetActive(true);
    }
}
