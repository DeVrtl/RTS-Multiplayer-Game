using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildingsRepository : MonoBehaviour
{
    private List<Building> _myBuildings = new List<Building>();

    public List<Building> MyBuildings => _myBuildings;
}
