using System.Collections.Generic;
using UnityEngine;

namespace Assignment2 {
    public class MainController : MonoBehaviour
    {
        public GameObject WaypointPrefab;

        private const float WORLD_BOUND = 5.0f;
        private const float GRANULARITY = 0.1f;
        private const float OFFSET = 0.1f;
        private float DIAG_COST = Mathf.Sqrt(2) * GRANULARITY;    // Cost of moving diagonally
        private Dictionary<Vector2, Node> nodes;

        private Node[][] nodesArray;
        
        void Start()
        {
            Transform parent = GameObject.Find("Grid").transform;

            int arrLength = Mathf.FloorToInt((WORLD_BOUND * 2) / GRANULARITY);
            nodesArray = new Node[arrLength][];
            for (int i = 0; i < arrLength; i++) {
                nodesArray[i] = new Node[arrLength];
                for (int j = 0; j < arrLength; j++) {
                    float xPos = i * GRANULARITY - WORLD_BOUND;
                    float yPos = j * GRANULARITY - WORLD_BOUND;
                    
                    if (!Physics.CheckSphere(new Vector3(xPos, 0, yPos), OFFSET)) {
                        Instantiate(WaypointPrefab, new Vector3(xPos, 0, yPos), Quaternion.identity, parent);
                        nodesArray[i][j] = new Node(new Vector2(xPos, yPos));
                    } else {
                        nodesArray[i][j] = null;
                    }
                }
            }

            for (int i = 0; i < arrLength; i++) {
                for (int j = 0; j < arrLength; j++) {
                    if (nodesArray[i][j] == null) {
                        continue;
                    }

                    bool safei = false;
                    bool safej = false;

                    if (i + 1 < arrLength) {
                        safei = true;
                        if (nodesArray[i+1][j] != null) {
                            nodesArray[i][j].Edges.Add(nodesArray[i+1][j].Location, GRANULARITY);
                            nodesArray[i+1][j].Edges.Add(nodesArray[i][j].Location, GRANULARITY);
                        }
                    }

                    if (j + 1 < arrLength) {
                        safej = true;
                        if (nodesArray[i][j+1] != null) {
                            nodesArray[i][j].Edges.Add(nodesArray[i][j+1].Location, GRANULARITY);
                            nodesArray[i][j+1].Edges.Add(nodesArray[i][j].Location, GRANULARITY);
                        }
                    }

                    if (safei && safej) {
                        if (nodesArray[i+1][j+1] != null) {
                            if (!Physics.Raycast(To3D(nodesArray[i][j].Location), To3D(nodesArray[i+1][j+1].Location), DIAG_COST)) {
                                nodesArray[i][j].Edges.Add(nodesArray[i+1][j+1].Location, DIAG_COST);
                                nodesArray[i+1][j+1].Edges.Add(nodesArray[i][j].Location, DIAG_COST);
                            }
                        }
                    }

                    if (safei && j > 0 && nodesArray[i+1][j-1] != null) {
                        if (!Physics.Raycast(To3D(nodesArray[i][j].Location), To3D(nodesArray[i+1][j-1].Location), DIAG_COST)) {
                            nodesArray[i][j].Edges.Add(nodesArray[i+1][j-1].Location, DIAG_COST);
                            nodesArray[i+1][j-1].Edges.Add(nodesArray[i][j].Location, DIAG_COST);
                        }
                    }
                }
            }
        }

        private Vector3 To3D(Vector2 twoD) {
            return new Vector3(twoD.x, 0, twoD.y);
        }
    }
}
