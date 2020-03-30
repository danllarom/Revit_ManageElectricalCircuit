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

using System.Windows.Forms;
using System.Windows.Media.Imaging; //https://archi-lab.net/create-your-own-tab-and-buttons-in-revit/
using System.Diagnostics;




//OTHER
//https://thebuildingcoder.typepad.com/blog/2010/07/retrieve-structural-elements.html

namespace Revit_ManageElectricalCircuit
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //Get application and documnet objects
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            try
            {
                Transaction trans2 = new Transaction(doc);
                trans2.Start("Lab");
                //Windos form
                Form1 ventana = new Form1(doc);
                ventana.Show();


                //get all cable tray element in de model
                FilteredElementCollector Collector = new FilteredElementCollector(doc);
                Collector = GetConnectorElements(doc, false);

                Graph connectorsSistem = new Graph();
                connectorsSistem.AddConnectorSetFromFilteredElementCollector(Collector);
                //connectorsSistem.RenameNode();

                FloydWarshall floydWarshall = new FloydWarshall(ref connectorsSistem);
                floydWarshall.PlayFloydWarshall();

                //MessageBox.Show(floydWarshall.nodos.Count().ToString());

                //Pick Circuit
                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                collector1.OfCategory(BuiltInCategory.OST_ElectricalCircuit);

                foreach (ElectricalSystem elem in collector1)
                {
                    Node nodeA = new Node();
                    Node nodeB = new Node();
                    Node nodeC = new Node();

                    LocationPoint locationPanel = elem.BaseEquipment.Location as LocationPoint;
                    XYZ XYZPanel = locationPanel.Point;
                    nodeA.Location = XYZPanel;
                    nodeA = floydWarshall.graph.closeNode(nodeA);

                    Graph receptor = new Graph();
                    receptor.AddXYZFromElementSet(elem.Elements);
                    //TODO: calculate the short cut for the receptor
                    receptor.moreCloseNodes(ref floydWarshall.graph.Nodes, ref nodeC, ref nodeB);
                    
                    List<XYZ> CircuitsElement = new List<XYZ>();
                    foreach (FamilyInstance elem1 in elem.Elements)
                    {
                        LocationPoint locationelem = elem1.Location as LocationPoint;
                        XYZ XYZelem = locationelem.Point;
                        CircuitsElement.Add(XYZelem);
                    }
                    floydWarshall.GetPath(nodeA.Name, nodeB.Name);

                    
                    elem.SetCircuitPath(floydWarshall.organizePath(nodeA.Name, nodeB.Name, XYZPanel, CircuitsElement));
                    
                }
                trans2.Commit();
            }
            //If the user right-clicks or presses Esc, handle the exception
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            //Catch other errors
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }
        public class ElectricalCircuitPickFilter : ISelectionFilter
        {
            public bool AllowElement(Element e)
            {
                return (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_ElectricalCircuit));
            }
            public bool AllowReference(Reference r, XYZ p)
            {
                return false;
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