using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    private int carCount = 5;
    private WaypointCircuit circuit;
    private List<Car> cars = new List<Car>();
    private float circleRadius = 100;
    private Vector3 startPosition;
    private float carOffset = 7;
    private TrafficLights trafficLights;

    private void Awake()
    {
        GenerateWorld();
    }

    private void Start()
    {
        StartCountdown();
    }

    private void GenerateWorld()
    {
        var obstacles = new GameObject("Obstacles");
        var pillarPrefab = Resources.Load<Transform>("Pillar");

        var outerPillars = Polygon.Circle(circleRadius + carOffset*carCount + 10, 30);
        foreach (var pillar in outerPillars)
        {
            var clone = (Transform) Instantiate(pillarPrefab, new Vector3(pillar.x, 0, pillar.y), Quaternion.identity);
            clone.parent = obstacles.transform;
        }

        var innerPillars = Polygon.Circle(circleRadius - 10, 30);
        foreach (var pillar in innerPillars)
        {
            var clone = (Transform) Instantiate(pillarPrefab, new Vector3(pillar.x, 0, pillar.y), Quaternion.identity);
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
        startPosition = new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < carCount; i++)
        {
            var position = startPosition + Vector3.right*i*carOffset;
            var tracker = (WaypointProgressTracker) Instantiate(carPrefab, position, Quaternion.identity);
            tracker.circuit = circuit;
            var car = tracker.GetComponent<Car>();
            car.SetColors(RandomE.colorHSV, RandomE.colorHSV);
            car.Deactivate();
            cars.Add(car);
        }

        var characterPrefab = Resources.Load("Character");
        Instantiate(characterPrefab, cars[0].transform.position + Vector3.up*2, Quaternion.identity);
        //cars[0].Attach();

        var trafficLightsPrefab = Resources.Load<TrafficLights>("TrafficLights");
        var trafficLightsPosition = startPosition + Vector3.up*3 + Vector3.forward*2;
        trafficLights = (TrafficLights) Instantiate(trafficLightsPrefab, trafficLightsPosition, Quaternion.identity);
    }

    private void StartCountdown()
    {
        trafficLights.StartCountdown(() => Debug.Log("Start!"));
    }

    private void ActivateCars()
    {
        foreach (var car in cars)
        {
            car.Activate();
        }
    }
}