using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaShellScript : MonoBehaviour
{
    public float bounceForce = 100f;
    public float groundThreshold = 0.9f;
    Rigidbody m_RigidBody;

    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Goomba"))
        {
            collision.gameObject.SetActive(false);
        }

        Debug.Log("LLEGO AQUI RARO");

        Vector3 normal = collision.contacts[0].normal;
        if (Vector3.Dot(normal, Vector3.up) > groundThreshold)
        {
            return;
        }
        if (m_RigidBody != null)
        {

            Vector3 bounceDirection = Vector3.Reflect(m_RigidBody.velocity, normal).normalized;
            m_RigidBody.velocity = Vector3.zero;
            m_RigidBody.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
        }
    }
}
