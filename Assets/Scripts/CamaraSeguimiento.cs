using UnityEngine;

public class CamaraSeguimiento : MonoBehaviour
{
    public Transform objetivo; // Aquí irá tu Warrior
    public float suavizado = 5f;
    public Vector3 desfase = new Vector3(2f, 1f, -10f); // Mantiene al personaje un poco a la izquierda

    void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = new Vector3(
            objetivo.position.x + desfase.x,
            objetivo.position.y + desfase.y,
            desfase.z
        );

        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
    }
}