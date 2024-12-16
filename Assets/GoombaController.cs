using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public float attackCooldown = 2.0f;
    public float retreatDistance = 2.0f; 
    private bool canAttack = true;
    private Vector3 initialPosition;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            
            Debug.Log("Goomba ataca a Mario.");
            StartCoroutine(AttackCooldownRoutine());

           
            Vector3 retreatDirection = (transform.position - other.transform.position).normalized;
            retreatDirection.y = 0; 
            transform.position += retreatDirection * retreatDistance;
        }
        if (other.CompareTag("MarioPunch"))
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator AttackCooldownRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown); 
        canAttack = true; 
    }

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
        initialPosition = transform.position; // Almacena la posición inicial
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
            //Debug.Log("ESTA CERCA");

            m_SeesPlayer = true;
            m_Animator.SetTrigger("Alert");
            m_Animator.SetBool("GoombaRun",true);

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
