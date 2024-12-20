using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallKick : MonoBehaviour
{
    private Animator animator;
    private bool canWallKick = false;
    private bool isWallKicking = false;
    private bool isFalling = false;
    private CharacterController characterController;

    public float wallKickForce = 10f;
    public float wallKickDuration = 0.5f;
    public float wallKickHeightMultiplier = 1.5f;  
    public float wallKickBackwardMultiplier = 1.5f;  

    private Vector3 wallKickVelocity = Vector3.zero;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            canWallKick = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            canWallKick = false;
        }
    }

    void Update()
    {
        
        if (characterController.isGrounded)
        {
            isFalling = false;
        }
        else if (wallKickVelocity.y < 0)
        {
            isFalling = true;
        }

        
        if (canWallKick && !isWallKicking && isFalling && Input.GetKeyDown(KeyCode.Space))
        {
            PerformWallKick();
        }

       
        if (isWallKicking)
        {
            characterController.Move(wallKickVelocity * Time.deltaTime);
        }
        else
        {
           
            wallKickVelocity.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    void PerformWallKick()
    {
        isWallKicking = true;

        animator.SetTrigger("WallJumpTrigger");

        Vector3 pushDirection = -transform.forward;  

        wallKickVelocity = pushDirection * wallKickForce;

        wallKickVelocity.y = wallKickHeightMultiplier; 

        wallKickVelocity.x *= wallKickBackwardMultiplier;
        wallKickVelocity.z *= wallKickBackwardMultiplier;

        StartCoroutine(StopWallKick());
    }

    IEnumerator StopWallKick()
    {
        yield return new WaitForSeconds(wallKickDuration);

        wallKickVelocity = Vector3.zero;
        isWallKicking = false;
    }
}
