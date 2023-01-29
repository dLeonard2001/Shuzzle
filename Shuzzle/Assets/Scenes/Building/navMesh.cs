using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class navMesh : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    [SerializeField] 
    private NavMeshAgent agent;

    [SerializeField] 
    private NavMeshSurface surface;

    [SerializeField] 
    private List<Transform> waypoints;

    private int starting_waypoint;
    private bool isPatrolling;
    private void Start()
    {
        surface.BuildNavMesh();
        starting_waypoint = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 0 or Keycode.mouse0 is left click
        // if (Input.GetMouseButtonDown(0))
        // {
        //     Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         agent.SetDestination(hit.point);
        //     }
        // }

        if (!isPatrolling)
        {
            Patrol();
        }
        else if(agent.remainingDistance <= 0.01)
        {
            isPatrolling = false;
        }
    }


    private void Patrol()
    {
        isPatrolling = true;

        // make our AI choose a random path based on where it can go from its current position
            // current iteration is extremely messy/unreadable, no scalability, 
                // better ideas:
                    // use a tree
                    // linked lists?
        int nextWaypoint;
        nextWaypoint = Random.Range(0, 2);

        if (starting_waypoint == 0) // on waypoint 0
        {
            starting_waypoint = Random.Range(1, 5);
        }
        else if (starting_waypoint == 1) // on waypoint 1
        {
            if (nextWaypoint == 0)
            {
                starting_waypoint = 2;
            }
            else
            {
                starting_waypoint = 4;
            }
        }
        else if (starting_waypoint == 2) // on waypoint 2
        {
            if (nextWaypoint == 0)
            {
                starting_waypoint = 1;
            }
            else
            {
                starting_waypoint = 3;
            }
        }
        else if (starting_waypoint == 3) // on waypoint 3
        {
            if (nextWaypoint == 0)
            {
                starting_waypoint = 2;
            }
            else
            {
                starting_waypoint = 4;
            }
        }else if (starting_waypoint == 4) // on waypoint 4
        {
            if (nextWaypoint == 0)
            {
                starting_waypoint = 3;
            }
            else
            {
                starting_waypoint = 1;
            }
        }

        agent.SetDestination(waypoints[starting_waypoint].position);
    }
}
