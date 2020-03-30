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
        public Form1(Document Doc)
        {
            //TODO: Add filter to column.
            doc = Doc;
            InitializeComponent();
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_ElectricalCircuit);

            FilteredElementCollector collector2 = new FilteredElementCollector(doc);
            collector2.OfCategory(BuiltInCategory.OST_Levels);

            List<Level> levels = new List<Level>();
            //TODO: what happen when a level take default option of this program?
            List<string> levelOption = new List<string>{ "None", "Level Upper", "Level Lower"};
            foreach (Element elem in collector2)
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

            List<Circuit> cicuits = new List<Circuit>();
            
            foreach (ElectricalSystem elem in collector)
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
    }
}
