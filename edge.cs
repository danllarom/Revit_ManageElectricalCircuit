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
    public struct edges
    {
        public edges(int nodeA, int nodeB, double lenth)
        {
            NodeA = nodeA;
            NodeB = nodeB;
            Lenth = lenth;
        }
        public int NodeA { get; set; }
        public int NodeB { get; set; }
        public double Lenth { get; set; }
    }
    class Edge
    {
        public Node nodeA = null;
        public Node nodeB = null;
        public double Lenth { get;}
        public Edge() {}
        public Edge(ref Node nodea, ref Node nodeb)
        {
            nodeA = nodea;
            nodeB = nodeb;
            Lenth = Math.Abs(nodeA.Location.Subtract(nodeA.Location).GetLength());
        }
    }
}
