﻿using System.Collections.Generic;
using ProceduralToolkit;
using UnityEngine;

public class Mastermind : MonoBehaviour
{
    public Vector3[] spawnPositions;

    private WaypointCircuit circuit;

    private void Awake()
    {
        var obstacles = new GameObject("Obstacles");
        var pillarPrefab = Resources.Load<Transform>("Pillar");

        for (int x = -5; x < 5; x++)
        {
            for (int z = -5; z < 5; z++)
            {
                var clone = (Transform) Instantiate(pillarPrefab, new Vector3(x*30, 0, z*50), Quaternion.identity);
                clone.parent = obstacles.transform;
            }
        }

        circuit = new GameObject("Waypoints").AddComponent<WaypointCircuit>();
        var polygon = Polygon.Circle(100, 30);
        var waypoints = new List<Transform>();
        foreach (var vertex in polygon)
        {
            var waypoint = new GameObject("Waypoint " + vertex.x + " " + vertex.y);
            waypoint.transform.position = new Vector3(vertex.x, 0, vertex.y);
            waypoint.transform.parent = circuit.transform;
            waypoints.Add(waypoint.transform);
        }

        circuit.Waypoints = waypoints.ToArray();
        circuit.Initialize();

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