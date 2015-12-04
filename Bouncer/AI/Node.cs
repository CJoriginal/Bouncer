using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace Bouncer.AI
{
    public class Node
    {
        public Vector2 Position;
        public int F;
        public int G;
        public int H;
        public Node Parent;

        public Node(Vector2 position)
        {
            Position = position;
        }
    }
}
