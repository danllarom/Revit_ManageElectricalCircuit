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

//MATRIZ DE INCIDENCIA:
//Cada columna representa una edges del grafo y cada fila un nodo
//los elementos que la componen son ceros y unos: uno si la edges y el nodo correspondientes tienen relacion directa y cero si no.
//MATRIZ DE ADYACENCIA:

//Es una matriz cuadrada donde tanto las filas con mo las columnas representan todos los Aristas del grafo.
//Los elementos que la componen son ceros y unos: uno si el nodo de la fila esta conectado por medio de una edges al modo de la columna y cero si no.
//si la cominicacion es bidireccional la matriz es simetrica.

//SECUENCIA DE GRADOS:
//es un vector en el que cada elemento representa el numero de aristas que llegan o salen de un vertice.

//Algoritmo de Floyd-Warshall
//https://es.wikipedia.org/wiki/Algoritmo_de_Floyd-Warshall
//https://www.programmingalgorithms.com/algorithm/floyd%E2%80%93warshall-algorithm/
//https://www.csharpstar.com/floyd-warshall-algorithm-csharp/


//OTHER
//https://thebuildingcoder.typepad.com/blog/2010/07/retrieve-structural-elements.html

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
                //get all cable tray element in de model
                FilteredElementCollector collectorCableTrayFitting = new FilteredElementCollector(doc);
                collectorCableTrayFitting = GetConnectorElements(doc, false);
                
                FloydWarshall floydWarshall = new FloydWarshall();
                floydWarshall.PlayFloydWarshall(GetLines(collectorCableTrayFitting));

                MessageBox.Show(floydWarshall.nodos.Count().ToString());

                //Pick Circuit
                FilteredElementCollector collector1 = new FilteredElementCollector(doc);
                collector1.OfCategory(BuiltInCategory.OST_ElectricalCircuit);

                nodo nodeA = new nodo();
                nodo nodeB = new nodo();

                foreach (ElectricalSystem circuito in collector1)
                {
                    LocationPoint locationPanel = circuito.BaseEquipment.Location as LocationPoint;
                    XYZ XYZPanel = locationPanel.Point;

                    int j = 0;
                    foreach (nodo node in floydWarshall.nodos)
                    {
                        if (j == 0)
                        {
                            nodeA.Location = node.Location;
                            nodeA.Name = node.Name;
                            j++;
                        }
                        else if (Math.Abs(XYZPanel.Subtract(node.Location).GetLength()) < Math.Abs(XYZPanel.Subtract(nodeA.Location).GetLength()))
                        {
                            nodeA.Location = node.Location;
                            nodeA.Name = node.Name;
                            j++;
                        }
                    }
                    j = 0;
                    List<XYZ> CircuitsElement = new List<XYZ>();
                    foreach (Element elem in circuito.Elements)
                    {
                        FamilyInstance elemCircuit = elem as FamilyInstance;
                        LocationPoint locationelem = elemCircuit.Location as LocationPoint;
                        XYZ XYZelem = locationelem.Point;
                        CircuitsElement.Add(XYZelem);

                        foreach (nodo node in floydWarshall.nodos)
                        {
                            if (j == 0)
                            {
                                nodeB.Location = node.Location;
                                nodeB.Name = node.Name;
                                j++;
                            }
                            else if (Math.Abs(XYZelem.Subtract(node.Location).GetLength()) < Math.Abs(XYZelem.Subtract(nodeB.Location).GetLength()))
                            {
                                nodeB.Location = node.Location;
                                nodeB.Name = node.Name;
                                j++;
                            }
                        }
                    }
                    floydWarshall.GetPath(nodeA.Name, nodeB.Name);

                    Transaction trans2 = new Transaction(doc);
                    trans2.Start("Lab");
                    circuito.SetCircuitPath(floydWarshall.organizePath(nodeA.Name, nodeB.Name, XYZPanel, CircuitsElement));
                    trans2.Commit();
                }
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
                //BuiltInCategory.OST_Conduit,
                //BuiltInCategory.OST_ConduitFitting,
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
            //b.Add(new ElementClassFilter(typeof(Conduit)));
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
        static ConnectorSet GetConnectors(Element e)
        {
            ConnectorSet connectors = null;

            if (e is FamilyInstance)
            {
                MEPModel m = ((FamilyInstance)e).MEPModel;

                if (null != m
                  && null != m.ConnectorManager)
                {
                    connectors = m.ConnectorManager.Connectors;
                }
            }
            else if (e is Wire)
            {
                connectors = ((Wire)e)
                  .ConnectorManager.Connectors;
            }
            else
            {
                Debug.Assert(e.GetType().IsSubclassOf(typeof(MEPCurve)),"expected all candidate connector provider " 
                    + "elements to be either family instances or "
                    + "derived from MEPCurve");

                if (e is MEPCurve)
                {
                    connectors = ((MEPCurve)e)
                      .ConnectorManager.Connectors;
                }
            }
            return connectors;
        }
        static List<XYZ[]> GetLines(FilteredElementCollector collectorCableTrayFitting)
        {
            List<XYZ[]> Aristas = new List<XYZ[]>();
            List<nodo> connectorsNodo = new List<nodo>();
            int i = 0;
            foreach (Element elem in collectorCableTrayFitting)
            {
                ConnectorSet conectores = GetConnectors(elem);

                i = 0;

                if (conectores.Size == 3)
                {
                    XYZ[] P = new XYZ[3];
                    XYZ Pcentro = null;
                    double[] L = new double[3];

                    foreach (Connector conector in conectores)
                    {
                        P[i] = conector.Origin;
                        i++;
                        var testWasTrue = false;
                        foreach (nodo connector1 in connectorsNodo)
                        {
                            if (conector.IsConnectedTo(connector1.Conector))
                            {
                                testWasTrue = true;
                                break;
                            }
                        }
                        if (testWasTrue == false)
                        {
                            connectorsNodo.Add(new nodo(0, null, conector));
                        }
                    }
                    L[0] = P[0].Subtract(P[1]).GetLength();
                    L[0] = Math.Abs(L[0]);
                    L[1] = P[1].Subtract(P[2]).GetLength();
                    L[1] = Math.Abs(L[1]);
                    L[2] = P[2].Subtract(P[0]).GetLength();
                    L[2] = Math.Abs(L[2]);

                    double Lmax = Math.Max(L[0], Math.Max(L[1], L[2]));

                    if (Lmax == L[0])
                    {
                        Pcentro = P[0].Subtract(P[1]).Divide(2).Add(P[1]);
                    }
                    if (Lmax == L[1])
                    {
                        Pcentro = P[1].Subtract(P[2]).Divide(2).Add(P[2]);
                    }
                    if (Lmax == L[2])
                    {
                        Pcentro = P[2].Subtract(P[0]).Divide(2).Add(P[0]);
                    }
                    foreach (Connector conector in conectores)
                    {
                        XYZ[] P1 = new XYZ[2];
                        P1[0] = conector.Origin;
                        P1[1] = Pcentro;
                        Aristas.Add(P1);
                        i++;
                    }

                }
                if (conectores.Size == 4)
                {
                    XYZ[] P = new XYZ[4];
                    foreach (Connector conector in conectores)
                    {
                        P[i] = conector.Origin;
                        i++;
                        var testWasTrue = false;
                        foreach (nodo connector1 in connectorsNodo)
                        {
                            if (conector.IsConnectedTo(connector1.Conector))
                            {
                                testWasTrue = true;
                                break;
                            }
                        }
                        if (testWasTrue == false)
                        {
                            connectorsNodo.Add(new nodo(0, null, conector));
                        }
                    }
                    XYZ Pcentro = P[0].Add(P[1]).Add(P[2]).Add(P[3]).Divide(4);
                    i = 0;
                    foreach (Connector conector in conectores)
                    {
                        XYZ[] P1 = new XYZ[2];
                        P1[0] = conector.Origin;
                        P1[1] = Pcentro;
                        Aristas.Add(P1);
                        i++;
                    }
                }
                if (conectores.Size == 2)
                {
                    XYZ[] P = new XYZ[2];
                    foreach (Connector conector in conectores)
                    {
                        P[i] = conector.Origin;
                        i++;

                        var testWasTrue = false;
                        foreach (nodo connector1 in connectorsNodo)
                        {
                            if (conector.IsConnectedTo(connector1.Conector))
                            {
                                testWasTrue = true;
                                break;
                            }
                        }
                        if (testWasTrue == false)
                        {
                            connectorsNodo.Add(new nodo(0, null, conector));
                        }
                    }
                    Aristas.Add(P);
                }
            }

            
            return Aristas;
        }
    }
}
