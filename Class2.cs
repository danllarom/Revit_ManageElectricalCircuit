using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using System.Diagnostics;
using System.Windows.Forms;

namespace Revit_ManageElectricalCircuit
{
    class FloydWarshall
    {
        public double[,] AdjacencyMatrix=null;
        public double[,] AdjacencyMatrix0 = null;
        public int[,] PathMatrix = null;
        public List<nodo> nodos = new List<nodo> { };
        public List<XYZ[]> Aristas = null;
        public List<edges> Edges = new List<edges> { };
        List<int> path = new List<int>();

        public FloydWarshall()
        {
        }
        public List<nodo> GetNodos()
        {
            int i = 0;
            foreach (XYZ[] arista in Aristas)
            {
                var testWasTrue = false;

                if (nodos.Count() == 0)
                {
                    nodo punto = new nodo(i, arista[0], null);
                    nodos.Add(punto);
                    i++;
                }
                foreach (nodo nodo1 in nodos)
                {
                    if (Math.Abs(arista[0].DistanceTo(nodo1.Location))<=1e-9)
                    {
                        testWasTrue = true;
                        break;
                    }
                }

                if (testWasTrue==false)
                {
                    nodo punto = new nodo(i, arista[0],null);
                    nodos.Add(punto);
                    i++;
                }
                testWasTrue = false;
                foreach (nodo nodo1 in nodos)
                {
                    if (Math.Abs(arista[1].DistanceTo(nodo1.Location)) <=1e-9)
                    {
                        testWasTrue = true;
                        break;
                    }
                }
                if (testWasTrue==false)
                {
                    nodo punto = new nodo(i, arista[1],null);
                    nodos.Add(punto);
                    i++;
                }
            }
            return nodos;
        }
        public List<edges> GetEdges()
        {
            Edges = new List<edges> { };
            foreach (XYZ[] arista in Aristas)
            {
                edges line1 = new edges{Lenth = Math.Abs(arista[0].Subtract(arista[1]).GetLength())};

                foreach (nodo nodo1 in nodos)
                {
                    if (Math.Abs(arista[0].Subtract(nodo1.Location).GetLength()) <= 1e-9) 
                    {
                        line1.NodeA = nodo1.Name;
                    }
                    if (Math.Abs(arista[1].Subtract(nodo1.Location).GetLength()) <= 1e-9)
                    {
                        line1.NodeB = nodo1.Name;
                    }
                }
                Edges.Add(line1);
            }
            return Edges;
        }
        public double[,] GetAdjacencyMatrix()
        {
            int Nnodes = nodos.Count();

            AdjacencyMatrix = new double[Nnodes, Nnodes];
            AdjacencyMatrix0 = new double[Nnodes, Nnodes];

            for (int i = 0; i < Nnodes; ++i)
            {
                for (int j = 0; j < Nnodes; ++j)
                {
                    AdjacencyMatrix[i, j] = PositiveInfinity;
                    AdjacencyMatrix0[i, j] = 0;
                    if (i == j)
                    {
                        AdjacencyMatrix[i, j] = 0;
                    }
                }
            }
            foreach (edges arista1 in Edges)
            {
                AdjacencyMatrix[arista1.NodeA, arista1.NodeB] = arista1.Lenth;
                AdjacencyMatrix[arista1.NodeB, arista1.NodeA] = arista1.Lenth;
                AdjacencyMatrix0[arista1.NodeA, arista1.NodeB] = 1;
                AdjacencyMatrix0[arista1.NodeB, arista1.NodeA] = 1;
            }
            return AdjacencyMatrix;
        }
        public const double PositiveInfinity = 1.5e300;
        public int[,] PlayFloydWarshall(List<XYZ[]> aristas)
        {
            Aristas = new List<XYZ[]> (aristas);

            nodos = new List<nodo>(GetNodos());

            Edges = new List<edges>(GetEdges());

            AdjacencyMatrix = (double[,])GetAdjacencyMatrix().Clone();

            //PathMatrix
            //     1-2-3-4=j
            //i=1-(1,2,3,4)
            //i=2-(1,2,3,4)
            //i=3-(1,2,3,4)
            //i=4-(1,2,3,4)
            int Nnodes = nodos.Count();
            PathMatrix = new int[Nnodes, Nnodes];
            
            for (int i = 0; i < Nnodes; ++i)
            {
                for (int j = 0; j < Nnodes; ++j)
                {
                    if (AdjacencyMatrix[i, j] != PositiveInfinity)
                    {
                        PathMatrix[i, j] = i;
                    }
                    else
                    {
                        PathMatrix[i, j] = -1;
                    }
                    if (i == j)
                    {
                        PathMatrix[i, j] = i;
                    }
                }
            }
            
            //FloydWarshall
            for (int k = 0; k < Nnodes; ++k)
            {
                for (int i = 0; i < Nnodes; ++i)
                {
                    for (int j = 0; j < Nnodes; ++j)
                    {
                        if ((i != k) && (j != k) && (AdjacencyMatrix[i, k] != PositiveInfinity) && (AdjacencyMatrix[k, j] != PositiveInfinity))
                        {
                            if (((AdjacencyMatrix[i, k] + AdjacencyMatrix[k, j]) < AdjacencyMatrix[i, j]))
                            {
                                AdjacencyMatrix[i, j] = AdjacencyMatrix[i, k] + AdjacencyMatrix[k, j];
                                PathMatrix[i, j] = k;
                            }
                        }
                    }
                }
            }
            return PathMatrix;
        }
        public List<int> GetPath(int nodeInit, int nodeEnd)
        {
            //http://web.mit.edu/urban_or_book/www/book/chapter6/6.2.2.html
            //https://medium.com/@vighneshtiwari16377/floyd-warshall-dynamic-programming-algorithm-e2a899c3e5e6

            path.Add(nodeInit);
            path.Add(nodeEnd);

            readPathMatrix(nodeInit, nodeEnd);
            
            return path;
        }
        public XYZ GetXYZNode(int node)
        {
            XYZ point = null;
            foreach (nodo nodei in nodos)
            {
                if (node == nodei.Name)
                {
                    point = nodei.Location;
                    break;
                }
            }
            return point;
        }
        public void readPathMatrix(int nodeInit, int nodeEnd)
        {
            if (((PathMatrix[nodeInit, nodeEnd] != nodeInit) && (PathMatrix[nodeInit, nodeEnd] != nodeEnd))|| ((PathMatrix[nodeEnd, nodeInit] != nodeInit) && (PathMatrix[nodeEnd, nodeInit] != nodeEnd)))
            {
                int node = nodeEnd;
                node = PathMatrix[nodeInit, node];
                path.Add(node);

                if ((PathMatrix[nodeInit, node] != nodeInit) && (PathMatrix[nodeInit, node] != node) && (node != -1))
                {
                    readPathMatrix(nodeInit, node);
                }
                else if ((PathMatrix[node, nodeInit] != nodeInit) && (PathMatrix[node, nodeInit] != node) && (node != -1))
                {
                    readPathMatrix(node, nodeInit);
                }

                if ((PathMatrix[nodeEnd, node] != nodeEnd) && (PathMatrix[nodeEnd, node] != node) && (node != -1))
                {
                    readPathMatrix(nodeEnd, node);
                }
                else if ((PathMatrix[node, nodeEnd] != nodeEnd) && (PathMatrix[node, nodeEnd] != node) && (node != -1))
                {
                    readPathMatrix(node, nodeEnd);
                }
            }
        }
        public List<XYZ> organizePath(int nodeInit, int nodeEnd, XYZ nodePanel, List<XYZ> element)
        {
            List<XYZ> path1 = new List<XYZ>();

            if (nodePanel != null)
            {
                path1.Add(nodePanel);
                if (nodePanel.Z != GetXYZNode(nodeInit).Z)
                {
                    XYZ a = new XYZ(nodePanel.X, nodePanel.Y, GetXYZNode(nodeInit).Z);
                    path1.Add(a);
                }
            }
            path1.Add(GetXYZNode(nodeInit));
            path.Remove(nodeInit);

            while (true)
            {
                //TODO: evitar el bucle infinito si i no aumenta
                foreach (int node in path)
                {
                    if (AdjacencyMatrix0[nodeInit, node] == 1)
                    {
                        if (Math.Abs(path1.Last().Z-GetXYZNode(node).Z)>1e-9)
                        {
                            XYZ a = new XYZ(path1.Last().X, path1.Last().Y, GetXYZNode(node).Z);
                            path1.Add(a);
                        }
                        path1.Add(GetXYZNode(node));
                        path.Remove(node);
                        nodeInit = node;
                        break;
                    }
                }
                if (nodeInit == nodeEnd)
                {
                    break;
                }
            }
            foreach (XYZ elem in element)
            {
                if (path1.Last().Z != elem.Z)
                {
                    XYZ a = new XYZ(path1.Last().X, path1.Last().Y, elem.Z);
                    path1.Add(a);
                }
                path1.Add(elem);
            }

            return path1;
        }
    }
}
