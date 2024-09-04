using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class Graph : MonoBehaviour
{

    [SerializeField]
    public Node start;
    private Node end;

    private List<Node> path;
    private int currentPoints = 0;

    [SerializeField]
    public int maxPoints = 2;

    private Node[] allNodes;
    public GameObject arrowGoal;
    // public TMP_Text goalText;
    public TMP_Text currentPointText;
    public TMP_Text maxPointsText;
    public string levelName;
    void Start()
    {
        allNodes = GameObject.FindGameObjectsWithTag("NodeDestino").Select(go => go.GetComponent<Node>()).ToArray();
        SelectNewEndNode();
        PlayGoalAnimation();
        // goalText.text="Sigue la flecha";
        currentPointText.text = this.currentPoints.ToString();
        maxPointsText.text = $"/{maxPoints}";
        Arrow arrow = FindObjectOfType<Arrow>();
        path = AStarPathFinding(start, end);
        Node newPlayerObject = path[0];
        if (newPlayerObject != null && arrow != null)
        {
            arrow.nodepath = newPlayerObject.transform;
        }

        foreach (Node node in path)
        {
            node.changeStatus("path");
        }

        start.changeStatus("start");
        end.changeStatus("end");
    }


    void Update()
    {

    }

    List<Node> AStarPathFinding(Node start, Node goal)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();
        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = HeuristicCostEstimate(start, goal);
        while (openSet.Count > 0)
        {
            Node current = openSet.OrderBy(node => fScore.ContainsKey(node) ? fScore[node] : float.MaxValue).First();
            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }
            openSet.Remove(current);
            closedSet.Add(current);
            foreach (Node neighbor in current.Neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }
                float tentative_gScore = gScore[current] + Vector3.Distance(current.getPosition(), neighbor.getPosition());
                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentative_gScore >= gScore[neighbor])
                {
                    continue;
                }
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, goal);
            }
        }
        return new List<Node>();
    }

    float HeuristicCostEstimate(Node node, Node goal)
    {
        return Vector3.Distance(node.getPosition(), goal.getPosition());
    }

    List<Node> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Node> totalPath = new List<Node> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
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
        path = AStarPathFinding(start, end);
        Arrow arrow = FindObjectOfType<Arrow>();
        foreach (Node node in path)
        {
            node.changeStatus("path");
        }

        if (start.name == end.name)
        {
            currentPoints += 1;
            currentPointText.text = currentPoints.ToString();
            if (currentPoints == maxPoints)
            {
                // goalText.text="Felicidades!! ya terminaste \ntodo el recorrido :)";
                arrowGoal.SetActive(false);
                SceneManager.LoadScene(levelName);
            }
            else
            {
                // goalText.text=$"Llegaste al destino: {currentPoints}\nSigue la flecha";
                SelectNewEndNode();
                PlayGoalAnimation();
                Node newPlayerObject = path[1];
                if (newPlayerObject != null && arrow != null)
                {
                    arrow.nodepath = newPlayerObject.transform;
                }
            }
        }
        else
        {
            Node newPlayerObject = path[1];
            if (newPlayerObject != null && arrow != null)
            {
                arrow.nodepath = newPlayerObject.transform;
            }
            // goalText.text="Sigue la flecha";
            currentPointText.text = this.currentPoints.ToString();
        }
        start.changeStatus("start");
        end.changeStatus("end");
    }

    private void SelectNewEndNode()
    {
        if (allNodes.Length > 0)
        {
            Node newEnd;
            do
            {
                newEnd = allNodes[Random.Range(0, allNodes.Length)];
            } while (newEnd == end); // Ensure the new end node is different from the current end node
            end = newEnd;
            path = AStarPathFinding(start, end);
            foreach (Node node in path)
            {
                node.changeStatus("path");
            }
            end.changeStatus("end");
            Debug.Log("Nueva meta establecida: " + end.name);
        }
    }

    private void PlayGoalAnimation()
    {
        Animator arrow_ani = arrowGoal.GetComponent<Animator>();
        arrow_ani.Play("Goal");
        Vector3 sourcePosition = end.transform.position;
        sourcePosition.y += 1.0f;
        arrowGoal.transform.position = sourcePosition;
    }
}

