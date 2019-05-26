using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Bouncer.AI
{
    /// <summary>
    /// Represents a Search Node to be used in A*
    /// </summary>
    public class Node
    {
        public Vector2 Position;                // Position of Node
        public int F;                           // F-Cost
        public int G;                           // G-Cost
        public int H;                           // H-Cost
        public Node Parent;                     // Parent of Node

        public Node(Vector2 position)
        {
            Position = position;
        }
    }
}
