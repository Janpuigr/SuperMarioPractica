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
    public GoombaController m_Goomba;
    public Transform m_Player;
    private NavMeshAgent agent;
    private List<Transform> points; 
    private int currentPointIndex = 0;
    Vector3 m_targetPlayerPosition;
    bool hasSeen;

    // Start is called before the first frame update
    void Start()
    {
        hasSeen = false;
        m_Goomba = GetComponent<GoombaController>();
        agent = GetComponent<NavMeshAgent>();

        points = new List<Transform> { m_point1, m_point2, m_point3, m_point4, m_point5, m_point6 };

        MoveToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Goomba.m_SeesPlayer)
        {
            hasSeen = true;
            agent.destination = m_Player.position;
            agent.speed = 6.0f;
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextPoint();
        }
    }

    void MoveToNextPoint()
    {
        agent.speed = 3.5f;
        if (points.Count == 0) return;

        agent.destination = points[currentPointIndex].position;

        currentPointIndex = (currentPointIndex + 1) % points.Count;
    }
}
