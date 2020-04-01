using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Electrical;
using Form = System.Windows.Forms.Form;

//https://www.encodedna.com/2013/02/show-combobox-datagridview.htm

//advanced datagridview: to create ilter in datagridview column

namespace Revit_ManageElectricalCircuit
{
    public struct Circuit
    {
        public Circuit(Boolean select, string panelName, string upperLevelPanel, string lowerLevelPanel, string circuitNumber, string circuitName, string upperLevelElem, string lowerLevelElem, ElectricalSystem elementcircuit)
        {
            //TODO: add service type
            Select = select;
            PanelName = panelName;
            UpperLevelPanel = upperLevelPanel;
            LowerLevelPanel = lowerLevelPanel;
            CircuitNumber = circuitNumber;
            CircuitName = circuitName;
            UpperLevelElem = upperLevelElem;
            LowerLevelElem = lowerLevelElem;
            ElementCircuit = elementcircuit;
        }
        public Boolean Select { get; set; }
        public string PanelName { get; set; }
        public string UpperLevelPanel { get; set; }
        public string LowerLevelPanel { get; set; }
        public string CircuitNumber { get; set; }
        public string CircuitName { get; set; }
        public string UpperLevelElem { get; set; }
        public string LowerLevelElem { get; set; }
        public ElectricalSystem ElementCircuit { get; set; }
    }
    

    public partial class Form1 : Form
    {
        public Document doc = null;
        private Graph connectorsSistem = new Graph();
        private FloydWarshall floydWarshall;
        public FilteredElementCollector Circuits;
        public FilteredElementCollector LevelsCollector;
        List<Level> levels = new List<Level>();
        List<Circuit> cicuits = new List<Circuit>();

        public Form1(Document Doc)
        {
            doc = Doc;

            //get all cable tray element in de model
            FilteredElementCollector Collector = new FilteredElementCollector(doc);
            Collector = GetConnectorElements(doc, false);

            //create graph to cable tray
            connectorsSistem = new Graph();
            connectorsSistem.AddConnectorSetFromFilteredElementCollector(Collector);

            //calculate floydWarshall
            floydWarshall = new FloydWarshall(ref connectorsSistem);
            //TODO: incluede playfloydWarshall in constructor.
            floydWarshall.PlayFloydWarshall();

            //Pick Circuit
            Circuits = new FilteredElementCollector(doc);
            Circuits.OfCategory(BuiltInCategory.OST_ElectricalCircuit);

            //Pick Levels
            FilteredElementCollector Levels = new FilteredElementCollector(doc);
            Levels.OfCategory(BuiltInCategory.OST_Levels);

            InitializeComponent();

            
            //TODO: what happen when a level take default option of this program?
            List<string> levelOption = new List<string>{ "None", "Level Upper", "Level Lower"};
            foreach (Element elem in Levels)
            {
                levels.Add(elem as Level);
                levelOption.Add(elem.Name);
            }

            foreach (string elem in levelOption)
            {
                Column5.Items.Add(elem);
                Column6.Items.Add(elem);
                Column7.Items.Add(elem);
                Column8.Items.Add(elem);
            }

            foreach (ElectricalSystem elem in Circuits)
            {
                //TODO: is correct use "none" in ths case?
                string panelName = null;
                if (elem.PanelName != null)
                {
                    panelName = elem.PanelName;
                }
                else
                {
                    panelName = "None";
                }
                Circuit circuit = new Circuit(false, panelName, "None" , "None", elem.CircuitNumber, elem.LoadName, "None", "None", elem);
                cicuits.Add(circuit);
            }

            foreach (Circuit elem in cicuits)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = elem.Select;
                dataGridView1.Rows[n].Cells[1].Value = elem.PanelName;
                dataGridView1.Rows[n].Cells[2].Value = elem.UpperLevelPanel;
                dataGridView1.Rows[n].Cells[3].Value = elem.LowerLevelPanel;
                dataGridView1.Rows[n].Cells[4].Value = elem.CircuitNumber;
                dataGridView1.Rows[n].Cells[5].Value = elem.CircuitName;
                dataGridView1.Rows[n].Cells[6].Value = elem.UpperLevelElem;
                dataGridView1.Rows[n].Cells[7].Value = elem.LowerLevelElem;
            }
        }

        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void CheckAll_Click(object sender, EventArgs e)
        {
            //TODO: ccambiar el valor tanto en la tabla como en las lista de circuitos
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = true;
            }
        }

        private void CheckNone_Click(object sender, EventArgs e)
        {
            //TODO: ccambiar el valor tanto en la tabla como en las lista de circuitos
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = false;
            }
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            foreach (ElectricalSystem elem in Circuits)
            {
                Node nodeA = new Node();
                Node nodeB = new Node();
                Node nodeC = new Node();

                LocationPoint locationPanel = elem.BaseEquipment.Location as LocationPoint;
                XYZ XYZPanel = locationPanel.Point;
                nodeA.Location = XYZPanel;
                floydWarshall.graph.closeNode(nodeA, ref nodeA);

                Graph receptor = new Graph();
                receptor.AddXYZFromElementSet(elem.Elements);
                //TODO: calculate the short cut for the receptor
                receptor.moreCloseNodes(ref floydWarshall.graph.Nodes, ref nodeC, ref nodeB);
                floydWarshall.GetPath(nodeA.Name, nodeB.Name);
                elem.SetCircuitPath(floydWarshall.organizePath(XYZPanel, receptor));
            }
        }
        static FilteredElementCollector GetConnectorElements(Document doc, bool include_wires)
        {
            //https://thebuildingcoder.typepad.com/blog/2010/06/retrieve-mep-elements-and-connectors.html
            // what categories of family instances
            // are we interested in?

            BuiltInCategory[] bics = new BuiltInCategory[] {
                //BuiltInCategory.OST_CableTray,
                BuiltInCategory.OST_CableTrayFitting,
                BuiltInCategory.OST_Conduit,
                BuiltInCategory.OST_ConduitFitting,
                //BuiltInCategory.OST_DuctCurves,
                //BuiltInCategory.OST_DuctFitting,
                //BuiltInCategory.OST_DuctTerminal,
                //BuiltInCategory.OST_ElectricalEquipment,
                //BuiltInCategory.OST_ElectricalFixtures,
                //BuiltInCategory.OST_LightingDevices,
                //BuiltInCategory.OST_LightingFixtures,
                //BuiltInCategory.OST_MechanicalEquipment,
                //BuiltInCategory.OST_PipeCurves,
                //BuiltInCategory.OST_PipeFitting,
                //BuiltInCategory.OST_PlumbingFixtures,
                //BuiltInCategory.OST_SpecialityEquipment,
                //BuiltInCategory.OST_Sprinklers,
                //BuiltInCategory.OST_Wire,
            };

            IList<ElementFilter> a
              = new List<ElementFilter>(bics.Count());

            foreach (BuiltInCategory bic in bics)
            {
                a.Add(new ElementCategoryFilter(bic));
            }

            LogicalOrFilter categoryFilter
              = new LogicalOrFilter(a);

            LogicalAndFilter familyInstanceFilter
              = new LogicalAndFilter(categoryFilter,
                new ElementClassFilter(
                  typeof(FamilyInstance)));

            IList<ElementFilter> b
              = new List<ElementFilter>(6);

            b.Add(new ElementClassFilter(typeof(CableTray)));
            b.Add(new ElementClassFilter(typeof(Conduit)));
            //b.Add(new ElementClassFilter(typeof(Duct)));
            //b.Add(new ElementClassFilter(typeof(Pipe)));

            if (include_wires)
            {
                b.Add(new ElementClassFilter(typeof(Wire)));
            }

            b.Add(familyInstanceFilter);

            LogicalOrFilter classFilter
              = new LogicalOrFilter(b);

            FilteredElementCollector collector
              = new FilteredElementCollector(doc);

            collector.WherePasses(classFilter);

            return collector;
        }
    }
}
