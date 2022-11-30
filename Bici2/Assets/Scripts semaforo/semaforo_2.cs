using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class semaforo_2 : MonoBehaviour
{

    public GameObject luz;

    public Transform semtwo_posverde;
    public Transform semtwo_posamarilla;
    public Transform semtwo_posroja;



    private void Start()
    {
        StartCoroutine(sucesos());
    }

    void Update()
    {

    }
        void roja()
        {
            luz.transform.position = semtwo_posroja.position;
            luz.GetComponent<Light>().color = Color.red;

        }

        void verde()
        {
            luz.transform.position = semtwo_posverde.position;
            luz.GetComponent<Light>().color = new Color32(52, 184, 31, 255);

        }

        void amarilloDesdeVerde()
        {
            luz.transform.position = semtwo_posamarilla.position;
            luz.GetComponent<Light>().color = Color.yellow;

        }

    IEnumerator sucesos()
    {
        while (true)
        {
            roja();
            yield return new WaitForSeconds(20);
            verde();
            yield return new WaitForSeconds(7);
            amarilloDesdeVerde();
            yield return new WaitForSeconds(3);
        }
    }




}
