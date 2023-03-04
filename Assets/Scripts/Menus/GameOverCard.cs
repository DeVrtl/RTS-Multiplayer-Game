using TMPro;
using UnityEngine;
using Mirror;

public class GameOverCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _winnerName;
    [SerializeField] private GameObject _gameOverCanvas;

    private void Start()
    {
        GameOverHandler.ClientGameOvered += OnClientGameOvered;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientGameOvered -= OnClientGameOvered;
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void OnClientGameOvered(string winnerName)
    {
        _winnerName.text = $"{winnerName} Has Won!";

        _gameOverCanvas.SetActive(true);
    }
}