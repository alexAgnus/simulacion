using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform nodepath;
    public float rotationSpeed = 5f;
    public float initialRotationX = 290f; // Rotación inicial en el eje X


    void Update()
    {
        Vector3 direction = nodepath.position - transform.position;
        direction.y = 0;

        if(direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Aplicar la rotación inicial en el eje X
            targetRotation *= Quaternion.Euler(initialRotationX, 0, 0);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
