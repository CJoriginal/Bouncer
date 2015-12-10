using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace Bouncer.AI
{
    /// <summary>
    /// This class is responsible for providing a search algorithm functionality for path-finding.
    /// </summary>
    class Pathfinder
    {
        int Height;                                         // Height of Tile
        int Width;                                          // Width of Tile

        Node current;                                       // Current Node
        Node start;                                         // Starting Node
        Node target;                                        // End Node
        List<Node> openList;                                // Open List
        List<Node> closedList;                              // Closed List
        int g;                                              // G-Cost

        /// <summary>
        /// Initalise Pathfinder
        /// </summary>
        /// <param name="height">Player Height</param>
        /// <param name="width">Player Width</param>
        public Pathfinder(int height, int width)
        {
            current = null;
            start = null;
            target = null;

            openList = new List<Node>();
            closedList = new List<Node>();

            Height = height;
            Width = width / 2;

            g = 0;
        }

        /// <summary>
        /// Initiate A* Search Algorithm to find the path
        /// </summary>
        /// <param name="startPos">Starting Position</param>
        /// <param name="targetPos">Target Position</param>
        /// <param name="blocks">Current Blocks</param>
        public LinkedList<Vector2> FindPath(Vector2 startPos, Vector2 targetPos, BlockManager blocks)
        {
            current = null;

            start = new Node(startPos);                             // Create Start Node
            target = new Node(targetPos);                           // Create End Node

            openList = new List<Node>();
            closedList = new List<Node>();

            g = 0;

            openList.Add(start);                                    // Add Start Node to Open List

            while (openList.Count > 0)                              // Begin Algorithm whilst Start Node is being found
            {
                int lowest = openList.Min(l => l.F);                // Grab the Node with the lowest F-Cost
                current = openList.First(l => l.F == lowest);

                closedList.Add(current);                            // Add Current Node to Closed List

                openList.Remove(current);                           // Remove Current Node from Open List

                if (closedList.FirstOrDefault(l => l.Position.X == target.Position.X && l.Position.Y == target.Position.Y) != null)     // If Target Node has been added to closed list, the path has been found
                {
                    break;
                }

                List<Node> adjacentNodes = GetWalkableAdjacentNodes(current.Position.X, current.Position.Y, blocks);      // Get Walkable Nodes

                foreach (Node adjacentNode in adjacentNodes)
                {
                    if (closedList.FirstOrDefault(l => l.Position.X == adjacentNode.Position.X                  // If Adj Node is in the Closed List, Ignore It
                            && l.Position.Y == adjacentNode.Position.Y) != null)
                        continue;

                    if (openList.FirstOrDefault(l => l.Position.X == adjacentNode.Position.X                    // If it is not in the Open List, Calculate the Costs and add Parent
                            && l.Position.Y == adjacentNode.Position.Y) == null)
                    {
                        adjacentNode.G = g;                                                                     
                        adjacentNode.H = ComputeHScore(adjacentNode.Position, target.Position);
                        adjacentNode.F = adjacentNode.G + adjacentNode.H;
                        adjacentNode.Parent = current;

                        openList.Insert(0, adjacentNode);                                                       // Add the Adj Node to the Open List
                    }
                    else
                    {
                        if (g + adjacentNode.H < adjacentNode.F)                                                // Test if using the current G score makes the adjacent square's F score
                        {                                                                                       // lower, if yes update the parent because it means it's a better path
                            adjacentNode.G = g;
                            adjacentNode.F = adjacentNode.G + adjacentNode.H;
                            adjacentNode.Parent = current;
                        }
                    }
                }
            }

            LinkedList<Vector2> path = new LinkedList<Vector2>();                                               // Place the path into a List of Vector2s

            foreach(Node n in closedList)
            {
                if (n.Parent != null)
                {
                    path.AddLast(n.Parent.Position);
                }
            }

            openList.Clear();
            closedList.Clear();

            startPos = Vector2.Zero;
            targetPos = Vector2.Zero;

            return path;
        }

        /// <summary>
        /// Compute the H-Cost 
        /// </summary>
        /// <param name="curNode">Current Adj Node</param>
        /// <param name="tarNode">Target Node</param>
        /// <returns>Computed Value</returns>
        private int ComputeHScore(Vector2 curNode, Vector2 tarNode)
        {
            return Math.Abs((int)tarNode.X - (int)curNode.X) + Math.Abs((int)tarNode.Y - (int)curNode.Y);
        }


        private List<Node> IsContained(List<Node> nodes, BlockManager blocks)
        {
            List<Node> confirmedNodes = new List<Node>();
            Node preNode = new Node(Vector2.Zero);

            foreach (Node n in nodes)
            {
                if(confirmedNodes.Count != 0 && preNode == nodes[0] || preNode == nodes[1])
                {
                    break;
                }
                foreach (Block b in blocks)
                {

                    if(b.TriggerZone.X < n.Position.X && b.TriggerZone.Right > n.Position.X)
                    {
                        if (b.TriggerZone.Y < n.Position.Y && b.TriggerZone.Bottom > n.Position.Y)
                        {
                            confirmedNodes.Add(n);
                            preNode = n;
                        }
                    }
                }
            }

            return confirmedNodes;
        }

        /// <summary>
        /// Grab Walkable Adjacent Nodes
        /// </summary>
        /// <param name="x">Current Node Position X</param>
        /// <param name="y">Current Node Position Y</param>
        /// <param name="map">Map of Level</param>
        /// <returns>List of Adjacent Nodes</returns>
        private List<Node> GetWalkableAdjacentNodes(float x, float y, BlockManager blocks)
        {
            List<Node> proposedLocations = new List<Node>()
            {
                new Node(new Vector2(x - 270, y - 150.0f )),       // Top Left
                new Node(new Vector2(x + 270, y - 150.0f )),       // Top Right
                new Node(new Vector2(x - Width, y)),            // Left Node
                new Node(new Vector2(x + Width, y)),            // Right Node
            };

            return IsContained(proposedLocations, blocks);
        }
    }
}