using UnityEngine;

public class ParallaxFondo : MonoBehaviour
{
    public Transform camara;

    [Header("0 = quieta como el suelo, 1 = pegada a la cámara")]
    [Range(0f, 1f)] public float efectoX = 0.5f;
    [Range(0f, 1f)] public float efectoY = 0.3f;

    private float inicioX, inicioY;
    private float camInicioX, camInicioY;
    private float anchoBloque;

    void Start()
    {
        if (camara == null) camara = Camera.main.transform;

        inicioX = transform.position.x;
        inicioY = transform.position.y;
        camInicioX = camara.position.x;
        camInicioY = camara.position.y;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        anchoBloque = sr.sprite.bounds.size.x * transform.lossyScale.x;
    }

    void LateUpdate()
    {
        float movX = camara.position.x - camInicioX;
        float movY = camara.position.y - camInicioY;

        transform.position = new Vector3(
            inicioX + movX * efectoX,
            inicioY + movY * efectoY,
            transform.position.z
        );

        // Reposicionar un bloque cuando la cámara se aleja, para que nunca se acabe
        float relativo = movX * (1 - efectoX);
        if (relativo > anchoBloque) { inicioX += anchoBloque; camInicioX += anchoBloque / (1 - efectoX); }
        else if (relativo < -anchoBloque) { inicioX -= anchoBloque; camInicioX -= anchoBloque / (1 - efectoX); }
    }
}