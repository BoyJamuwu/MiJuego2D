using UnityEngine;

public class EnemigoVertical : Enemigo
{
    [Header("Vuelo vertical")]
    public float distanciaVertical = 2f;

    private float limiteInf;
    private float limiteSup;
    private int direccion = 1;

    protected override void Awake()
    {
        base.Awake();
        limiteInf = transform.position.y - distanciaVertical;
        limiteSup = transform.position.y + distanciaVertical;
    }

    protected override void Mover()
    {
        if (transform.position.y >= limiteSup) direccion = -1;
        else if (transform.position.y <= limiteInf) direccion = 1;

        rb.velocity = new Vector2(0f, direccion * velocidad);
    }

    void OnDrawGizmosSelected()
    {
        float y = Application.isPlaying ? (limiteInf + limiteSup) / 2f : transform.position.y;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(transform.position.x, y - distanciaVertical, 0),
                        new Vector3(transform.position.x, y + distanciaVertical, 0));
    }
}