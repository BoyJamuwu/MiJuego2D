using UnityEngine;

public class EnemigoPatrulla : Enemigo
{
    [Header("Patrulla")]
    public float distanciaPatrulla = 3f;
    public bool spriteMiraDerecha = true;

    private float limiteIzq;
    private float limiteDer;
    private int direccion = 1;

    protected override void Awake()
    {
        base.Awake();
        limiteIzq = transform.position.x - distanciaPatrulla;
        limiteDer = transform.position.x + distanciaPatrulla;
    }

    protected override void Mover()
    {
        if (transform.position.x >= limiteDer) direccion = -1;
        else if (transform.position.x <= limiteIzq) direccion = 1;

        rb.velocity = new Vector2(direccion * velocidad, rb.velocity.y);
        sprite.flipX = spriteMiraDerecha ? direccion < 0 : direccion > 0;
    }

    void OnDrawGizmosSelected()
    {
        float x = Application.isPlaying ? (limiteIzq + limiteDer) / 2f : transform.position.x;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(x - distanciaPatrulla, transform.position.y, 0),
                        new Vector3(x + distanciaPatrulla, transform.position.y, 0));
    }
}