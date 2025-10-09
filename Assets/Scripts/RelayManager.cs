using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Netcode.Transports.UTP;


public class RelayManager : MonoBehaviour
{
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] TMP_Text codeText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private UnityTransport transport;
    // This string will be "wss" for WebGL builds and "dtls" for all other platforms
    private string connectionType = "dtls";
    async void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // This is a preprocessor directive that will only compile the code inside
        // if the build target is WebGL
        #if UNITY_WEBGL
            connectionType = "wss";
        // Also enable WebSockets in the transport for WebGL
        transport.UseWebSockets = true;
        #else
            transport.UseWebSockets = false;
        #endif
        
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.onClick.AddListener(CreateRelay);
        joinButton.onClick.AddListener(() => JoinRelay(joinInput.text));
    }

    async void CreateRelay()
    {
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        codeText.text = "Code: " + joinCode;

        var relayServerData = AllocationUtils.ToRelayServerData(allocation, connectionType);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();
        HideCanvas();
    }

    async void JoinRelay(string joinCode)
    {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        var relayServerData = AllocationUtils.ToRelayServerData(joinAllocation, connectionType);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
        HideCanvas();
    }
    
    void HideCanvas()
    {
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        joinInput.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
