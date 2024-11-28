using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavegationScript : MonoBehaviour
{
    public Transform m_point1;
    public Transform m_point2;
    public Transform m_point3;
    public Transform m_point4;
    public Transform m_point5;
    public Transform m_point6;

    private NavMeshAgent agent;
    private List<Transform> points; 
    private int currentPointIndex = 0; 

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        points = new List<Transform> { m_point1, m_point2, m_point3, m_point4, m_point5, m_point6 };

        MoveToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextPoint();
        }
    }

    void MoveToNextPoint()
    {
        if (points.Count == 0) return;

        agent.destination = points[currentPointIndex].position;

        currentPointIndex = (currentPointIndex + 1) % points.Count;
    }
}
