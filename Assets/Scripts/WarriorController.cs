using UnityEngine;

public class WarriorController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidadCaminar = 3f;
    public float velocidadCorrer = 6f;
    public float fuerzaSalto = 9.5f;

    [Header("Físicas de Salto Realista")]
    [Tooltip("Multiplica la gravedad cuando el personaje empieza a caer para evitar que flote")]
    public float multiplicadorCaida = 2.5f;
    [Tooltip("Multiplica la gravedad si sueltas el botón de salto antes de llegar arriba")]
    public float multiplicadorSaltoCorto = 2f;

    [Header("Ajustes de Combate")]
    public float cooldownAtaque = 0.5f;
    private float tiempoSiguienteAtaque = 0f;

    [Header("Detección de Suelo")]
    public Transform checkSuelo;
    public Vector2 tamanoCheck = new Vector2(0.4f, 0.1f);
    public LayerMask capaSuelo;

    [Header("Límite de Caída")]
    public float limiteCaidaY = -6f;

    [Header("Efectos de Sonido y Volumen")]
    public AudioClip sonidoSalto;
    [Range(0f, 1f)] public float volumenSalto = 0.5f;

    public AudioClip sonidoAtaque;
    [Range(0f, 1f)] public float volumenAtaque = 0.6f;

    public AudioClip sonidoPasosContinuo;
    [Range(0f, 1f)] public float volumenPasos = 0.4f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    private AudioSource audioEfectos;
    private AudioSource audioPasos;

    private bool enElSuelo;
    private float movimientoX;
    private float velocidadActual;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();

        AudioSource[] fuentesAudio = GetComponents<AudioSource>();
        audioEfectos = fuentesAudio[0];
        audioPasos = fuentesAudio[1];

        audioPasos.clip = sonidoPasosContinuo;
        audioPasos.loop = true;
    }

    void Update()
    {
        if (transform.position.y < limiteCaidaY)
        {
            GameManager.Instance.GameOver();
            return;
        }

        // 1. Leer movimiento horizontal
        movimientoX = Input.GetAxisRaw("Horizontal");

        // 2. Solo permitir correr (Shift) si está tocando el suelo
        bool puedeCorrer = Input.GetKey(KeyCode.LeftShift) && enElSuelo;
        velocidadActual = puedeCorrer ? velocidadCorrer : velocidadCaminar;

        // Girar el personaje
        if (movimientoX > 0)
            sprite.flipX = false;
        else if (movimientoX < 0)
            sprite.flipX = true;

        // 3. Animación y audio continuo de pasos
        bool estaCaminandoEnSuelo = (movimientoX != 0) && enElSuelo;
        anim.SetBool("isRunning", estaCaminandoEnSuelo);
        
        if (Time.time >= tiempoSiguienteAtaque)
        {
            anim.speed = (estaCaminandoEnSuelo && puedeCorrer) ? 1.4f : 1f;
        }

        ManejarAudioPasos(estaCaminandoEnSuelo, puedeCorrer);

        // 4. Salto (Espacio)
        if (Input.GetButtonDown("Jump") && enElSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            enElSuelo = false;
            ReproducirEfecto(sonidoSalto, volumenSalto);
        }

        // 5. Ataque con Cooldown (Clic izquierdo o Ctrl)
        if (Input.GetButtonDown("Fire1") && Time.time >= tiempoSiguienteAtaque)
        {
            tiempoSiguienteAtaque = Time.time + cooldownAtaque;
            anim.speed = 1f;
            anim.SetTrigger("attack");
            ReproducirEfecto(sonidoAtaque, volumenAtaque);
        }
    }

    void FixedUpdate()
    {
        bool tocandoSuelo = Physics2D.OverlapBox(checkSuelo.position, tamanoCheck, 0f, capaSuelo);
        enElSuelo = tocandoSuelo && rb.velocity.y <= 0.01f;

        // Movimiento horizontal
        rb.velocity = new Vector2(movimientoX * velocidadActual, rb.velocity.y);

        // Aplicar gravedad mejorada para evitar el efecto "flotando"
        if (rb.velocity.y < 0)
        {
            // Está cayendo: aceleramos la caída hacia el suelo
            rb.velocity += Vector2.up * Physics2D.gravity.y * (multiplicadorCaida - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            // Está subiendo pero el jugador ya soltó la barra espaciadora: frena la subida
            rb.velocity += Vector2.up * Physics2D.gravity.y * (multiplicadorSaltoCorto - 1) * Time.fixedDeltaTime;
        }
    }

    private void ManejarAudioPasos(bool caminando, bool corriendo)
    {
        if (audioPasos == null || sonidoPasosContinuo == null) return;

        audioPasos.volume = volumenPasos;

        if (caminando)
        {
            audioPasos.pitch = corriendo ? 1.35f : 1.0f;

            if (!audioPasos.isPlaying)
            {
                audioPasos.Play();
            }
        }
        else
        {
            if (audioPasos.isPlaying)
            {
                audioPasos.Stop();
            }
        }
    }

    private void ReproducirEfecto(AudioClip clip, float volumen)
    {
        if (clip != null && audioEfectos != null)
        {
            audioEfectos.pitch = Random.Range(0.95f, 1.05f);
            audioEfectos.PlayOneShot(clip, volumen);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (checkSuelo != null)
            Gizmos.DrawWireCube(checkSuelo.position, tamanoCheck);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-500f, limiteCaidaY, 0f), new Vector3(500f, limiteCaidaY, 0f));
    }

}