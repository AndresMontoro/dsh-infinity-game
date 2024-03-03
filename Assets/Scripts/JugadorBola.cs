using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JugadorBola : MonoBehaviour 
{
    public Camera camara;
    public GameObject suelo;
    public float velocidad = 5;

    public GameObject[] suelosAleatorios = new GameObject[1];

    private Vector3 offset;
    private float Valx = 0.0f, Valz= 0.0f, ValxP = 0.0f;
    private Vector3 DireccionActual;
    private float lastSueloTime;

    // Start is called before the first frame update
    void Start()
    {
        offset = camara.transform.position;
        CrearSueloInicial();
        DireccionActual = Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        camara.transform.position = transform.position + offset;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CambiarDireccion();
        }
    }

    void FixedUpdate()
    {
        transform.Translate(DireccionActual * velocidad * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            Collided collided = collision.gameObject.GetComponent<Collided>();
            if (!collided.wasCollided)
            { 
                StartCoroutine(BorrarSuelo(collision.gameObject));
                collided.wasCollided = true;
            }
        }
    }

    IEnumerator BorrarSuelo(GameObject suelo)
    {
        float aleatorio = Random.Range(0.0f, 1.0f);
        ValxP = Valx;
        if (aleatorio > 0.5)
        {
            Valx += 6.0f;
        }
        else
        {
            Valz += 6.0f;
        }

        // Instantiate(suelo, new Vector3(Valx, 0, Valz), Quaternion.identity);
        int randomIndex = Random.Range(0, suelosAleatorios.Length);
        if (ValxP != Valx)
            Instantiate(suelosAleatorios[randomIndex], new Vector3(Valx, 0, Valz), Quaternion.Euler(0, 90, 0));
        else
            Instantiate(suelosAleatorios[randomIndex], new Vector3(Valx, 0, Valz), Quaternion.identity);

        yield return new WaitForSeconds(2);
        suelo.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        suelo.gameObject.GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(2);
        Destroy(suelo);
    }

    void CrearSueloInicial()
    {
        for (int i = 0; i < 3; i++)
        {
            Valz += 6;
            Instantiate(suelo, new Vector3(Valx, 0, Valz), Quaternion.identity);
        }
    }

    void CambiarDireccion()
    {
        if (DireccionActual == Vector3.forward)
        {
            DireccionActual = Vector3.right;
        }
        else
        {
            DireccionActual = Vector3.forward;
        }
    }
}
