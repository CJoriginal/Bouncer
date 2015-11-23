using Microsoft.Xna.Framework;

namespace Bouncer.AI
{
    /// <summary>
    /// This class represents a node to be used in a search algorithm
    /// </summary>
    class SearchNode
    {
        public Point position;

        public bool usable;

        public SearchNode[] neighbours;

        /// <summary>
        /// A reference to the node that transfered this node to
        /// the open list. This will be used to trace our path back
        /// from the goal node to the start node.
        /// </summary>
        public SearchNode parent;

        /// <summary>
        /// Provides an easy way to check if this node
        /// is in the open list.
        /// </summary>
        public bool inOpenList;
        /// <summary>
        /// Provides an easy way to check if this node
        /// is in the closed list.
        /// </summary>
        public bool inClosedList;

        /// <summary>
        /// The approximate distance from the start node to the
        /// goal node if the path goes through this node. (F)
        /// </summary>
        public float distanceToGoal;
        /// <summary>
        /// Distance traveled from the spawn point. (G)
        /// </summary>
        public float distanceTraveled;
    }
}
