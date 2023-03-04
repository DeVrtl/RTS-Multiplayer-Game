using TMPro;
using UnityEngine;
using Mirror;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text _resources;

    private Player _player;

    private void Start()
    {
        _player = NetworkClient.connection.identity.GetComponent<Player>();

        OnClientResourcesUpdated(_player.Resources);

        _player.ClientResourcesUpdated += OnClientResourcesUpdated;
    }

    private void OnDestroy()
    {
        _player.ClientResourcesUpdated -= OnClientResourcesUpdated;
    }

    private void OnClientResourcesUpdated(int amout)
    {
        _resources.text = $"Resources: {amout}";
    }
}
