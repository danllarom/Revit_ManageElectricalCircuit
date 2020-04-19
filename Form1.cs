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
using System.Windows.Media.Imaging;
using System.Reflection;

//https://www.encodedna.com/2013/02/show-combobox-datagridview.htm

//advanced datagridview: to create ilter in datagridview column

namespace Revit_ManageElectricalCircuit
{
    public struct Circuit
    {
        public Circuit(Boolean select, string panelName, string upperLevelPanel, string lowerLevelPanel, string circuitNumber, string circuitName, string upperLevelElem, string lowerLevelElem, string serviceType, ElectricalSystem elementcircuit)
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
            ServiceType = serviceType;
        }
        public Boolean Select { get; set; }
        public string PanelName { get; set; }
        public string UpperLevelPanel { get; set; }
        public string LowerLevelPanel { get; set; }
        public string CircuitNumber { get; set; }
        public string CircuitName { get; set; }
        public string UpperLevelElem { get; set; }
        public string LowerLevelElem { get; set; }
        public string ServiceType { get; set; }
        public ElectricalSystem ElementCircuit { get; set; }
    }
    public struct GraphSystem
    {
        public GraphSystem(Graph graph1, FloydWarshall FloydWarshall1)
        {
            //TODO: add service type
            Graph = graph1;
            FloydWarshall = FloydWarshall1;
        }
        public Graph Graph { get; set; }
        public FloydWarshall FloydWarshall { get; set; }
    }
    public partial class Form1 : Form
    {
        public bool Cancelclose = false; 
        public Document doc = null;
        public FilteredElementCollector LevelsCollector;
        Dictionary<string, Level> levels = new Dictionary<string, Level>();
        Dictionary<int, Circuit> circuits = new Dictionary<int, Circuit>();
        Dictionary<string, GraphSystem> serviceTypes = new Dictionary<string, GraphSystem>();
        public Form1(Document Doc)
        {
            InitializeComponent();

            doc = Doc;

            //get servicetype, circuits and levels
            serviceTypes = ServiceTypeSystem(Doc);
            circuits = GetCircuits(Doc);
            levels = GetLevels(Doc);

            //add default data
            dataGridView1.Rows.Clear();
            foreach (string elem in levels.Keys)
            {
                UpperLevelPanel.Items.Add(elem);
                LowerLevelPanel.Items.Add(elem);
                UpperLevelElement.Items.Add(elem);
                LowerLevelElement.Items.Add(elem);
            }
            foreach (var elem in serviceTypes.Keys)
            {
                ServiceType.Items.Add(elem);
            }
            for (int i = 0; i < circuits.Count(); i++)
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
                dataGridView1.Rows[n].Cells[8].Value = circuits[n].ServiceType;
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
        private void Cancel_Click(object sender, EventArgs e)
        {
            Cancelclose = true;
            this.Close();
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
            for (int i = 0; i < circuits.Count(); i++)
            {
                //TODO: crear exception propias
                //https://docs.microsoft.com/es-es/dotnet/csharp/programming-guide/exceptions/creating-and-throwing-exceptions
                dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.White;
                if (dataGridView1.Rows[i].Cells[0].Value.Equals(true))
                {
                    Node nodeA = new Node();
                    Node nodeB = new Node();
                    Node nodeC = new Node();

                    string serviceTypeName = dataGridView1.Rows[i].Cells[8].Value.ToString();
                    LocationPoint locationPanel = circuits[i].ElementCircuit.BaseEquipment.Location as LocationPoint;
                    XYZ XYZPanel = locationPanel.Point;
                    nodeA.Location = XYZPanel;

                    //get node more closed at the panel
                    List<string> mesageError = new List<string>();
                    string mesage = serviceTypes[serviceTypeName].FloydWarshall.graph.closeNode(nodeA, ref nodeA, levels[dataGridView1.Rows[i].Cells[2].Value.ToString()], levels[dataGridView1.Rows[i].Cells[3].Value.ToString()]);
                    if (mesage != "Correct") { mesageError.Add(mesage); }

                    //get nodes more closet betwy element and system
                    Graph receptor = new Graph();
                    receptor.AddXYZFromElementSet(circuits[i].ElementCircuit.Elements);
                    //TODO: calculate the short cut for the receptor

                    mesage = receptor.moreCloseNodes(ref serviceTypes[serviceTypeName].FloydWarshall.graph.Nodes, ref nodeC, ref nodeB);
                    if (mesage != "Correct") { mesageError.Add(mesage); }

                    //get path in the system
                    mesage = serviceTypes[serviceTypeName].FloydWarshall.GetPath(nodeA.Name, nodeB.Name);
                    if (mesage != "Correct") { mesageError.Add(mesage);}

                    if (mesageError.Count() == 0)
                    {
                        circuits[i].ElementCircuit.SetCircuitPath(serviceTypes[serviceTypeName].FloydWarshall.organizePath(XYZPanel, receptor));
                        if (circuits[i].ElementCircuit.HasCustomCircuitPath)
                        {
                            circuits[i].ElementCircuit.CircuitPathMode = ElectricalCircuitPathMode.Custom;
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.ForestGreen;
                        }
                        else
                        {
                            dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                    }
                }

                progressBar1.Value = (int)Math.Round((double)((i + 1) / circuits.Count()) * 100);
                label2.Text = progressBar1.Value.ToString() + "%";
                label3.Text = circuits[i].PanelName.ToString() + " - " + circuits[i].CircuitNumber.ToString() + " - " + circuits[i].CircuitName.ToString();
                if (progressBar1.Value == 100)
                {
                    label3.Text = "Completed";
                }
            }
        }
        private Dictionary<string, GraphSystem> ServiceTypeSystem(Document Doc)
        {
            Dictionary<string, GraphSystem> ServiceTypes = new Dictionary<string, GraphSystem>();

            //get all cable tray element in de model
            FilteredElementCollector Collector = new FilteredElementCollector(doc);
            Collector = GetConnectorElements(doc, false);

            //filter service type
            //https://thebuildingcoder.typepad.com/blog/2010/06/parameter-filter.html#7
            BuiltInParameter testParam = BuiltInParameter.RBS_CTC_SERVICE_TYPE;
            List<string> serviceTypeNames = new List<string>();
            foreach (Element elem in Collector)
            {
                Parameter param = elem.get_Parameter(testParam);
                string paremeterValue = param.AsString();
                if (paremeterValue != null)
                {
                    if (!serviceTypeNames.Contains(paremeterValue))
                    {
                        serviceTypeNames.Add(paremeterValue);
                    }
                }
            }
            foreach (string key in serviceTypeNames)
            {

                //create Filter servicetype cable tray
                //https://thebuildingcoder.typepad.com/blog/2010/06/parameter-filter.html#7
                ParameterValueProvider pvp = new ParameterValueProvider(new ElementId((int)testParam));
                FilterStringRuleEvaluator fnrvStr = new FilterStringContains();
                FilterStringRule paramFr = new FilterStringRule(pvp, fnrvStr, key, false);
                ElementParameterFilter epf = new ElementParameterFilter(paramFr);

                //Filter servicetype cable tray
                FilteredElementCollector CollectorKey = new FilteredElementCollector(doc);
                CollectorKey = GetConnectorElements(doc, false);
                CollectorKey.WherePasses(epf);

                //create graph to cable tray
                Graph connectorsSistem1 = new Graph();
                connectorsSistem1.AddConnectorSetFromFilteredElementCollector(CollectorKey);

                //create floydWarshall to all cable tray
                FloydWarshall floydWarshall1 = new FloydWarshall(ref connectorsSistem1);
                floydWarshall1.PlayFloydWarshall();

                //Add service type system
                GraphSystem graphSystem1 = new GraphSystem(connectorsSistem1, floydWarshall1);
                ServiceTypes.Add(key, graphSystem1);
            }

            //create graph to all cable tray
            Graph connectorsSistem = new Graph();
            connectorsSistem = new Graph();
            connectorsSistem.AddConnectorSetFromFilteredElementCollector(Collector);

            //create floydWarshall to all cable tray
            FloydWarshall floydWarshall;
            floydWarshall = new FloydWarshall(ref connectorsSistem);
            floydWarshall.PlayFloydWarshall();

            //Add service type system
            GraphSystem graphSystem = new GraphSystem(connectorsSistem, floydWarshall);
            ServiceTypes.Add("None", graphSystem);

            return ServiceTypes;
        }
        private Dictionary<int, Circuit> GetCircuits(Document Doc)
        {
            Dictionary<int, Circuit> Circuits = new Dictionary<int, Circuit>();
            //Pick Circuit
            FilteredElementCollector CircuitsCollector = new FilteredElementCollector(doc);
            CircuitsCollector.OfCategory(BuiltInCategory.OST_ElectricalCircuit);

            int j = 0;
            foreach (ElectricalSystem elem in CircuitsCollector)
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
                Circuit circuit = new Circuit(false, panelName, "Level Upper", "Level Lower", elem.CircuitNumber, elem.LoadName, "Level Upper", "Level Lower", "None", elem);
                Circuits.Add(j, circuit);
                j++;
            }
            return Circuits;
        }
        private Dictionary<string, Level> GetLevels(Document Doc)
        {
            Dictionary<string, Level> levels = new Dictionary<string, Level>();
            //Pick Levels
            FilteredElementCollector Levels = new FilteredElementCollector(doc);
            Levels.OfCategory(BuiltInCategory.OST_Levels);

            //TODO: what happen when a level take default option of this program?
            this.levels.Add("None", null);
            this.levels.Add("Level Upper", null);
            this.levels.Add("Level Lower", null);

            foreach (Element elem in Levels)
            {
                this.levels.Add(elem.Name, elem as Level);
            }
            return this.levels;
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
    }
}