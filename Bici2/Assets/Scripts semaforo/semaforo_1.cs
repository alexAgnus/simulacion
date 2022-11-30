using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class semaforo_1 : MonoBehaviour
{
    public GameObject luz;

    public Transform posVerde;
    public Transform posAmarilla;
    public Transform posRoja;

    private bool verde;
    private bool amarilloDesdeVerde;
    private bool roja;


    private void Start()
    {
        verde = true;
    }

    void Update()
    {
        if (verde == true)
        {
            luz.transform.position = posVerde.position;
            luz.GetComponent<Light>().color = new Color32(52, 184, 31, 255);
            StartCoroutine(luzVerde());
            roja = false;
        }

        if (amarilloDesdeVerde == true)
        {
            luz.transform.position = posAmarilla.position;
            luz.GetComponent<Light>().color = Color.yellow;
            StartCoroutine(luzAmarillaV());
            verde = false;
        }

        if (roja == true)
        {
            luz.transform.position = posRoja.position;
            luz.GetComponent<Light>().color = Color.red;
            StartCoroutine(luzRoja());
            amarilloDesdeVerde = false;
        }
    }

    IEnumerator luzVerde()
    {
        yield return new WaitForSeconds(15);
        amarilloDesdeVerde = true;
    }

    IEnumerator luzAmarillaV()
    {
        yield return new WaitForSeconds(5);
        roja = true;
    }

    IEnumerator luzRoja()
    {
        yield return new WaitForSeconds(10);
        verde = true;
    }
}