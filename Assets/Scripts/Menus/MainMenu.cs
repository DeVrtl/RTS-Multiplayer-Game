using UnityEngine;
using Mirror;
using Steamworks;
using System.IO;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _landingPagePanel = null;
    [SerializeField] private bool _useSteam = false;

    protected Callback<LobbyCreated_t> LobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> GameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> LobbyEntered;

    private void Start()
    {
        if (!_useSteam) { return; }

        LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        _landingPagePanel.SetActive(false);

        if (_useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            _landingPagePanel.SetActive(true);
            return;
        }

        NetworkManager.singleton.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress", SteamUser.GetSteamID().ToString());
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) { return; }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

        NetworkManager.singleton.networkAddress = hostAddress;
        NetworkManager.singleton.StartClient();

        _landingPagePanel.SetActive(false);
    }
}