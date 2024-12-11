using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class MarioController : MonoBehaviour, IRestartGameElement
{
    public enum TpunchType
    {
        RIGHT_HAND = 0,
        LEFT_HAND,
        KICK
    }
    public float m_GoombaHitForce = 10f; 
    private Vector3 pushDirection = Vector3.zero;
    private float pushBackTime = 0f;

    public GameObject UIDeadCanva;
    public static Action OnPlayerDeath;

    public Image LifeImage;
    public Animation m_Animation;
    public AnimationClip m_IdleAnimationClip;
    public AnimationClip m_ShowAnimationClip;
    GameManager m_GameManager;
    CharacterController m_CharacterController;
    static public Animator m_Animator;
    public Camera m_Camera;
    public float m_HorizontalSpeed = 0.0f;
    public float m_GoombaHitSpeed = 8.0f;
    Checkpoint m_CurrentCheckpoint;
    public float m_BridgeForce = 20.0f;
    public float m_WalkSpeed = 2.0f;
    public float m_RunSpeed = 8.0f;
    public float m_LerpRotationPct = 0.8f;
    float m_VerticalSpeed = 0.0f;

    [Header("Punch")]
    public float m_PunchComboAvailableTime = 1.3f;
    int m_CurrentPunchId = 0;
    float m_lastPunchTime;

    [Header("Punch Colliders")]
    public GameObject m_LeftHandPunchHitCollider;
    public GameObject m_RightHandPunchHitCollider;
    public GameObject m_RightFootKickHitCollider;


    [Header("Input")]
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public int m_PunchHitButton = 0;
    public KeyCode m_JumpKeyCode = KeyCode.Space;


    Vector3 m_StartPosition;
    Quaternion m_StartRotation;


    [Header("Jump")]

    public float m_JumpVerticalSpeed = 5.0f;
    public float m_killJumpVerticalSpeed = 8.0f;
    public float m_WaitStartJumpTime = 0.12f;
    public float m_MaxAngleNeededToKillGoomba = 15.0f;
    public float m_MinVerticalSpeedTokillGoomba = -1.0f;

    public float m_SecondJumpMultiplier = 1.5f; 
    public float m_ThirdJumpMultiplier = 2f;
    public float m_JumpComboTime = 0.5f; 
    private int m_JumpCount = 0; 
    private float m_LastJumpTime; 
    private float m_LastLandTime; 
    private bool m_CanJump = true;
    public float m_ResetJumpTime = 2.0f;
    public float m_LandCooldownTime = 0.2f;
    private bool m_IsDead = false; 
    public Transform m_RespawnPoint;

    [Header("Elevator")]
    public float m_MaxAngleToAttachElevator=8.0f;
    Collider m_CurrentElevator = null;
    
    private float m_IdleTime = 0.0f; 
    private bool m_HasMovement = false;


    [Header("Long Jump Settings")]
    public float m_LongJumpForwardForce = 10f;
    public float m_LongJumpVerticalForce = 5f;

    [Header("Wall Jump Settings")]
    public float m_WallJumpForce = 7f;
    public LayerMask m_WallLayer;

    [Header("Hit Settings")]
    public float m_HitRecoveryTime = 1.0f;
    private bool m_IsHit = false;

    private float m_pushForce = 10.0f;

    private CharacterController characterController;
    private Vector3 knockbackDirection;
    private float knockbackForce = 5.0f; 
    private float knockbackDuration = 0.2f;
    private float knockbackTimer = 0.0f;


    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("CharacterController is not assigned in Mario!");
        }
        m_Animator = GetComponent<Animator>();
        m_GameManager = GetComponent<GameManager>();
    }

    void Start()
    {
        m_Animator.fireEvents = false;
        LifeImage.fillAmount = 1f;
        m_LeftHandPunchHitCollider.gameObject.SetActive(false);
        m_RightHandPunchHitCollider.gameObject.SetActive(false);
        m_RightFootKickHitCollider.gameObject.SetActive(false);
        GameManager.GetGameManager().AddRestartGameElement(this);
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        UIDeadCanva.SetActive(false);
    }

    void Update()
    {
        if (IsGrounded() && m_VerticalSpeed <= 0.0f)
        {
            m_LastLandTime = Time.time; 
        }
        Vector3 l_Forward = m_Camera.transform.forward;
        Vector3 l_Right = m_Camera.transform.right;
        l_Forward.y = 0.0f;
        l_Right.y = 0.0f;
        l_Forward.Normalize();
        l_Right.Normalize();

        m_HasMovement = false;
        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement = l_Right;
            m_HasMovement = true;
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement = -l_Right;
            m_HasMovement = true;
        }

        if (Input.GetKey(m_UpKeyCode))
        {
            l_Movement += l_Forward;
            m_HasMovement = true;
        }
        else if (Input.GetKey(m_DownKeyCode))
        {
            l_Movement -= l_Forward;
            m_HasMovement = true;
        }

        if (!m_HasMovement)
        {
            m_IdleTime += Time.deltaTime; 
        }
        else
        {
            m_IdleTime = 0.0f; 
        }

       
        if (m_IdleTime >= 10.0f) 
        {
            m_Animator.SetBool("SpecialIdle", true); 
        }
        else
        {
            m_Animator.SetBool("SpecialIdle", false); 
        }
       
        
        float l_Speed = 0.0f;

        if (m_HasMovement)
        {
            if (Input.GetKey(m_RunKeyCode))
            {
                l_Speed = m_RunSpeed;
                m_Animator.SetFloat("Speed", 1.0f);
            }
            else
            {
                l_Speed = m_WalkSpeed;
                m_Animator.SetFloat("Speed", 0.2f);
            }

           
            Quaternion l_desiredRotation = Quaternion.LookRotation(l_Movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, l_desiredRotation,
                m_LerpRotationPct * Time.deltaTime);
        }
        else
        {
            m_Animator.SetFloat("Speed", 0.0f);
        }

        if (Time.time - m_LastJumpTime > m_ResetJumpTime)
        {
            m_JumpCount = 0;
        }

        
        if (!m_CanJump && IsGrounded() && Time.time - m_LastLandTime > m_LandCooldownTime)
        {
            m_CanJump = true;
        }

        if (Input.GetKeyDown(m_JumpKeyCode) && CanJump())
        {
            HandleJump();
        }

        l_Movement.Normalize();
        l_Movement = l_Movement * l_Speed * Time.deltaTime;

        m_HorizontalSpeed += Physics.gravity.x * Time.deltaTime;
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f)
            m_Animator.SetBool("Falling", false);
        else
            m_Animator.SetBool("Falling", true);

        if (((l_CollisionFlags & CollisionFlags.Below) != 0 && m_VerticalSpeed < 0.0f) ||
                (l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
                m_VerticalSpeed = 0.0f;

            UpdatePunch();

        m_CharacterController.Move(l_Movement);

        UpdateElevator();

        if (Input.GetKey(m_RunKeyCode) && Input.GetKeyDown(m_JumpKeyCode) && IsGrounded())
        {
            PerformLongJump();
        }

        if (!IsGrounded() && Input.GetKeyDown(m_JumpKeyCode) && IsTouchingWall())
        {
            PerformWallJump();
        }

        if (knockbackTimer > 0)
        {
            if (characterController != null)
            {
                if (knockbackDirection != Vector3.zero) 
                {
                    characterController.Move(knockbackDirection * knockbackForce * Time.deltaTime);
                    knockbackTimer -= Time.deltaTime; 
                }
                else
                {
                    Debug.LogError("Knockback direction is zero!");
                }
            }
            else
            {
                Debug.LogError("CharacterController is not assigned!");
            }
        }

        if (pushBackTime > 0f)
        {
            
            CharacterController characterController = GetComponent<CharacterController>();

            if (characterController != null)
            {
               
                characterController.Move(pushDirection * m_GoombaHitForce * Time.deltaTime);
                pushBackTime -= Time.deltaTime; 
            }
        }
    }

    bool IsTouchingWall()
    {
        return Physics.Raycast(transform.position, transform.right, 1f, m_WallLayer) ||
               Physics.Raycast(transform.position, -transform.right, 1f, m_WallLayer);
    }

    void PerformWallJump()
    {
        m_Animator.SetTrigger("WallJumpTrigger");
        m_VerticalSpeed = m_WallJumpForce;
        Vector3 wallJumpDirection = -transform.right; 
        m_CharacterController.Move(wallJumpDirection * Time.deltaTime);
    }


    void PerformLongJump()
    {
        m_Animator.SetTrigger("LongJumpTrigger");
        Vector3 jumpDirection = transform.forward * m_LongJumpForwardForce;
        m_VerticalSpeed = m_LongJumpVerticalForce;
        m_CharacterController.Move(jumpDirection * Time.deltaTime);
    }


    void LateUpdate()
    {
        Vector3 l_Angles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0.0f, l_Angles.y, 0.0f);
    }
    void HandleJump()
    {
        m_Animator.SetTrigger("Jump");
        if (m_JumpCount >= 3)
        {
            m_JumpCount = 0;
        }

        float jumpForce = m_JumpVerticalSpeed;

        if (m_JumpCount == 1)
        {
            jumpForce *= m_SecondJumpMultiplier;
            m_Animator.SetInteger("JumpsCombo", 2);
        }
        else if (m_JumpCount == 2)
        {
            jumpForce *= m_ThirdJumpMultiplier;
            m_Animator.SetInteger("JumpsCombo", 3);
        }
        else
        {
            m_Animator.SetInteger("JumpsCombo", 1);
        }

        m_VerticalSpeed = jumpForce;
        m_LastJumpTime = Time.time;
        m_JumpCount++;
        m_CanJump = false; 
    }
    bool IsGrounded()
    {
        
        return Physics.Raycast(transform.position, Vector3.down, 1.4f);
    }
    bool CanJump()
    {
        if (IsGrounded())
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    void Jump()
    {
        m_Animator.SetTrigger("Jump");
        m_Animator.SetInteger("JumpsCombo", 1); //CAMBIAR INT PARA CAMBIAR LA ANIMACION DE SALTO
        //StartCoroutine(ExecuteJump());
    }
    /*
    IEnumerator ExecuteJump()
    {
        yield return new WaitForSeconds(m_WaitStartJumpTime); 
        m_VerticalSpeed = m_JumpVerticalSpeed;
        m_Animator.SetBool("Falling", false);
    }
    */

    void UpdatePunch()
    {
        if(Input.GetMouseButtonDown(m_PunchHitButton)&& CanPunch())
        {
            PunchCombo();
        }
    }

    bool CanPunch()
    {
        return true;
    }

    void PunchCombo()
    {
        m_HasMovement = true;
        m_Animator.SetTrigger("Punch");
        float l_DiffTime = Time.time - m_lastPunchTime;
        if (l_DiffTime <= m_PunchComboAvailableTime)
        {
            m_CurrentPunchId = (m_CurrentPunchId + 1);
            if (m_CurrentPunchId >= 3)
                m_CurrentPunchId = 0;
                //m_CurrentPunchId=(m_CurrentPunchId+1)%3;
        }
        else
            m_CurrentPunchId = 0;
        m_lastPunchTime = Time.time;
        m_Animator.SetInteger("PunchCombo", m_CurrentPunchId);
    }

    public void EnableHitCollider(TpunchType PunchType, bool Active)
    {
        switch (PunchType)
        {
            case TpunchType.LEFT_HAND:
                m_LeftHandPunchHitCollider.SetActive(Active);
                break;
            case TpunchType.RIGHT_HAND:
                m_RightHandPunchHitCollider.SetActive(Active);
                break;
            case TpunchType.KICK:
                m_RightFootKickHitCollider.SetActive(Active);
                break;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Goomba"))
        {
            if (IsUpperHit(hit.transform))
            {
               
                hit.gameObject.GetComponent<GoombaController>().Kill();
                m_VerticalSpeed = m_killJumpVerticalSpeed;
            }
            else
            {
                
                knockbackDirection = (transform.position - hit.transform.position).normalized;
                knockbackDirection.y = 0; 

                
                knockbackTimer = knockbackDuration;

                
                UpdateLife();
                Debug.Log("Mario fue golpeado por Goomba.");
            }
        }
        else if (hit.gameObject.CompareTag("Bridge"))
        {
            hit.rigidbody.AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
        }
        else if (hit.gameObject.CompareTag("Deadzone"))
        {
            m_Animator.SetBool("IsDead", true);
            StartCoroutine(RestartGameWithDelay());
        }
    }


    public void UpdateLife()
    {

        LifeImage.fillAmount -= 0.125f;
        CheckLifeColor();

        if (LifeImage.fillAmount == 0)
        {
            m_Animator.SetBool("IsDead", true);
            StartCoroutine(RestartGameWithDelay());
        }
        ShowAnimation();
    }
    private void CheckLifeColor()
    {
        if (LifeImage.fillAmount <= 0.75f)
        {
            LifeImage.color = Color.blue;
        }
        if (LifeImage.fillAmount <= 0.50f)
        {
            LifeImage.color = Color.yellow;
        }
        if (LifeImage.fillAmount <= 0.25f)
        {
            LifeImage.color = Color.red;
        }
    }
    private IEnumerator RestartGameWithDelay()
    {
        yield return new WaitForSeconds(2f); 
        RestartGame();
    }
    void ShowAnimation()
    {
        m_Animation.Play(m_ShowAnimationClip.name);
        m_Animation.PlayQueued(m_IdleAnimationClip.name);
    }

    bool IsUpperHit(Transform GoombaTransform)
    {
        Vector3 l_GoombaDirection = transform.position - GoombaTransform.position;
        l_GoombaDirection.Normalize();
        float l_DotAngle = Vector3.Dot(l_GoombaDirection, Vector3.up);
        Debug.Log("m_VerticalSpeed" + m_VerticalSpeed);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleNeededToKillGoomba * Mathf.Deg2Rad) && m_VerticalSpeed <= m_MinVerticalSpeedTokillGoomba)
            return true;
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            m_CurrentCheckpoint = other.GetComponent<Checkpoint>();
        }

        if (other.CompareTag("Elevator"))
        {
            Debug.Log("Toco Elevator");
            if (CanAttachElevator(other))
            {
                AttachElevator(other);
            }
        }
        if (other.CompareTag("Star"))
        {
            LifeImage.fillAmount += 0.125f;
            CheckLifeColor();
            other.gameObject.SetActive(false);
            ShowAnimation();
        }

        if (other.CompareTag("Goomba"))
        {
           
            UpdateLife();

            
            CharacterController characterController = GetComponent<CharacterController>();
            Animator animator = GetComponent<Animator>();

            if (characterController != null && animator != null)
            {
               
                pushDirection = transform.position - other.transform.position;
                pushDirection.y = 0; 

                
                pushDirection.Normalize();

               
                animator.SetTrigger("Hit");

                
                pushBackTime = 0.5f;

                Debug.Log("Player pushed back and hit animation played");
            }

            
            if (!m_IsHit)
            {
                PerformHit();
            }
            if (other.CompareTag("Deadzone"))
            {
                RestartGame();
            }
        }
    }


    void PerformHit()
    {
        m_IsHit = true;
        m_Animator.SetTrigger("HitTrigger");
        StartCoroutine(HitRecovery());
    }

    IEnumerator HitRecovery()
    {
        yield return new WaitForSeconds(m_HitRecoveryTime);
        m_IsHit = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator")&& other==m_CurrentElevator)
        {
            DetachElevator();
        }
    }
    bool CanAttachElevator(Collider Elevator)
    {
        if (m_CurrentElevator != null)
        {
            return false;
        }
        return IsAttachableElevator(Elevator);
    }
    bool IsAttachableElevator(Collider Elevator)
    {
        float l_DotAngle = Vector3.Dot(Elevator.transform.forward, Vector3.up);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleToAttachElevator * Mathf.Deg2Rad))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void AttachElevator(Collider Elevator)
    {
        transform.SetParent(Elevator.transform.parent);
        m_CurrentElevator = Elevator;

    }
    
    void DetachElevator()
    {
        m_CurrentElevator = null;
        transform.SetParent(null);
   
    }


    void UpdateElevator()
    {
        if (m_CurrentElevator == null) 
        {
            return;
        }
        if (!IsAttachableElevator(m_CurrentElevator))
        {
            DetachElevator();
        }
    }
    
    public void RestartGame()
    {
        m_CharacterController.enabled = false;

        if (m_CurrentCheckpoint == null)
        {
            transform.position = m_StartPosition;
            transform.rotation = m_StartRotation;
        }
        else
        {
            transform.position = m_CurrentCheckpoint.m_RespawnPosition.position;
            transform.rotation = m_CurrentCheckpoint.m_RespawnPosition.rotation;
        }
        UIDeadCanva.SetActive(true);
        m_Animator.SetBool("IsDead", false);
        LifeImage.fillAmount = 1.0f;
        m_CharacterController.enabled = true;
    }


    public void Die()
    {
        if (m_IsDead) return; 

        m_IsDead = true; 
        m_Animator.SetBool("IsDead", true); 
        m_CharacterController.enabled = false; 

        Invoke(nameof(Respawn), 3.0f);
        UIDeadCanva.SetActive(true);
    }


    private void Respawn()
    {
        Debug.Log("Respawning at: " + (m_RespawnPoint != null ? m_RespawnPoint.position.ToString() : "No Respawn Point!"));

        
        if (m_RespawnPoint != null)
        {
            transform.position = m_RespawnPoint.position;
        }
        else
        {
            Debug.LogWarning("No Respawn Point assigned!");
        }

        m_Animator.SetBool("IsDead", false); 
        m_Animator.Play("Blend Tree", 0, 0f);

        m_IsDead = false;
        m_CharacterController.enabled = true; 
    }



    public void Step(AnimationEvent _Animation)
    {

    }
}
