using UnityEngine;

public class SmoothRotateToTarget : MonoBehaviour
{
    private bool m_PlayerTouching;
    // Ángulo objetivo en el eje X
    public float targetAngleX = 90f;

    // Velocidad de interpolación
    public float rotationSpeed = 2f;

    private Quaternion targetRotation;

    Vector3 startingPositionX;

    void Start()
    {
        startingPositionX = transform.position;
        // Configurar la rotación objetivo
        targetRotation = Quaternion.Euler(targetAngleX, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    void Update()
    {
        transform.position = startingPositionX;
        Debug.Log("a    "+ m_PlayerTouching);
        if (m_PlayerTouching == false)
        {   
            // Rotar suavemente hacia el objetivo
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

             // Detener la rotación si estamos lo suficientemente cerca
        
        
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                Debug.Log("Rotación completada suavemente.");
                transform.rotation = targetRotation; // Asegurar precisión al finalizar

            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("SI");
            m_PlayerTouching = true;
        }
        else
        {
            Debug.Log("NO");
            m_PlayerTouching = false;
        }
    }

}
