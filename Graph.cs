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
    
    class Graph
    {
        public List<Node> Nodes = new List<Node> { };
        public List<Edge> Edges = new List<Edge> { };
        public int NodesCount;

        public ConnectorSet Connectors;
        public Graph()
        {
        }
        public Graph(ConnectorSet connectors)
        {
            Connectors = connectors;
        }
        public void GetNode(XYZ P, ref Node node)
        {
            var testWasTrue = false;
            foreach (Node elem in Nodes)
            {
                if (Math.Abs(P.Subtract(elem.Location).GetLength()) <= 1e-9)
                {
                    node = elem;
                    testWasTrue = true;
                    break;
                }
            }
            if (testWasTrue == false)
            {
                node = new Node(NodesCount, P, null);
                Nodes.Add(node);
                NodesCount++;
            }
        }
        public void AddEdge(XYZ P1, XYZ P2)
        {
            Edge edge = new Edge();

            GetNode(P1, ref edge.nodeA);
            GetNode(P2, ref edge.nodeB);

            Edges.Add(edge);
        }
        public void AddConnectorSet(ConnectorSet connectors)
        {
            if (connectors.Size == 3)
            {
                List<XYZ> P = new List<XYZ>();
                XYZ Pcentro = null;
                double[] L = new double[3];

                foreach (Connector elem1 in connectors) { P.Add(elem1.Origin); }
                
                L[0] = Math.Abs(P[0].Subtract(P[1]).GetLength());
                L[1] = Math.Abs(P[1].Subtract(P[2]).GetLength());
                L[2] = Math.Abs(P[2].Subtract(P[0]).GetLength());

                double Lmax = Math.Max(L[0], Math.Max(L[1], L[2]));

                if (Lmax == L[0]) {Pcentro = P[0].Subtract(P[1]).Divide(2).Add(P[1]);}
                if (Lmax == L[1]) {Pcentro = P[1].Subtract(P[2]).Divide(2).Add(P[2]);}
                if (Lmax == L[2]) {Pcentro = P[2].Subtract(P[0]).Divide(2).Add(P[0]);}

                foreach (XYZ elem in P) { AddEdge(elem, Pcentro); }
            }
            if (connectors.Size == 4)
            {
                List<XYZ> P = new List<XYZ>();

                foreach (Connector elem in connectors) { P.Add(elem.Origin);}

                XYZ Pcentro = P[0].Add(P[1]).Add(P[2]).Add(P[3]).Divide(4);

                foreach (XYZ elem in P) { AddEdge(elem, Pcentro); }
            }
            if (connectors.Size == 2)
            {
                List<XYZ> P = new List<XYZ>();

                foreach (Connector elem in connectors) {P.Add(elem.Origin);}

                AddEdge(P[0], P[1]);
            }
        }
        public void AddConnectorSetFromElementConnector(Element elem)
        {
            ConnectorSet connectors = GetConnectors(elem);
            AddConnectorSet(connectors);
        }
        public void AddConnectorSetFromFilteredElementCollector(FilteredElementCollector Collector)
        {
            foreach (Element elem in Collector)
            {
                AddConnectorSetFromElementConnector(elem);
            }
        }
        public void AddXYZFromElementSet(ElementSet Collector)
        {
            foreach (Element elem in Collector)
            {
                LocationPoint locationelem = elem.Location as LocationPoint;
                XYZ XYZelem = locationelem.Point;
                Node node = new Node();
                GetNode(XYZelem, ref node);
            }
        }
        public void moreCloseNodes(ref List<Node> nodes, ref Node Node, ref Node node)
        {
            Node = Nodes.First();
            node = nodes.First();
            foreach (Node elem in Nodes)
            {
                foreach (Node elem1 in nodes)
                {
                    if (Math.Abs(elem.Location.Subtract(elem1.Location).GetLength()) < Math.Abs(Node.Location.Subtract(node.Location).GetLength()))
                    {
                        Node = elem;
                        node = elem1;
                    }
                }
            }
        }
        public void closeNode(Node nodeA, ref Node closeNodo)
        {
            int j = 0;
            foreach (Node elem in Nodes)
            {
                if (j == 0)
                {
                    closeNodo = elem;
                    j++;
                }
                else if (Math.Abs(nodeA.Location.Subtract(elem.Location).GetLength()) < Math.Abs(nodeA.Location.Subtract(closeNodo.Location).GetLength()))
                {
                    closeNodo = elem;
                    j++;
                }
            }
        }
        public void RenameNode()
        {
            int i = 0;
            foreach (Node elem in Nodes)
            {
                elem.Name = i;
                i++;
            }
            NodesCount = Nodes.Count();
        }
        static ConnectorSet GetConnectors(Element e)
        {
            ConnectorSet connectors = null;

            if (e is FamilyInstance)
            {
                MEPModel m = ((FamilyInstance)e).MEPModel;

                if (null != m && null != m.ConnectorManager)
                {
                    connectors = m.ConnectorManager.Connectors;
                }
            }
            else if (e is Wire)
            {
                connectors = ((Wire)e).ConnectorManager.Connectors;
            }
            else
            {
                Debug.Assert(e.GetType().IsSubclassOf(typeof(MEPCurve)), "expected all candidate connector provider "
                    + "elements to be either family instances or "
                    + "derived from MEPCurve");

                if (e is MEPCurve)
                {
                    connectors = ((MEPCurve)e).ConnectorManager.Connectors;
                }
            }
            return connectors;
        }   
    }
}