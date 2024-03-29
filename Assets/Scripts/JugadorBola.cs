using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JugadorBola : MonoBehaviour 
{
    public Camera camara;
    public GameObject suelo;
    public GameObject win, lose;
    public float velocidad = 5;
    public int limitePuntuacion = 1;
    public Text Puntos;

    public GameObject[] suelosAleatorios = new GameObject[1];

    private Vector3 offset;
    private float Valx = 0.0f, Valz= 0.0f, ValxP = 0.0f;
    private Vector3 DireccionActual;

    private int indiceEscenaActual;
    private float lastSueloTime;
    private int puntuacion;

    // Start is called before the first frame update
    void Start()
    {
        offset = camara.transform.position;
        CrearSueloInicial();
        DireccionActual = Vector3.forward;
        puntuacion = 0;

        Scene escenaActual = SceneManager.GetActiveScene();
        indiceEscenaActual = escenaActual.buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        camara.transform.position = transform.position + offset;
        if (Input.GetKeyUp(KeyCode.Space))
        {
            CambiarDireccion();
        }
        if (transform.position.y < -10)
        {
            StartCoroutine(Perdiste());
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

    IEnumerator Perdiste ()
    {
        transform.GetComponent<Rigidbody>().isKinematic = true;
        velocidad = 0.0f;
        lose.SetActive(true);
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("SampleScene");
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

        puntuacion++;
        Puntos.text = "Puntuacion: " + puntuacion;
        Debug.Log("Puntuacion: " + puntuacion);
        if (puntuacion == limitePuntuacion)
        {
            indiceEscenaActual++;
            if (indiceEscenaActual > 3)
            {
                if (transform.position.y >= -10)
                {
                    transform.GetComponent<Rigidbody>().isKinematic = true;
                    velocidad = 0.0f;
                    win.SetActive(true);
                    yield return new WaitForSeconds(2);
                    SceneManager.LoadScene("SampleScene");
                }
            }
            else
            {
                SceneManager.LoadScene("Nivel" + indiceEscenaActual.ToString());
            }
        }
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
