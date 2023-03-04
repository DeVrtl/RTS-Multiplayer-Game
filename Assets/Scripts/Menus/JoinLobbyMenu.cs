using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JoinLobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject _landingPage;
    [SerializeField] private TMP_InputField _addressInput;
    [SerializeField] private Button _join;

    private void OnEnable()
    {
        Network.ClientConnected += OnClientConnected;
        Network.ClientDisconnected += OnClientDisconnected;
    }

    private void OnDisable()
    {
        Network.ClientConnected -= OnClientConnected;
        Network.ClientDisconnected -= OnClientDisconnected;
    }

    public void Join()
    {
        string address = _addressInput.text;

        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();

        _join.interactable = false;
    }

    private void OnClientConnected()
    {
        _join.interactable = true;

        gameObject.SetActive(false);
        _landingPage.SetActive(false);
    }

    private void OnClientDisconnected()
    {
        _join.interactable = true;
    }
}
