using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour,IRestartGameElement
{
    Animator m_Animator;
    CharacterController characterController;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    public GameObject m_Player;
    public LayerMask m_SigthLayerMask;
    public float m_ConeAngle = 60f;
    public float m_MaxDistanceToSeePlayer = 20.0f;
    public bool m_SeesPlayer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        m_SeesPlayer = false;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameManager.GetGameManager().AddRestartGameElement(this);
    }
    private void Update()
    {
        
        Vector3 l_PlayerPostion = m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position + Vector3.up * 1.6f;

        Vector3 l_Direction = l_EnemyPosition - l_PlayerPostion;

        float l_Distance = l_Direction.magnitude;



        Ray l_Ray = new Ray(l_PlayerPostion, l_Direction);


        if (l_Distance < m_MaxDistanceToSeePlayer)
        {
            Debug.Log("ESTA CERCA");

            m_SeesPlayer = true;
            m_Animator.SetTrigger("Alert");
            m_Animator.SetBool("GoombaRun",true);
            l_Direction /= l_Distance;
            float l_DotAngle = Vector3.Dot(l_Direction, transform.forward);

            if (Physics.Raycast(l_Ray, l_Distance, m_SigthLayerMask.value))
            {
                if (l_DotAngle >= Mathf.Cos(m_ConeAngle * Mathf.Deg2Rad / 2.0f))
                {
                    Debug.Log("VE PLAYER");
                }
            }
        }
        else
        {
            m_SeesPlayer = false;
            m_Animator.SetBool("GoombaRun", false);
        }
    }
    public void RestartGame()
    {
        gameObject.SetActive(true);
        characterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        characterController.enabled = true;
    }

    public void Kill()
    {
        gameObject.SetActive(false);
    }
}
