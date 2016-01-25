using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CourseProject
{
    class Graph
    {
        private int _avalibleId;

        public readonly List<Node> Nodes;
        public readonly List<Edge> Edges;

        public Graph()
        {
            _avalibleId = 0;
            Nodes = new List<Node>();
            Edges = new List<Edge>();
        }

        public void AddNode(Point p, int chId)
        {
            Nodes.Add(new Node(_avalibleId++, p, chId));
        }

        public int GetId(Point p)
        {
            foreach (var node in Nodes.Where(node => ((int)(p.X - node.Coordinate.X) << 1) + ((int)(p.Y - node.Coordinate.Y) << 1) < (Node.DefaultRadius << 1) / 4))
                return node.Id;
            return -1;
        }

        public int GetNum(Point p)
        {
            for (var i = 0; i < Nodes.Count; i++)
                if (Math.Pow(p.X - Nodes[i].Coordinate.X, 2) + Math.Pow(p.Y - Nodes[i].Coordinate.Y, 2) <=
                    Math.Pow(Node.DefaultRadius * 0.5, 2))
                    return i;
            return -1;
        }

        public bool RemoveNode(Point p)
        {
            var id = GetId(p);
            if (id == -1)
                return false;

            for (var i = 0; i < Nodes.Count; i++)
                if (Nodes[i].Id == id)
                {
                    Nodes.RemoveAt(i);
                    break;
                }

            for (var i = 0; i < Edges.Count; i++)
                if ((Edges[i].IdVertex1 == id) || (Edges[i].IdVertex2 == id))
                    Edges.RemoveAt(i--);
            return true;
        }

        public bool AddEdge(Edge edge)
        {
            foreach (var e in Edges.Where(e => ((e.IdVertex1 == edge.IdVertex1) && (e.IdVertex2 == edge.IdVertex2)) || ((e.IdVertex1 == edge.IdVertex2) && (e.IdVertex2 == edge.IdVertex1))))
            {
                e.Content = edge.Content;
                return false;
            }
            if (!Nodes[GetNum(edge.IdVertex1)].Enable || !Nodes[GetNum(edge.IdVertex2)].Enable) return false;
            Edges.Add(edge);
            return true;
        }

        public List<Edge> GetNodeEdges(int id)
        {
            return Edges.Where(e => (e.IdVertex1 == id) || (e.IdVertex2 == id)).ToList();
        }

        public void DeleteEdge(Edge e)
        {
            var index = e.ChildId;

            Edges.Remove(e);
            foreach (var ed in Edges.Where(ed => ed.ChildId > index))
                ed.ChildId--;
            foreach (var node in Nodes.Where(node => node.ChildId > index))
                node.ChildId--;
        }

        public void DeleteNode(int i)
        {
            var index = Nodes[i].ChildId;

            Nodes.RemoveAt(i);
            foreach (var ed in Edges.Where(ed => ed.ChildId > index))
                ed.ChildId--;
            foreach (var node in Nodes.Where(node => node.ChildId > index))
                node.ChildId--;
        }

        public int GetNum(int id)
        {
            for (var i = 0; i < Nodes.Count; i++)
                if (Nodes[i].Id == id)
                    return i;
            return -1;
        }

        public List<int> GetConnectedNodeId(int id)
        {
            return GetNodeEdges(id).Select(e => e.IdVertex1 == id ? e.IdVertex2 : e.IdVertex1).ToList();
        }

        public List<int> GetWayToNode(int id, int findId, int size)
        {
            if (id == findId)
                return null;

            var way = new List<int>();

            foreach (var node in Nodes)
            {
                node.Label = int.MaxValue;
                node.Visited = false;
            }
            Nodes[GetNum(id)].Label = 0;
            Nodes[GetNum(id)].Visited = true;

            if ((Dextra(id, way, findId, size) == int.MaxValue) || (way.Count <= 1))
                return null;
            return way;
        }

        private int Dextra(int id, List<int> way, int findId, int size)
        {
            var lst = GetNodeEdges(id);
            var currentWeight = Nodes[GetNum(id)].Label;
            var childId = new List<int>();
            var fastWay = int.MaxValue;
            var shortTempWay = new List<int>();

            way.Add(id);
            //if (id == findId)
            //    return Nodes[GetNum(findId)].Label;

            foreach (var edge in lst)
            {
                var i = GetNum(edge.IdVertex1 == id ? edge.IdVertex2 : edge.IdVertex1);

                //if (Nodes[i].Visited)
                //    continue;

                if (Nodes[i].Label - currentWeight > edge.ChannelWeight)
                {
                    Nodes[i].Label = currentWeight + edge.ChannelWeight;
                    Nodes[i].Visited = false;
                }
                else Nodes[i].Label = Nodes[i].Label;
                //countOfNotVisitedChild++;
                if ((Nodes[i].Id == findId) && (edge.CurentCapacity >= size))
                {
                    fastWay = Nodes[i].Label;
                    shortTempWay.Add(findId);
                }
            }

            //if (countOfNotVisitedChild == 0)
            //    return int.MaxValue;

            while (true)
            {
                var iMin = -1;
                var iDel = -1;
                for (var i = 0; i < lst.Count; i++)
                {
                    var ind = GetNum(lst[i].IdVertex1 == id ? lst[i].IdVertex2 : lst[i].IdVertex1);
                    if (lst[i].CurentCapacity < size) continue;
                    if ((Nodes[ind].Visited) || (ind == findId)) continue;
                    if ((iMin != -1) && (Nodes[ind].Label >= Nodes[i].Label)) continue;
                    iMin = ind;
                    iDel = i;
                }
                if (iMin == -1) break;
                childId.Add(Nodes[iMin].Id);
                Nodes[iMin].Visited = true;
                lst.RemoveAt(iDel);
            }


            foreach (var i in childId)
            {
                var tempWay = new List<int>();
                Nodes[GetNum(i)].Visited = true;
                var childWay = Dextra(i, tempWay, findId, size);

                if (childWay >= fastWay) continue;

                fastWay = childWay;
                shortTempWay = tempWay;
            }

            if (fastWay == int.MaxValue)
                return int.MaxValue;

            way.AddRange(shortTempWay);
            return fastWay;
        }

        public void RestoreEdgeCapacity()
        {
            foreach (var edge in Edges)
                edge.CurentCapacity = edge.ChannelCapacity;
        }
    }
}
