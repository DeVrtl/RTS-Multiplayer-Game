using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject _lobby;
    [SerializeField] private Button _start;
    [SerializeField] private TMP_Text[] _playerNames;

    private void Start()
    {
        Network.ClientConnected += OnClientConnected;
        Player.AuthorityPartyOwnerStateUpdated += OnAuthorityPartyOwnerStateUpdated;
        Player.ClientInfoUpdated += OnClientInfoUpdated;
    }

    private void OnDestroy()
    {
        Network.ClientConnected -= OnClientConnected;
        Player.AuthorityPartyOwnerStateUpdated -= OnAuthorityPartyOwnerStateUpdated;
        Player.ClientInfoUpdated -= OnClientInfoUpdated;
    }

    private void OnClientConnected()
    {
        _lobby.SetActive(true);
    }

    private void OnAuthorityPartyOwnerStateUpdated(bool state)
    {
        _start.gameObject.SetActive(state);
    }

    private void OnClientInfoUpdated()
    {
        List<Player> players = ((Network)NetworkManager.singleton).Players;

        for (int i = 0; i < players.Count; i++)
        {
            _playerNames[i].text = players[i].DisplayName;
        }

        for (int i = players.Count; i < _playerNames.Length; i++)
        {
            _playerNames[i].text = "Waiting For Player...";
        }

        _start.interactable = players.Count >= 2;
    }

    public void StartGame()
    {
        Player player = NetworkClient.connection.identity.GetComponent<Player>();
        player.CmdStartGame();
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.OnStopClient();

            SceneManager.LoadScene(0);
        }
    }
}
