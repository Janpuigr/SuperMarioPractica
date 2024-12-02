using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WallKick : MonoBehaviour
{
    private Animator animator;
    private bool canWallKick = false;
    private CharacterController characterController;

    public float wallKickForce = 10f; 
    public float wallKickDuration = 0.5f; 

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
        if (canWallKick && Input.GetKeyDown(KeyCode.Space)) 
        {
            PerformWallKick();
        }

        if (wallKickVelocity != Vector3.zero)
        {
            characterController.Move(wallKickVelocity * Time.deltaTime);
        }
    }

    void PerformWallKick()
    {
        animator.SetTrigger("WallJumpTrigger");

        if (characterController != null)
        {
            Vector3 pushDirection = (transform.forward + transform.up).normalized;

            wallKickVelocity = pushDirection * wallKickForce;

            StartCoroutine(StopWallKick());
        }
    }

    IEnumerator StopWallKick()
    {
        yield return new WaitForSeconds(wallKickDuration);

        wallKickVelocity = Vector3.zero; 

    }
}
