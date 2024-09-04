using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Start is called before the first frame update
    public Node[] Neighbors;

    [SerializeField]
    private Material startMaterial;
    [SerializeField]

    private Material endMaterial;
    [SerializeField]
    private Material pathMaterial;
    [SerializeField]
    private Material defaultMaterial;

    public void changeStatus(string stauts)
    {
        switch (stauts)
        {
            case "start":
                ChangeMaterial(startMaterial);
                break;
            case "end":
                ChangeMaterial(endMaterial);
                break;
            case "path":
                ChangeMaterial(pathMaterial);
                break;
            default:
                ChangeMaterial(defaultMaterial);
                break;
        }
    }

    private void ChangeMaterial(Material mat)
    {
        GetComponent<MeshRenderer>().material = mat;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = this.getPosition();
        Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.9f);

        float delta = 0.5f;

        for (int i = 0; i < Neighbors.Length; i++)
        {
            Vector3 nextPos = Neighbors[i].getPosition();
            Vector3 direction = nextPos - pos;
            Vector3 startPos = pos;

            float angle = Mathf.Atan2(direction.z, direction.x) + 0.5f * Mathf.PI;
            Vector3 desviation = new Vector3(Mathf.Cos(angle) * delta, 0.0f, Mathf.Sin(angle) * delta);

            Gizmos.DrawRay(startPos + desviation, direction);
        }

    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Obtener la referencia al script Graph
            Graph graph = FindObjectOfType<Graph>();

            // Recalcular la ruta desde este nodo hasta el nodo final
            graph.RecalculateRoute(this);
        }
    }
}
