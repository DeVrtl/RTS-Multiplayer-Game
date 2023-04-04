using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitsRepository : MonoBehaviour
{
    private List<Unit> _myUnits = new List<Unit>();
    public List<Unit> MyUnits => _myUnits;
}
