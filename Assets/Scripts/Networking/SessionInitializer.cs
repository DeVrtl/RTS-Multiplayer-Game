using UnityEngine;
using System.Collections.Generic;
using Mirror;
using UnityEngine.SceneManagement;

public class SessionInitializer : MonoBehaviour
{
    [SerializeField] private GameOverHandler _gameOverHandler;
    [SerializeField] private Headquarters _headquarter;

    public void Initialize(List<Player> players, NetworkManager networkManager)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Map"))
        {
            GameOverHandler gameOverHandler = Instantiate(_gameOverHandler);

            NetworkServer.Spawn(gameOverHandler.gameObject);

            foreach(Player player in players)
            {
                Headquarters unitBase = Instantiate(_headquarter, networkManager.GetStartPosition().position, Quaternion.identity);

                NetworkServer.Spawn(unitBase.gameObject, player.connectionToClient);
            }
        }
    }
}
