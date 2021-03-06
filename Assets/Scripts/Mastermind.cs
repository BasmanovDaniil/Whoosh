﻿using System.Collections;
using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public static bool started = false;

    private int carCount = 6;
    private WaypointCircuit circuit;
    private List<Car> cars = new List<Car>();
    private float circleRadius = 150;
    private float circleCount = 3;
    private Vector3 startPosition;
    private float carOffset = 5;
    private List<TrafficLights> trafficLights = new List<TrafficLights>();
    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    private GUIText counterText;
    private GUIText crosshairText;
    private int characterPoints = 0;
    private int aiPoints = 0;
    private float rampChance = 0.03f;
    private GameObject obstacles;
    private Transform pillarPrefab;
    private CheckPoint checkPointPrefab;
    private WaypointProgressTracker carPrefab;
    private Object characterPrefab;
    private List<Transform> ramps = new List<Transform>();

    private IEnumerator Start()
    {
        LoadPrefabs();
        yield return null;
        LoadUI();
        yield return null;
        GenerateTrack();
        yield return null;
        GenerateCars();
        yield return null;
        GenerateTrafficLights();
        GenerateCharacter();
        yield return null;
        StartCountdown();
        audio.Play();
    }

    private void LoadPrefabs()
    {
        obstacles = new GameObject("Obstacles");
        pillarPrefab = Resources.Load<Transform>("Pillar");
        checkPointPrefab = Resources.Load<CheckPoint>("CheckPoint");
        carPrefab = Resources.Load<WaypointProgressTracker>("Car");
        characterPrefab = Resources.Load("Character");
        ramps.Add(Resources.Load<Transform>("MiniRamp"));
        ramps.Add(Resources.Load<Transform>("Ramp"));
    }

    private void LoadUI()
    {
        var ui = Instantiate(Resources.Load("UI"));
        ui.name = "UI";
        counterText = GameObject.Find("UI/Counter").GetComponent<GUIText>();
        crosshairText = GameObject.Find("UI/Crosshair").GetComponent<GUIText>();
    }

    private void GenerateTrack()
    {
        // Circuit
        circuit = GetComponent<WaypointCircuit>();
        var path = new Polygon();
        var circleOffset = Vector2.right*circleRadius*2.5f;
        for (int i = 0; i < circleCount; i++)
        {
            var center = -circleOffset*i;
            var secondPath = Polygon.Circle(circleRadius, 30) + center;
            if (i%2 == 1)
            {
                secondPath.Reverse();
            }
            if (i != 0)
            {
                path.Insert(path.Count/2, center + circleOffset/2);
            }
            path.InsertRange(path.Count/2, secondPath);
            GenerateCircle(center, circleRadius);
        }

        for (int i = 0; i < circleCount; i++)
        {
            GenerateCheckPoints(path);
        }
    }

    private void GenerateCircle(Vector3 center, float radius)
    {
        // Pillars
        var innerPillars = Polygon.Circle(radius - 10, 30) + center;
        foreach (var pillar in innerPillars)
        {
            var clone = (Transform) Instantiate(pillarPrefab, new Vector3(pillar.x, 0, pillar.y), Quaternion.identity);
            clone.parent = obstacles.transform;
        }
    }

    private void GenerateCheckPoints(Polygon path)
    {
        var waypoints = new List<Vector3>();
        for (int i = 0; i < path.Count; i++)
        {
            var position = new Vector3(path[i].x, 0, path[i].y);
            waypoints.Add(position);
            var checkPoint = (CheckPoint) Instantiate(checkPointPrefab, position + Vector3.up*2, Quaternion.identity);
            checkPoints.Add(checkPoint);
            checkPoint.transform.parent = transform;
            checkPoint.callback = CheckPoint;
            checkPoint.index = i;
            if (i > 0)
            {
                if (Random.value < rampChance)
                {
                    var clone =
                        (Transform)
                            Instantiate(ramps.Random(), position, Quaternion.LookRotation(position - waypoints[i - 1]));
                    clone.parent = transform;
                }
            }
            else
            {
                checkPoint.Disable();
            }
        }
        circuit.waypoints = waypoints.ToArray();
        circuit.Initialize();
    }

    private void GenerateCars()
    {
        // Cars
        startPosition = new Vector3(circleRadius, 0, 0);
        for (int i = 0; i < carCount; i++)
        {
            var position = startPosition + (Vector3.back + Vector3.right)*i*carOffset;
            var tracker = (WaypointProgressTracker) Instantiate(carPrefab, position, Quaternion.identity);
            tracker.circuit = circuit;
            var car = tracker.GetComponent<Car>();
            car.Initialize();
            car.SetColors(RandomE.colorHSV, RandomE.colorHSV);
            car.Deactivate();
            cars.Add(car);
        }
    }

    private void GenerateTrafficLights()
    {
        // Traffic lights
        var trafficLightsPrefab = Resources.Load<TrafficLights>("TrafficLights");

        foreach (var car in cars)
        {
            var trafficLightsPosition = car.transform.position + Vector3.up*5 + Vector3.forward*10;
            trafficLights.Add(
                (TrafficLights) Instantiate(trafficLightsPrefab, trafficLightsPosition, Quaternion.identity));
        }
    }

    private void GenerateCharacter()
    {
        // Character
        Instantiate(characterPrefab, cars[cars.Count - 1].transform.position + Vector3.up*2, Quaternion.identity);
        cars[cars.Count - 1].Attach();
    }

    private void StartCountdown()
    {
        for (int i = 0; i < trafficLights.Count; i++)
        {
            if (i == 0)
            {
                trafficLights[i].StartCountdown(3, () => { }, ActivateCars);
            }
            else
            {
                trafficLights[i].StartCountdown(3, () => { }, () => { });
            }
        }
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
        if (counterText != null)
        {
            counterText.text = string.Format("You {0}\nAI {1}", characterPoints, aiPoints);
        }
    }

    private void CheckPoint(int index, Car firstCar)
    {
        if (started)
        {
            if (checkPoints[index].disabled)
            {
                return;
            }
            checkPoints[index].Disable();
            if (firstCar == Character.instance.car)
            {
                characterPoints++;
            }
            else
            {
                aiPoints++;
            }
            if (index > checkPoints.Count/2)
            {
                checkPoints[0].Reset();
            }
            if (index == 0)
            {
                Finish();
            }
        }
    }

    private void Finish()
    {
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