using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    
    [SerializeField]
    public Node start;
    [SerializeField]
    private Node end;

    public List<Node> path;
    
    void Start()
    {
        Arrow arrow = FindObjectOfType<Arrow>();
        path = GetRoute(new List<Node>(), start);

        Node newPlayerObject = path[0];
        if (newPlayerObject != null && arrow != null)
        {
            arrow.nodepath = newPlayerObject.transform;
        }

        for(int i = 0; i < path.Count; i++){
            path[i].changeStatus("path");
        }

        start.changeStatus("start");
        end.changeStatus("end");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Node> GetRoute(List<Node> visited, Node point)
    {
        List<Node> route = new List<Node>();
        List<Node> newVisited = new List<Node>(visited);
        route.Add(point);
        if (point == end)
        {
            return route;
        }
        newVisited.Add(point);

        bool stuck = true;
        float minDis = -1.0f;

        float distance;

        List<Node> nextPath = new List<Node>();
        for (int i = 0; i < point.Neighbors.Length; i++)
        {
            Node next = point.Neighbors[i];

            if (!visited.Contains(next))
            {
                stuck = false;
                List<Node> p = GetRoute(newVisited, next);

                if (p.Count <= 0) continue;

                distance = pathDistance(p);
                if (minDis == -1 || distance < minDis)
                {
                    nextPath = p;
                    minDis = distance;
                }
            }
        }

        if (stuck)
            return new List<Node>();

        if (nextPath.Count <= 0)
        {
            return new List<Node>();
        }

        for (int i = 0; i < nextPath.Count; i++)
        {
            route.Add(nextPath[i]);
        }
        return route;
    }

    float pathDistance(List<Node> route){
        float sum = 0.0f;
        for(int i = 1; i < route.Count; i++){
            Vector3 distance = route[i].getPosition() - route[i -1].getPosition();
            sum += distance.magnitude;
        }

        return sum;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 dir = path[i + 1].getPosition() - path[i].getPosition();
            Gizmos.DrawRay(path[i].getPosition(), dir);
        }
    }

    public void RecalculateRoute(Node newStart)
    {
        start = newStart;
        path = GetRoute(new List<Node>(), start);
        Arrow arrow = FindObjectOfType<Arrow>();
        for (int i = 0; i < path.Count; i++)
        {
            path[i].changeStatus("path");
        }
        if (start.name == end.name)
        {
            Debug.Log("Llegaste a la meta");
        }else{
            Node newPlayerObject = path[1];
            if (newPlayerObject != null && arrow != null)
            {
                arrow.nodepath = newPlayerObject.transform;
            }
        }
        start.changeStatus("start");
        end.changeStatus("end");
    }
}
