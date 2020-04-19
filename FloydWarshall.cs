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

//MATRIZ DE INCIDENCIA:
//Cada columna representa una edges del grafo y cada fila un nodo
//los elementos que la componen son ceros y unos: uno si la edges y el nodo correspondientes tienen relacion directa y cero si no.
//MATRIZ DE ADYACENCIA:

//Es una matriz cuadrada donde tanto las filas con mo las columnas representan todos los Aristas del grafo.
//Los elementos que la componen son ceros y unos: uno si el nodo de la fila esta conectado por medio de una edges al modo de la columna y cero si no.
//si la cominicacion es bidireccional la matriz es simetrica.

//SECUENCIA DE GRADOS:
//es un vector en el que cada elemento representa el numero de aristas que llegan o salen de un vertice.

//Algoritmo de Floyd-Warshall
//https://es.wikipedia.org/wiki/Algoritmo_de_Floyd-Warshall
//https://www.programmingalgorithms.com/algorithm/floyd%E2%80%93warshall-algorithm/
//https://www.csharpstar.com/floyd-warshall-algorithm-csharp/

namespace Revit_ManageElectricalCircuit
{
    public class FloydWarshall
    {
        public double[,] AdjacencyMatrix = null;
        public double[,] AdjacencyMatrix0 = null;
        public int[,] PathMatrix = null;
        public List<Node> nodos = new List<Node> { };
        public List<Edge> Edges = new List<Edge> { };
        public List<int> path = new List<int>();
        public Graph graph = new Graph();

        public FloydWarshall(ref Graph graph1)
        {
            graph = graph1;
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
            foreach (Edge elem in Edges)
            {
                AdjacencyMatrix[elem.nodeA.Name, elem.nodeB.Name] = elem.Lenth;
                AdjacencyMatrix[elem.nodeB.Name, elem.nodeA.Name] = elem.Lenth;
                AdjacencyMatrix0[elem.nodeA.Name, elem.nodeB.Name] = 1;
                AdjacencyMatrix0[elem.nodeB.Name, elem.nodeA.Name] = 1;
            }
            return AdjacencyMatrix;
        }
        public const double PositiveInfinity = 1.5e300;
        public int[,] PlayFloydWarshall()
        {
            nodos = graph.Nodes;
            Edges = graph.Edges;

            GetAdjacencyMatrix();

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
                                //PathMatrix[i, j] = k;
                                PathMatrix[i, j] = PathMatrix[k, i];
                            }
                        }
                    }
                }
            }
            return PathMatrix;
        }
        public string GetPath(int nodeInit, int nodeEnd)
        {
            //http://web.mit.edu/urban_or_book/www/book/chapter6/6.2.2.html
            //https://medium.com/@vighneshtiwari16377/floyd-warshall-dynamic-programming-algorithm-e2a899c3e5e6
            path.Clear();
            path.Add(nodeInit);
            path.Add(nodeEnd);

            string mesage = null;
            mesage = readPathMatrix(nodeInit, nodeEnd);
            List<int> path1 = new List<int>(path);
            
            path.Clear();
            path.Add(nodeInit);
            path1.Remove(nodeInit);
            
            if (mesage == "Correct")
            {
                while (true)
                {
                    //TODO: evitar el bucle infinito si i no aumenta
                    foreach (int elem in path1)
                    {
                        if (AdjacencyMatrix0[nodeInit, elem] == 1)
                        {
                            path.Add(elem);
                            path1.Remove(elem);
                            nodeInit = elem;
                            break;
                        }
                    }
                    if (nodeInit == nodeEnd)
                    {
                        break;
                    }
                }
            }
            
            return mesage;
        }
        public XYZ GetXYZNode(int node)
        {
            XYZ point = null;
            foreach (Node elem in nodos)
            {
                if (node == elem.Name)
                {
                    point = elem.Location;
                    break;
                }
            }
            return point;
        }
        public string readPathMatrix(int nodeInit, int nodeEnd)
        {
            //TODO: evitar el bucles infinitos si puediera ocurrir
            //TODO: que pasa si no hay bandejas
            //TODO: que pasa si el sistema de bajdejas esta dividido en dos grupos que no estan conectados
            string mesage = "Correct";
            
            if (((PathMatrix[nodeInit, nodeEnd] != nodeInit) && (PathMatrix[nodeInit, nodeEnd] != nodeEnd))|| ((PathMatrix[nodeEnd, nodeInit] != nodeInit) && (PathMatrix[nodeEnd, nodeInit] != nodeEnd)))
            {
                int node = nodeEnd;
                if (AdjacencyMatrix[nodeInit, nodeEnd] < AdjacencyMatrix[nodeEnd, nodeInit])
                {
                    node = PathMatrix[nodeInit, nodeEnd];
                }
                else
                {
                    node = PathMatrix[nodeEnd, nodeInit];
                }
                //node = PathMatrix[nodeInit, node];
                
                
                path.Add(node);
                if ((node != -1))
                {
                    if ((PathMatrix[nodeInit, node] != nodeInit) && (PathMatrix[nodeInit, node] != node))
                    {
                        mesage = readPathMatrix(nodeInit, node);
                    }
                    else if ((PathMatrix[node, nodeInit] != nodeInit) && (PathMatrix[node, nodeInit] != node))
                    {
                        mesage = readPathMatrix(node, nodeInit);
                    }

                    if ((PathMatrix[nodeEnd, node] != nodeEnd) && (PathMatrix[nodeEnd, node] != node))
                    {
                        mesage = readPathMatrix(nodeEnd, node);
                    }
                    else if ((PathMatrix[node, nodeEnd] != nodeEnd) && (PathMatrix[node, nodeEnd] != node))
                    {
                        mesage = readPathMatrix(node, nodeEnd);
                    }
                }
                else
                {
                    mesage = "The sistem is not valid";
                }
            }
            
            return mesage;
        }
        public List<XYZ> organizePath(XYZ nodePanel, Graph element)
        {
            List<XYZ> path1 = new List<XYZ>();
            //DOTO:la distancia entre dos puntos no debe ser menos que la tolerancia
            if (nodePanel != null)
            {
                path1.Add(nodePanel);
            }
            foreach (int elem in path)
            {
                if (Math.Abs(path1.Last().Z - GetXYZNode(elem).Z) > 1e-9)
                {
                    XYZ a = new XYZ(path1.Last().X, path1.Last().Y, GetXYZNode(elem).Z);
                    path1.Add(a);
                }
                path1.Add(GetXYZNode(elem));
            }
            foreach (Node elem in element.Nodes)
            {
                if (Math.Abs(path1.Last().Z - elem.Location.Z) > 1e-9)
                {
                    XYZ a = new XYZ(path1.Last().X, path1.Last().Y, elem.Location.Z);
                    path1.Add(a);
                }
                path1.Add(elem.Location);
            }
            return path1;
        }
    }
}
