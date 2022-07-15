using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding
{
    class Node
    {
        public Vector2Int position;
        public Node bestPreviousNode;
        public float value;

        public Node(Vector2Int position, Node bestPreviousNode, float value)
        {
            this.position = position;
            this.bestPreviousNode = bestPreviousNode;
            this.value = value;
        }
    }

    List<Node> unknownNodes;
    List<Node> openList;
    List<Node> closedList;

    List<Vector2Int> GetPathTo(Vector2Int startPosition, Vector2Int target, Grid grid)
    {
        unknownNodes = new List<Node>();
        openList = new List<Node>();
        closedList = new List<Node>();

        openList.Add(new Node(startPosition, null, (target - startPosition).magnitude));

        List<Vector2Int> result = new List<Vector2Int>();

        

        return result;
    }

    private void CheckNode(Node node, Grid grid)
    {
        CheckNodeForUnknown(node.position + Vector2Int.up, node);
        CheckNodeForUnknown(node.position + Vector2Int.right, node);
        CheckNodeForUnknown(node.position + Vector2Int.down, node);
        CheckNodeForUnknown(node.position + Vector2Int.left, node);
    }

    void CheckNodeForUnknown(Vector2Int position, Node currentNode)
    {
        Node nodeToCheck = unknownNodes.First(n => n.position == position);
        if (nodeToCheck != null)
        {
            nodeToCheck.bestPreviousNode = currentNode;
            int currentPath = 1;
            Node parentCheckNode = currentNode;
            while(parentCheckNode.bestPreviousNode != null)
            {
                currentPath++;
                parentCheckNode = parentCheckNode.bestPreviousNode;
            }


            unknownNodes.Remove(nodeToCheck);
            openList.Add(nodeToCheck);
        }
    }
}
