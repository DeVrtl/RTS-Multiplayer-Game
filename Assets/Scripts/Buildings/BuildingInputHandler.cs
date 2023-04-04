using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingInputHandler : MonoBehaviour
{
    public void ShowPreview(Player player, Building building, GameObject buildingPreview, Renderer buildingRenderer)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (player.Resources < building.Price)
            return;

        buildingPreview = Instantiate(building.Preview);
        buildingRenderer = buildingPreview.GetComponentInChildren<Renderer>();

        buildingPreview.SetActive(false);
    }

    public void HidePreview(PlayerBuildings playerBuildings, Building building, GameObject buildingPreview, Camera camera, LayerMask mask)
    {
        if (buildingPreview == null)
            return;

        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
        {
            playerBuildings.CmdTryPlaceBuilding(building.Id, hit.point);
        }

        Destroy(buildingPreview);
    }

    public void UpdatePreview(PlayerBuildings playerBuildings, GameObject buildingPreview, BoxCollider buildingCollider, Renderer buildingRenderer, Camera camera, LayerMask mask)
    {
        Ray ray = camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, mask))
            return;

        buildingPreview.transform.position = hit.point;

        if (!buildingPreview.activeSelf)
            buildingPreview.SetActive(true);

        Color color = playerBuildings.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;

        buildingRenderer.material.SetColor("_BaseColor", color);
    }
}
