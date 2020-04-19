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
//A Simpler Dockable Panel Sample
//https://thebuildingcoder.typepad.com/blog/2013/05/a-simpler-dockable-panel-sample.html
//instalator -> Inno setup
//https://www.youtube.com/watch?v=1kkRPfWBnLw

namespace Revit_ManageElectricalCircuit
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //MessageBox.Show(floydWarshall.nodos.Count().ToString());
            //Get application and documnet objects
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;

            try
            {
                //Init transaction
                Transaction trans = new Transaction(doc);
                trans.Start("Lab");

                //Windos form
                Form1 ventana = new Form1(doc);
                ventana.ShowDialog();

                if (!ventana.Cancelclose)
                {
                    trans.Commit();
                }
                else
                {
                    trans.RollBack();
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
    }
}