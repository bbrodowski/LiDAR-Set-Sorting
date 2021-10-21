using PPL_Lib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

///----------------------------------------------------------------------------
/// This example plugin demonstrates
///   1) Adding a menu item to the main O-Calc Pro menu
///   2) Enabling and disabling that menu item
///   3) Performing a task when the item is clicked
///----------------------------------------------------------------------------

namespace OCalcProPlugin
{
    public class Plugin : PPLPluginInterface
    {

        /// <summary>
        /// THis is the handle to the main O-Calc Pro component
        /// </summary>
        PPL_Lib.PPLMain cPPLMain = null;

        /// <summary>
        /// Declare the type of plugin as one of:
        ///         DOCKED_TAB
        ///         MENU_ITEM
        ///         BOTH_DOCKED_AND_MENU
        ///         CLEARANCE_SAG_PROVIDER
        /// </summary>
        public PLUGIN_TYPE Type
        {
            get
            {
                return PLUGIN_TYPE.MENU_ITEM;
            }
        }

        /// <summary>
        /// Declare the name of the plugin usd for synthesizing the registry keys ect
        /// </summary>
        public String Name
        {
            get
            {
                return "LidarSetSort";
            }
        }

        /// <summary>
        /// Optionally declare a description string (defaults to the name);
        /// </summary>
        public String Description
        {
            get
            {
                return Name;
            }
        }

        /// <summary>
        /// Add a tabbed form to the tabbed window (if the plugin type is 
        /// PLUGIN_TYPE.DOCKED_TAB 
        /// or
        /// PLUGIN_TYPE.BOTH_DOCKED_AND_MENU
        /// </summary>
        /// <param name="pPPLMain"></param>
        public void AddForm(PPL_Lib.PPLMain pPPLMain) { }

        /// <summary>
        /// Perform clearance analysis if type is PLUGIN_TYPE.CLEARANCE_SAG_PROVIDER
        /// </summary>
        /// <param name="pMain"></param>
        /// <returns></returns>
        public PPLClearance.ClearanceSagProvider GetClearanceSagProvider(PPL_Lib.PPLMain pMain)
        {
            System.Diagnostics.Debug.Assert(Type == PLUGIN_TYPE.CLEARANCE_SAG_PROVIDER, Name + " is not a clearance provider plugin.");
            return null;
        }

        bool hasMenu = false;

        FlowLayoutPanel flowLayoutPanel = null;

        public void AddToMenu(PPL_Lib.PPLMain pPPLMain, System.Windows.Forms.ToolStrip pToolStrip)
        {
            pPPLMain.BeforeEvent += (object sender, PPL_Lib.PPLMain.EVENT_TYPE pEventType, ref bool pAbortEvent) =>
            {
                if (flowLayoutPanel is null)
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        if (form.Text == "OV Overlay")
                        {
                            var overlayControl = (OV_Overlay.OverlayControl)form.Controls[0];
                            var tabControl = (TabControl)overlayControl.Controls[5];
                            var layers = (TabPage)tabControl.TabPages[0];
                            var splitContainer = (SplitContainer)layers.Controls[0];
                            var spliterpanel2 = (SplitterPanel)splitContainer.Panel2;
                            flowLayoutPanel = (FlowLayoutPanel)spliterpanel2.Controls[0];

                            foreach (Control control in flowLayoutPanel.Controls)
                            {
                                if (control.GetType() == typeof(ToolStrip))
                                {
                                    hasMenu = true;
                                }
                            }

                            if (hasMenu is false)
                            {
                                ToolStrip toolStrip = new ToolStrip();
                                ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Sort");

                                toolStrip.Dock = DockStyle.Top;
                                toolStrip.Items.Add(toolStripMenuItem);
                                toolStrip.Margin = new Padding(10);

                                toolStripMenuItem.Click += (Object lul, EventArgs e) => 
                                {
                                    var controls = flowLayoutPanel.Controls.OfType<CheckBox>().ToList();

                                    controls.Sort((c1, c2) => c1.Text.CompareTo(c2.Text));

                                    flowLayoutPanel.Controls.Clear();

                                    foreach (var item in controls)
                                    {
                                        flowLayoutPanel.Controls.Add(item);
                                    }
                                };

                                flowLayoutPanel.Dock = DockStyle.Fill;

                                spliterpanel2.Controls.Add(toolStrip);
                            }
                        }
                    }
                }
            };
        }
    }
}
