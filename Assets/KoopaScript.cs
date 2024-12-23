using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaScript : MonoBehaviour, IRestartGameElement
{
    CharacterController characterController;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    public GameObject m_Player;
    MarioController m_Mario;
    public LayerMask m_SigthLayerMask;
    public float m_ConeAngle = 60f;
    public float m_MaxDistanceToSeePlayer = 20.0f;
    public bool m_SeesPlayer;

    public float attackCooldown = 2.0f;
    public float retreatDistance = 2.0f;
    private bool canAttack = true;
    private Vector3 initialPosition;
    public GameObject m_Shell;
    public GameObject m_Koopa;
    public bool m_ShellMode;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canAttack)
        {
            Vector3 retreatDirection = (transform.position - other.transform.position).normalized;
            retreatDirection.y = 0;
            transform.position += retreatDirection * retreatDistance;
        }
        if (other.CompareTag("MarioPunch"))
        {
            m_Koopa.SetActive(false);
            Kill();
        }
        if (other.CompareTag("KoopaShell"))
        {
            FullKill();
        }

    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        m_Shell.transform.SetParent(m_Koopa.transform);
        m_ShellMode = false;
        m_Shell.SetActive(false);
        m_SeesPlayer = false;
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        GameManager.GetGameManager().AddRestartGameElement(this);
        initialPosition = transform.position; // Almacena la posici�n inicial
    }

    // Update is called once per frame
    private void Update()
    {

        Vector3 l_PlayerPostion = m_Player.transform.position;
        Vector3 l_EnemyPosition = transform.position + Vector3.up * 1.6f;

        Vector3 l_Direction = l_EnemyPosition - l_PlayerPostion;

        float l_Distance = l_Direction.magnitude;

        Ray l_Ray = new Ray(l_PlayerPostion, l_Direction);


        if (l_Distance < m_MaxDistanceToSeePlayer)
        {
            m_SeesPlayer = true;
        }
        else
        {
            m_SeesPlayer = false;
        }
    }
    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_Koopa.SetActive(true);
        characterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        characterController.enabled = true;
        m_ShellMode = false;
        m_Shell.SetActive(false);
        m_Shell.transform.SetParent(m_Koopa.transform);
        m_Shell.transform.position = m_StartPosition;

    }
    public void Kill()
    {
        m_Shell.transform.SetParent(null);
        m_Koopa.SetActive(false);
        ActivateShell();
        gameObject.SetActive(false);
    }
    public void FullKill()
    {
        gameObject.SetActive(false);
    }
    public void ActivateShell()
    {
        m_ShellMode = true;
        m_Shell.SetActive(true);
    }
}
