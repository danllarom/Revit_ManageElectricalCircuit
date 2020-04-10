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
        Dictionary<string, Level> levels = new Dictionary<string, Level>();
        Dictionary<int, Circuit> circuits = new Dictionary<int, Circuit>();

        public Form1(Document Doc)
        {
            doc = Doc;
            


            //get all cable tray element in de model
            FilteredElementCollector Collector = new FilteredElementCollector(doc);
            Collector = GetConnectorElements(doc, false);

            //filter service type
            //https://thebuildingcoder.typepad.com/blog/2010/06/parameter-filter.html#7
            BuiltInParameter testParam = BuiltInParameter.RBS_CTC_SERVICE_TYPE;
            Dictionary<string, FilteredElementCollector> serviceType = new Dictionary<string, FilteredElementCollector>();
            foreach (Element elem in Collector)
            {
                Parameter param = elem.get_Parameter(testParam);
                if (param != null)
                {
                    //if (!serviceType.ContainsKey(param.AsString()))
                    //{
                    //    serviceType.Add(param.AsString(), null);
                    //}                        
                }
            }
            foreach(string key in serviceType.Keys)
            {
                String ruleValStr = key.ToString();
                ParameterValueProvider pvp = new ParameterValueProvider(new ElementId((int)testParam));
                FilterStringRuleEvaluator fnrvStr = new FilterStringContains();
                FilterStringRule paramFr = new FilterStringRule(pvp, fnrvStr, ruleValStr, false);
                ElementParameterFilter epf = new ElementParameterFilter(paramFr);
                //Collector.OfClass(typeof(CableTray)).WherePasses(epf); // only deal with CableTray
                FilteredElementCollector CollectorKey =Collector;
                CollectorKey.WherePasses(epf);
                serviceType[key] = CollectorKey;
            }
            serviceType.Add("None", Collector);

            //TODO: select service type
            //String ruleValStr = serviceType.First();
            
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
            levels.Add("None", null);
            levels.Add("Level Upper", null);
            levels.Add("Level Lower", null);

            foreach (Element elem in Levels)
            {
                levels.Add(elem.Name, elem as Level);
            }

            foreach (string elem in levels.Keys)
            {
                UpperLevelPanel.Items.Add(elem);
                LowerLevelPanel.Items.Add(elem);
                UpperLevelElement.Items.Add(elem);
                LowerLevelElement.Items.Add(elem);
            }
            int j = 0;
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
                Circuit circuit = new Circuit(false, panelName, "Level Upper", "Level Lower", elem.CircuitNumber, elem.LoadName, "Level Upper", "Level Lower", elem);
                circuits.Add(j, circuit);
                j++;
            }

            dataGridView1.Rows.Clear();
            for (int i = 0; i < Circuits.Count(); i++)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = circuits[n].Select;
                dataGridView1.Rows[n].Cells[1].Value = circuits[n].PanelName;
                dataGridView1.Rows[n].Cells[2].Value = circuits[n].UpperLevelPanel;
                dataGridView1.Rows[n].Cells[3].Value = circuits[n].LowerLevelPanel;
                dataGridView1.Rows[n].Cells[4].Value = circuits[n].CircuitNumber;
                dataGridView1.Rows[n].Cells[5].Value = circuits[n].CircuitName;
                dataGridView1.Rows[n].Cells[6].Value = circuits[n].UpperLevelElem;
                dataGridView1.Rows[n].Cells[7].Value = circuits[n].LowerLevelElem;
            }
            label1.Text = "Count: " + dataGridView1.Rows.Count.ToString();
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
            ApplyChanges();
            this.Close();
        }
        static FilteredElementCollector GetConnectorElements(Document doc, bool include_wires)
        {
            //https://thebuildingcoder.typepad.com/blog/2010/06/retrieve-mep-elements-and-connectors.html
            // what categories of family instances
            // are we interested in?

            BuiltInCategory[] bics = new BuiltInCategory[] {
                BuiltInCategory.OST_CableTray,
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
        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Apply_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }
        private void ApplyChanges()
        {
            progressBar1.Value = 0;
            for (int i = 0; i < Circuits.Count(); i++)
            {
                //TODO: crear exception propias
                //https://docs.microsoft.com/es-es/dotnet/csharp/programming-guide/exceptions/creating-and-throwing-exceptions
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                if (dataGridView1.Rows[i].Cells[0].Value.Equals(true))
                {
                    Node nodeA = new Node();
                    Node nodeB = new Node();
                    Node nodeC = new Node();

                    LocationPoint locationPanel = circuits[i].ElementCircuit.BaseEquipment.Location as LocationPoint;
                    XYZ XYZPanel = locationPanel.Point;
                    nodeA.Location = XYZPanel;

                    //get node more closed at the panel
                    List<string> mesageError = new List<string>();
                    string mesage = floydWarshall.graph.closeNode(nodeA, ref nodeA, levels[dataGridView1.Rows[i].Cells[2].Value.ToString()], levels[dataGridView1.Rows[i].Cells[3].Value.ToString()]);
                    if (mesage != "Correct") { mesageError.Add(mesage);}

                    //get nodes more closet betwy element and system
                    Graph receptor = new Graph();
                    receptor.AddXYZFromElementSet(circuits[i].ElementCircuit.Elements);
                    //TODO: calculate the short cut for the receptor

                    mesage = receptor.moreCloseNodes(ref floydWarshall.graph.Nodes, ref nodeC, ref nodeB);
                    if (mesage != "Correct") { mesageError.Add(mesage); }

                    //get path in the system
                    mesage = floydWarshall.GetPath(nodeA.Name, nodeB.Name);
                    if (mesage != "Correct") { mesageError.Add(mesage); }

                    if (mesageError.Count()==0)
                    {
                        circuits[i].ElementCircuit.SetCircuitPath(floydWarshall.organizePath(XYZPanel, receptor));
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.ForestGreen;
                    }
                    else
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                    }
                }

                progressBar1.Value =(int)Math.Round((double)((i+1)/Circuits.Count())*100);
                label2.Text =progressBar1.Value.ToString() + "%";
                label3.Text = circuits[i].PanelName.ToString() + " - " + circuits[i].CircuitNumber.ToString() + " - " + circuits[i].CircuitName.ToString();
                if (progressBar1.Value == 100)
                {
                    label3.Text = "Completed";
                }
            }
        }
    }
}
