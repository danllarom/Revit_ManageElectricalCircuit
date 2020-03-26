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
    public struct nodo
    {
        public nodo(int name, XYZ location, Connector conector)
        {
            Name = name;
            Location = location;
            Conector = conector;
        }

        public int Name { get; set; }
        public XYZ Location { get; set; }
        public Connector Conector { get; set; }
    }
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

    class nodes
    {
       
    }
}
