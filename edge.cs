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
    public class Edge
    {
        public Node nodeA = null;
        public Node nodeB = null;
        public double Lenth { get;}
        public Edge() {}
        public Edge(ref Node nodea, ref Node nodeb)
        {
            nodeA = nodea;
            nodeB = nodeb;

            XYZ a = nodeA.Location.Subtract(nodeB.Location);
            double b = a.GetLength();
            Lenth = Math.Abs(b);
        }
    }
}
