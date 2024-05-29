using System;
using System.Linq;
using System.Windows.Forms;
using Sound_Space_Editor.Gui;
using OpenTK;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace Sound_Space_Editor
{
    public partial class LayerViewer : Form
    {
        public static LayerViewer inst;
        private CultureInfo culture;
        public LayerViewer()
        {
            inst = this;
            InitializeComponent();
            ResetList();
            culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
        }

        public void OrderList()
        {
            try 
            {
                Colorset.layers = Colorset.layers.OrderBy(o => o.LayerNumber).ToList();
            }
            catch { }
        }
        public void ResetList()
        {
            try
            {
                LayerList.Rows.Clear();
                int i = 0;
                foreach (var layer in Colorset.layers)
                {
                    int colorAmount = 0;
                    layer.LayerNumber = i;
                    foreach (var alternate in layer.Alternates)
                    {
                        colorAmount += alternate.Colors.Count;
                    }
                    LayerList.Rows.Add(layer.LayerNumber, layer.Alternates.Count, colorAmount, layer.Shift.Count, layer.Fade);
                    i++;
                }
                Colorset.VerifyNotes();
            }
            catch { }
        }

        private void LayerList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void LayerList_SelectionChanged(object sender, EventArgs e)
        {
            Console.WriteLine("Layer List Selection Changed!");
            foreach (DataGridViewRow row in LayerList.SelectedRows)
            {
                int index = row.Index;
                Layer selectedLayer = new Layer(-1, new List<Alternate>(), new List<Shift>(), 0);
                for (int i = 0; i < Colorset.layers.Count; i++)
                {
                    LayerBox.Value = Colorset.layers[i].LayerNumber;
                    FadeAmountBox.Value = Colorset.layers[i].Fade;
                }
            }
            //This section of code never gets activated, maybe it's because I wrote the whole function manually
            //Instead of trying to make the form auto-generate the function, I give up here.
        }

        private void SendNotesButton_Click(object sender, EventArgs e)
        {
            if (LayerBox.Value > 0 && SendNotesBox.Value >= 0)
            {
                Layer from = new Layer(-1, new List<Alternate>(), new List<Shift>(), 0);
                Layer to = new Layer(-1, new List<Alternate>(), new List<Shift>(), 0);
                bool conf1 = false;
                bool conf2 = false;
                foreach (Layer layer in Colorset.layers)
                {
                    if (layer.LayerNumber == LayerBox.Value)
                    {
                        conf1 = true;
                        from = layer;
                    }
                    if (layer.LayerNumber == SendNotesBox.Value)
                    {
                        conf2 = true;
                        to = layer;
                    }
                }
                if(conf1 == true && conf2 == true)
                {
                    SendNotesToLayer(from.LayerNumber, to.LayerNumber, KeepShift.Checked);
                }

                bool ex1 = false;
                bool ex2 = false;
                int la1 = 0;
                int la2 = 0;
                foreach (var layer in Colorset.layers)
                {
                    if (LayerBox.Value == layer.LayerNumber)
                    {
                        ex1 = true;
                        la1 = (int)LayerBox.Value;
                    }
                    if (SendNotesBox.Value == layer.LayerNumber)
                    {
                        ex2 = true;
                        la2 = (int)SendNotesBox.Value;
                    }
                }
                if (ex1 == true && ex2 == true)
                {
                    foreach (var note in EditorWindow.Instance.Notes._notes)
                    {
                        if (note.layer == la1)
                        {
                            note.layer = la2;
                        }
                    }
                }
            }
            Colorset.VerifyNotes();
            ResetList();
        }

        private void AddLayerButton_Click(object sender, EventArgs e)
        {
            if (LayerBox.Value > 0)
            {
                var exists = false;
                foreach (var layer in Colorset.layers)
                {
                    if (layer.LayerNumber == LayerBox.Value)
                        exists = true;
                }

                if (!exists)
                {
                    Colorset.AddLayer((int)LayerBox.Value);

                    OrderList();

                    ResetList();
                }
            }
        }

        private void RemoveLayerButton_Click(object sender, EventArgs e)
        {
            if (LayerBox.Value > 0)
            {
                bool reset = false;
                foreach (Layer layer in Colorset.layers)
                {
                    if (layer.LayerNumber == LayerBox.Value)
                    {
                        Colorset.layers.Remove(layer);
                        reset = true;
                    }
                    if (reset == true)
                    {
                        Colorset.DefaultColors();
                        OrderList();
                        ResetList();
                        return;
                    }
                }
            }
        }

        private void SendNotesToLayer(int fromLayer, int toLayer, bool keepShift)
        {
            bool reset = false;
            foreach(Layer layer in Colorset.layers)
            {
                if(fromLayer == layer.LayerNumber)
                {
                    foreach(Layer secondLayer in Colorset.layers)
                    {
                        if(toLayer == secondLayer.LayerNumber)
                        {
                            foreach (var note in layer.Shift)
                            {
                                if (keepShift)
                                {
                                    secondLayer.Shift.Add(note);
                                }
                                else
                                {
                                    secondLayer.Shift.Add(new Shift(note.Note, 0));
                                }
                            }
                            reset = true;
                        }
                    }
                    if (reset == true)
                    {
                        layer.Shift.Clear();
                    }
                }
            }
            OrderList();
            ResetList();
        }

        private void FadeButton_Click(object sender, EventArgs e)
        {
            foreach (Layer layer in Colorset.layers)
            {
                if (layer.LayerNumber == LayerBox.Value)
                {
                    layer.Fade = (int)FadeAmountBox.Value;
                }
            }
            OrderList();
            ResetList();
        }
    }
}
