using ProceduralToolkit;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public WaypointCircuit circuit;
    public Vector3[] spawnPositions;

    private void Awake()
    {
        var carPrefab = Resources.Load<WaypointProgressTracker>("Car");
        foreach (var spawnPosition in spawnPositions)
        {
            var tracker = (WaypointProgressTracker) Instantiate(carPrefab, spawnPosition, Quaternion.identity);
            tracker.circuit = circuit;
            var car = tracker.GetComponent<Car>();
            car.SetColors(RandomE.colorHSV, RandomE.colorHSV);
        }

        var characterPrefab = Resources.Load("Character");
        Instantiate(characterPrefab, spawnPositions[0] + Vector3.up*2, Quaternion.identity);
    }
}