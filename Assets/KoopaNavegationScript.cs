using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KoopaNavegationScript : MonoBehaviour
{
    public Transform m_point1;
    public Transform m_point2;
    public Transform m_point3;
    public KoopaScript m_Koopa;
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
        m_Koopa = GetComponent<KoopaScript>();
        agent = GetComponent<NavMeshAgent>();

        points = new List<Transform> { m_point1, m_point2, m_point3};

        MoveToNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Koopa.m_SeesPlayer)
        {
            hasSeen = true;
            agent.destination = m_Player.position;
            agent.speed = 6.0f;
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            MoveToNextPoint();
        }
        if (ShellState())
        {
            agent.speed = 0.0f;
        }
    }

    void MoveToNextPoint()
    {
        agent.speed = 3.5f;
        if (points.Count == 0) return;

        agent.destination = points[currentPointIndex].position;

        currentPointIndex = (currentPointIndex + 1) % points.Count;
    }
    bool ShellState()
    {
        if (m_Koopa.m_ShellMode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
