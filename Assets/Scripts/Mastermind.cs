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
    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    private GUIText counterText;
    private GUIText crosshairText;
    private int characterPoints = 0;
    private int aiPoints = 0;
    private bool started = false;

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
        var checkPointPrefab = Resources.Load<CheckPoint>("CheckPoint");
        var carPrefab = Resources.Load<WaypointProgressTracker>("Car");
        var characterPrefab = Resources.Load("Character");

        var ui = Instantiate(Resources.Load("UI"));
        ui.name = "UI";
        counterText = GameObject.Find("UI/Counter").GetComponent<GUIText>();
        crosshairText = GameObject.Find("UI/Crosshair").GetComponent<GUIText>();

        // Pillars
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

        // Circuit
        circuit = GetComponent<WaypointCircuit>();
        var path = Polygon.Circle(circleRadius, 30);
        var waypoints = new List<Vector3>();
        foreach (var vertex in path)
        {
        }
        for (int i = 0; i < path.Count; i++)
        {
            var position = new Vector3(path[i].x, 0, path[i].y);
            waypoints.Add(position);
            var checkPoint = (CheckPoint) Instantiate(checkPointPrefab, position, Quaternion.identity);
            checkPoints.Add(checkPoint);
            checkPoint.transform.parent = transform;
            checkPoint.callback = CheckPoint;
            checkPoint.index = i;
        }
        checkPoints[0].Disable();
        circuit.waypoints = waypoints.ToArray();
        circuit.Initialize();

        // Cars
        startPosition = new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < carCount; i++)
        {
            var position = startPosition + Vector3.right*i*carOffset;
            var tracker = (WaypointProgressTracker) Instantiate(carPrefab, position, Quaternion.identity);
            tracker.circuit = circuit;
            var car = tracker.GetComponent<Car>();
            car.Initialize();
            car.SetColors(RandomE.colorHSV, RandomE.colorHSV);
            car.Deactivate();
            cars.Add(car);
        }

        // Character
        Instantiate(characterPrefab, cars[0].transform.position + Vector3.up*2, Quaternion.identity);
        cars[0].Attach();

        // Traffic lights
        var trafficLightsPrefab = Resources.Load<TrafficLights>("TrafficLights");
        var trafficLightsPosition = startPosition + Vector3.up*5 + Vector3.forward*10;
        trafficLights = (TrafficLights) Instantiate(trafficLightsPrefab, trafficLightsPosition, Quaternion.identity);
    }

    private void StartCountdown()
    {
        trafficLights.StartCountdown(10, () => Debug.Log("Byr!"), () =>
        {
            Debug.Log("Start!");
            ActivateCars();
        });
    }

    private void ActivateCars()
    {
        foreach (var car in cars)
        {
            car.Activate();
        }
        started = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Application.LoadLevel(0);
        }
        counterText.text = string.Format("You {0}\nAI {1}", characterPoints, aiPoints);
    }

    private void CheckPoint(int index, Car firstCar)
    {
        if (started)
        {
            if (firstCar == Character.instance.car)
            {
                characterPoints++;
            }
            else
            {
                aiPoints++;
            }
            checkPoints[index].Disable();
            if (index > checkPoints.Count/2)
            {
                checkPoints[0].Reset();
            }
            if (index == 0)
            {
                Finish();
            }
        }
        else
        {
        }
    }

    private void Finish()
    {
        foreach (var car in cars)
        {
            car.Deactivate();
        }
        started = false;

        if (characterPoints > aiPoints)
        {
            crosshairText.text = "Win!";
        }
        else if (characterPoints < aiPoints)
        {
            crosshairText.text = "Lost!";
        }
        else
        {
            crosshairText.text = "Draw!";
        }
    }
}