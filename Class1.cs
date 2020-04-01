﻿using System;
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
            //MessageBox.Show(floydWarshall.nodos.Count().ToString());
            //Get application and documnet objects
            UIApplication uiapp = commandData.Application;
            Document doc = uiapp.ActiveUIDocument.Document;
            
            try
            {
                //Init transaction
                Transaction trans2 = new Transaction(doc);
                trans2.Start("Lab");

                //Windos form
                Form1 ventana = new Form1(doc);
                ventana.Show();

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
    }
}