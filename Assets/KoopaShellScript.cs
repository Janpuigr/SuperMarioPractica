using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoopaShellScript : MonoBehaviour, IRestartGameElement
{
    public AudioSource m_ShellAudioSource;
    public float bounceForce = 100f;
    public float groundThreshold = 0.9f;
    Rigidbody m_RigidBody;
    Vector3 m_StartPosition;
    Quaternion m_StartRotation;
    MarioController m_Mario;

    void Start()
    {
        m_ShellAudioSource = GetComponent<AudioSource>();
        m_Mario = GetComponent<MarioController>();
        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;
        m_RigidBody = GetComponent<Rigidbody>();
    }
    private void Update()
    {

    }
    private void OnCollisionEnter(Collision collision)
    {

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
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goomba"))
        {
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("Coin"))
        {
            m_ShellAudioSource.Play();
        }
        if (other.CompareTag("Koopa"))
        {
            other.gameObject.SetActive(false);
        }
            
    }
    public void RestartGame()
    {
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;


    }

}
