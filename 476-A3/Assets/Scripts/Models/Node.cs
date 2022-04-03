using System.Collections.Generic;
using UnityEngine;

public class Node {
    public Vector2 Location {get; private set; }
    public Dictionary<Vector2, float> Edges;

    public Node(Vector2 loc) {
        Location = loc;
        Edges = new Dictionary<Vector2, float>();
    }
}