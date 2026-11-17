using UnityEngine;

public class GeneradorNubes : MonoBehaviour
{
    [Header("Imágenes")]
    public Sprite[] sprites;

    [Header("Cantidad")]
    public int cantidad = 8;

    [Header("Tamaño (pequeña = lejana, grande = cercana)")]
    public float escalaMin = 0.5f;
    public float escalaMax = 1.3f;

    [Header("Altura (relativa al centro de la cámara, acepta negativos)")]
    public float alturaMin = -1f;
    public float alturaMax = 4f;

    [Header("Movimiento")]
    public float velocidadMin = 0.15f;
    public float velocidadMax = 0.5f;
    [Range(0f, 0.99f)] public float parallaxLejana = 0.95f;
    [Range(0f, 0.99f)] public float parallaxCercana = 0.75f;

    [Header("Vaivén vertical")]
    public float vaivenAltura = 0.15f;
    public float vaivenVelocidad = 0.5f;

    [Header("Margen fuera de pantalla")]
    public float margen = 3f;

    [Header("Capas")]
    public string sortingLayer = "Fondo";
    public int orderInLayer = -3;

    class Nube
    {
        public Transform t;
        public SpriteRenderer sr;
        public float velocidad;
        public float parallax;
        public float offsetY;
        public float fase;
    }

    Nube[] nubes;
    Transform cam;
    Camera camara;
    float ultimaCamX;

    void Start()
    {
        camara = Camera.main;
        cam = camara.transform;
        ultimaCamX = cam.position.x;

        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("GeneradorNubes: asigna los sprites en el Inspector.");
            enabled = false;
            return;
        }

        // Alturas repartidas en franjas y mezcladas, para que no se alineen
        float[] alturas = new float[cantidad];
        for (int i = 0; i < cantidad; i++)
        {
            float franja = (i + Random.value) / cantidad;
            alturas[i] = Mathf.Lerp(alturaMin, alturaMax, franja);
        }
        for (int i = cantidad - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            float tmp = alturas[i]; alturas[i] = alturas[j]; alturas[j] = tmp;
        }

        nubes = new Nube[cantidad];
        float mitad = MitadAncho();

        for (int i = 0; i < cantidad; i++)
        {
            GameObject go = new GameObject("Nube_" + i);
            go.transform.SetParent(transform);

            Nube n = new Nube();
            n.t = go.transform;
            n.sr = go.AddComponent<SpriteRenderer>();
            n.sr.sortingLayerName = sortingLayer;

            Configurar(n, alturas[i]);

            // Posición X repartida a lo ancho con un poco de azar
            float paso = (mitad * 2f) / cantidad;
            float x = cam.position.x - mitad + paso * i + Random.Range(0f, paso);
            n.t.position = new Vector3(x, cam.position.y + n.offsetY, 0f);

            nubes[i] = n;
        }
    }

    // Da a una nube un aspecto nuevo: imagen, tamaño, velocidad y altura
    void Configurar(Nube n, float altura)
    {
        n.sr.sprite = sprites[Random.Range(0, sprites.Length)];
        n.sr.flipX = Random.value > 0.5f;

        float t = Random.value; // 0 = lejana, 1 = cercana
        float escala = Mathf.Lerp(escalaMin, escalaMax, t);
        n.t.localScale = new Vector3(escala, escala, 1f);
        n.sr.sortingOrder = orderInLayer; 

        n.parallax = Mathf.Lerp(parallaxLejana, parallaxCercana, t);
        n.velocidad = Mathf.Lerp(velocidadMin, velocidadMax, t);
        n.offsetY = altura;
        n.fase = Random.value * 6.28f;
    }

    float MitadAncho()
    {
        return camara.orthographicSize * camara.aspect + margen;
    }

    void LateUpdate()
    {
        float mitad = MitadAncho();
        float deltaCam = cam.position.x - ultimaCamX;
        ultimaCamX = cam.position.x;

        foreach (Nube n in nubes)
        {
            Vector3 p = n.t.position;

            p.x += deltaCam * n.parallax - n.velocidad * Time.deltaTime;

            float vaivén = Mathf.Sin(Time.time * vaivenVelocidad + n.fase) * vaivenAltura;
            p.y = cam.position.y + n.offsetY + vaivén;

            bool saleIzq = p.x < cam.position.x - mitad;
            bool saleDer = p.x > cam.position.x + mitad;

            if (saleIzq || saleDer)
            {
                // Reaparece al otro lado con otra altura, tamaño e imagen
                Configurar(n, Random.Range(alturaMin, alturaMax));
                p.x += saleIzq ? mitad * 2f : -mitad * 2f;
                p.y = cam.position.y + n.offsetY;
            }

            n.t.position = p;
        }
    }
}