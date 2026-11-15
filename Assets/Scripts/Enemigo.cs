using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemigo : MonoBehaviour
{
    [Header("Enemigo")]
    public float velocidad = 2f;

    protected Rigidbody2D rb;
    protected SpriteRenderer sprite;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void FixedUpdate()
    {
        Mover();
    }

    // Cada tipo de enemigo define su propia rutina
    protected abstract void Mover();

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<WarriorController>() != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}