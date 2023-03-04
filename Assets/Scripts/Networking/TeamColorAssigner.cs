using UnityEngine;
using Mirror;

public class TeamColorAssigner : NetworkBehaviour
{
    [SerializeField] private Renderer[] _colorRenderers;

    [SyncVar(hook = nameof(HandleTeamColorUpdated))] private Color _team;

    public override void OnStartServer()
    {
        Player player = connectionToClient.identity.GetComponent<Player>();

        _team = player.TeamColor;
    }

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach(Renderer colorRenderer in _colorRenderers)
        {
            colorRenderer.material.SetColor("_BaseColor", newColor);
        }
    }
}
