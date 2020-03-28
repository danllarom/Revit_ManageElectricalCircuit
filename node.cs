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
    class Node
    {
        public int Name;
        public XYZ Location = null;
        public Node(int name, XYZ location, Connector conector)
        {
            Name = name;
            Location = location;
        }
        public Node closeNode(List<Node> nodos)
        {
            Node closeNodo = null;
            int j = 0;
            foreach (Node elem in nodos)
            {
                if (j == 0)
                {
                    closeNodo = elem;
                    j++;
                }
                else if (Math.Abs(Location.Subtract(elem.Location).GetLength()) < Math.Abs(Location.Subtract(closeNodo.Location).GetLength()))
                {
                    closeNodo = elem;
                    j++;
                }
            }
            return closeNodo;
        }
    }
}
