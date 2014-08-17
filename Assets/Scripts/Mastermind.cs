using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    private int carCount = 5;
    private WaypointCircuit circuit;
    private List<Car> cars = new List<Car>();
    private float circleRadius = 100;

    private void Awake()
    {
        GenerateWorld();
    }

    private void GenerateWorld()
    {
        var obstacles = new GameObject("Obstacles");
        var pillarPrefab = Resources.Load<Transform>("Pillar");

        var outerPillars = Polygon.Circle(circleRadius + 10, 30);
        foreach (var pillar in outerPillars)
        {
            var clone = (Transform)Instantiate(pillarPrefab, new Vector3(pillar.x, 0, pillar.y), Quaternion.identity);
            clone.parent = obstacles.transform;
        }

        var innerPillars = Polygon.Circle(circleRadius - 10, 30);
        foreach (var pillar in innerPillars)
        {
            var clone = (Transform)Instantiate(pillarPrefab, new Vector3(pillar.x, 0, pillar.y), Quaternion.identity);
            clone.parent = obstacles.transform;
        }

        circuit = GetComponent<WaypointCircuit>();
        var path = Polygon.Circle(circleRadius, 30);
        var waypoints = new List<Vector3>();
        foreach (var vertex in path)
        {
            waypoints.Add(new Vector3(vertex.x, 0, vertex.y));
        }

        circuit.waypoints = waypoints.ToArray();
        circuit.Initialize();

        var carPrefab = Resources.Load<WaypointProgressTracker>("Car");

        for (int i = 0; i < carCount; i++)
        {
            var position = new Vector3(circleRadius + i*7, 0, 5);
            var tracker = (WaypointProgressTracker) Instantiate(carPrefab, position, Quaternion.identity);
            tracker.circuit = circuit;
            var car = tracker.GetComponent<Car>();
            car.SetColors(RandomE.colorHSV, RandomE.colorHSV);
            cars.Add(car);
        }

        var characterPrefab = Resources.Load("Character");
        Instantiate(characterPrefab, cars[0].transform.position + Vector3.up*2, Quaternion.identity);
        cars[0].Attach();
    }
}