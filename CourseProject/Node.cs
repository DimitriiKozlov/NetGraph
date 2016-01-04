using System;
using System.Collections.Generic;
using System.Windows;

namespace CourseProject
{
    class Node
    {
        public readonly int Id;
        public Point Coordinate;
        public const int DefaultRadius = 60;
        public int ChildId;
        public bool Enable;
        public int Label;
        public bool Visited;

        private const int BufferSize = 10;
        public readonly List<Message> Buffer; 

        public Node(int id, Point p, int chId)
        {
            Id = id;
            Coordinate = p;
            ChildId = chId;
            Enable = true;
            Buffer = new List<Message>();
            Label = int.MaxValue;
            Visited = false;
        }
    }
}
