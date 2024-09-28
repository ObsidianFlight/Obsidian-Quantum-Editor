using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using OpenTK.Graphics;
using System.Threading.Tasks;

namespace Sound_Space_Editor
{
    class Colorset
    {
        public static List<Color4>[] Colors = new List<Color4>[0];
        public static bool hasColorset;
        public static List<Layer> layers = new List<Layer>();

        public static Color4 currentColor = new Color4(255, 255, 255, 255);
        public static Color4 previousColor = new Color4(255, 255, 255, 255);

        public static Note TurnNoteWhite(Note note)
        {
            note.Color = Color4.White;
            return note;
        }
        public static int ColorAmount(int layerNumber)
        {
            int colorAmount;
            colorAmount = 0;
            string result = "";
            foreach(Layer layer in layers)
            {
                if(layer.LayerNumber == layerNumber)
                {
                    foreach (Alternate alternate in layer.Alternates)
                    {
                        if (colorAmount == 0)
                        {
                            colorAmount += alternate.Colors.Count;
                        }
                        else
                        {
                            colorAmount *= alternate.Colors.Count;
                        }
                        result += $"alternate {alternate.Colors.Count} ";
                    }
                    colorAmount *= layer.Alternates.Count;
                    result += $"layer {layer.Alternates.Count}";
                    //Console.WriteLine(result);
                    return colorAmount;
                }
            }
            return 0;
        }
        public static int ColorAmountLayer(Layer layer)
        {
            //Console.WriteLine("Color Amount Start!");
            int colorAmount;
            colorAmount = 0;
            string result = "";
            foreach (Alternate alternate in layer.Alternates)
            {
                if (colorAmount == 0)
                {
                    colorAmount += alternate.Colors.Count;
                }
                else
                {
                    colorAmount *= alternate.Colors.Count;
                }
                result += $"alternate {alternate.Colors.Count} ";
            }
            colorAmount *= layer.Alternates.Count;
            result += $"layer {layer.Alternates.Count}";
            //Console.WriteLine(result);
            //Console.WriteLine("Color Amount End!");
            return colorAmount;
        }
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }

            return value;
        }
        public static void LoadColorset(string value)
        {
            try
            {
                Console.WriteLine(value);

                hasColorset = true;
                string[] colorset = File.ReadAllLines(value);
                foreach(string lll in colorset)
                {
                    Console.WriteLine(lll);
                }
                EditorWindow.setColorsetFile(value);
                layers.Clear();
                try
                {

                    foreach (var line in colorset)
                    {
                        if (line.Contains('='))
                        {
                            var splits = line.Split('=');
                            if (splits.Length == 2)
                            {
                                var stringValue = splits[1].Trim();
                                var stringKey = splits[0].Trim();
                                try
                                {
                                    if (stringKey == "layers")
                                    {
                                        string[] stringOutput = stringValue.Split(',');
                                        for (int i = 0; i < stringOutput.Length; i++)
                                        {
                                            Console.WriteLine("Before Fade Split");
                                            string[] fadeOutput = stringOutput[i].Split('-');
                                            Console.WriteLine("Before Add Layer");
                                            layers.Add(new Layer(i, new List<Alternate>(), new List<Shift>(), Int32.Parse(fadeOutput[1])));
                                            Console.WriteLine("Before StringAlternates");
                                            string[] stringAlternates = fadeOutput[0].Split('%');
                                            Console.WriteLine("Before For Loop");
                                            for (int j = 0; j < stringAlternates.Length; j++)
                                            {
                                                Console.WriteLine("Before Add Alternate");
                                                AddAlternate(i, j+1);
                                                Console.WriteLine("Before Color Split");
                                                string[] stringColors = stringAlternates[j].Split('|');
                                                Console.WriteLine("Before For Loop 2");
                                                for (int l = 0; l < stringColors.Length; l++)
                                                {
                                                    try
                                                    {
                                                        AddColor(i, j+1, l+1, HexColor.HextoColor4(stringColors[l]));
                                                    }
                                                    catch (Exception e) { Console.WriteLine("Failed to load colorset, Depth 3-1-1, " + e); }
                                                    Console.WriteLine($"{i}, {j}, {l}, {HexColor.HextoColor4(stringColors[l]).ToString()}");
                                                }
                                            }
                                            Console.WriteLine("End of loading layers");
                                        }
                                    }
                                }
                                catch (Exception e) { Console.WriteLine("Failed to load colorset, Depth 3-1, " + e); }
                                try
                                {
                                    if (stringKey == "notes")
                                    {
                                        string[] stringOutput = stringValue.Split('|');
                                        for (int i = 0; i < stringOutput.Length; i++)
                                        {
                                            string[] stringNotesOutput = stringOutput[i].Split(',');
                                            EditorWindow.Instance.Notes._notes[i].Color = HexColor.HextoColor4("#" + stringNotesOutput[0]);
                                            EditorWindow.Instance.Notes._notes[i].layer = Int32.Parse(stringNotesOutput[1]);
                                            EditorWindow.Instance.Notes._notes[i].shift = Int32.Parse(stringNotesOutput[2]);
                                        }
                                    }
                                }
                                catch (Exception e) { Console.WriteLine("Failed to load colorset, Depth 3-2, " + e); }
                            }
                        }
                    }
                    VerifyNotes();
                }
                catch (Exception e) { Console.WriteLine("Failed to load colorset, Depth 2, " + e); }
            } 
            catch (Exception e)
            {
                Console.WriteLine("Failed to load colorset, Depth 1, " + e);
                hasColorset = false;
            }
        }

        public static void AddLayer(int layerNumber)
        {
            Console.WriteLine($"Start of AddLayer, {layerNumber}");
            bool alreadyExists = false;
            Console.WriteLine("Before foreach layer");
            foreach(var layer in layers)
            {
                if(layer.LayerNumber == layerNumber)
                {
                    Console.WriteLine("Layer Already Exists");
                    alreadyExists = true;
                }
            }
            if (!alreadyExists)
            {
                Console.WriteLine("Layer does not exist, adding layer...");
                layers.Add(new Layer(layerNumber, new List<Alternate>(), new List<Shift>(), 0));
                Console.WriteLine("Assigning Default Colors");
                DefaultColors();
            }
            Console.WriteLine("End of AddLayer");
        }
        
        public static void AddAlternate(int layerNumber, int alternateNumber)
        {
            Console.WriteLine($"Start of Add Alternate, {layerNumber}, {alternateNumber}");
            bool alreadyExists = false;
            foreach (var layer in layers)
            {
                if(layer.LayerNumber == layerNumber)
                {
                    try
                    {
                        layer.Alternates.Insert(alternateNumber - 1, new Alternate(alternateNumber, new List<Color4>()));
                    }
                    catch
                    {
                        Console.WriteLine("Alternate too far away to insert!");
                        layer.Alternates.Add(new Alternate(layer.Alternates.Count, new List<Color4>()));
                    }
                    Console.WriteLine("Before Sort Alternate");
                    Layer.SortAlternate(layer.Alternates);
                    Console.WriteLine("After Sort Alternate");
                    break;
                }
            }
            Console.WriteLine("End of Add Alternate");
        }

        public static void AddColor(int layerNumber, int alternateNumber, int colorNumber, Color4 theColor)
        {
            Console.WriteLine("Start of Add Color");
            foreach (var layer in layers)
            {
                if (layer.LayerNumber == layerNumber)
                {
                    foreach (var alternates in layer.Alternates)
                    {
                        if(alternates.AlternateNumber == alternateNumber)
                        {
                            try
                            {
                                alternates.Colors.Insert(colorNumber - 1, theColor);
                            }
                            catch
                            {
                                Console.WriteLine("Color too far away to insert!");
                                alternates.Colors.Add(theColor);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("End of Add Color");
        }

        public static void ChangeColor(int layerNumber, int alternateNumber, int colorNumber)
        {
            foreach (var layer in layers)
            {
                if (layer.LayerNumber == layerNumber)
                {
                    foreach (var alternates in layer.Alternates)
                    {
                        if (alternates.AlternateNumber == alternateNumber)
                        {
                            try
                            {
                                alternates.Colors[colorNumber-1] = currentColor;
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        public static void DeleteAlternate(int layerNumber, int alternateNumber)
        {
            foreach (var layer in layers)
            {
                if (layer.LayerNumber == layerNumber)
                {
                    foreach (var alternates in layer.Alternates)
                    {
                        if (alternates.AlternateNumber == alternateNumber)
                        {
                            layer.Alternates.Remove(alternates);
                            Layer.SortAlternate(layer.Alternates);
                            return;
                        }
                    }
                }
            }
            DefaultColors();
        }

        public static void DeleteColor(int layerNumber, int alternateNumber, int colorNumber)
        {
            foreach (var layer in layers)
            {
                if (layer.LayerNumber == layerNumber)
                {
                    foreach (var alternates in layer.Alternates)
                    {
                        if (alternates.AlternateNumber == alternateNumber)
                        {
                            try
                            {
                                alternates.Colors.RemoveAt(colorNumber - 1);
                            }
                            catch { }
                            if(alternates.Colors.Count == 0)
                            {
                                layer.Alternates.Remove(alternates);
                                foreach(var alternates2 in layer.Alternates)
                                {
                                    if(alternates2 != alternates)
                                    {
                                        alternates2.AlternateNumber = alternates2.AlternateNumber - 1;
                                    }
                                }
                            }
                            if(layer.Alternates.Count == 0)
                            {
                                LayerViewer.removeLayer(layer.LayerNumber);
                            }
                            return;
                        }
                    }
                }
            }
            DefaultColors();
        }

        public static void DefaultColors()
        {
            //Console.WriteLine("Start of Default Colors");
            if(layers.Count == 0)
            {
                //Console.WriteLine("Adding Layer Zero");
                AddLayer(0);
            }
            //Console.WriteLine("Before Foreach in Default Colors");
            foreach(Layer layer in layers)
            {
                if (layer.Alternates.Count == 0)
                {
                    //Console.WriteLine("Adding First Alternate");
                    AddAlternate(layer.LayerNumber, 0);
                }
                //Console.WriteLine("Before Second Foreach in Default Colors");
                foreach (Alternate alternate in layer.Alternates)
                {
                    if(alternate.Colors.Count == 0)
                    {
                        //Console.WriteLine("Adding First Color");
                        AddColor(layer.LayerNumber, alternate.AlternateNumber, 0, currentColor);
                    }
                }
            }
            //Console.WriteLine("End of Default Colors");
        }

        public static List<Color4> ColorList(int theLayer)
        {
            List<Color4> theList = new List<Color4>();
            foreach (Layer layer in layers)
            {
                try
                {
                    if (layer.LayerNumber == theLayer)
                    {
                        int colorAmount = 0;
                        colorAmount = ColorAmount(layer.LayerNumber);
                        for (int i = 0; i < colorAmount; i++)
                        {
                            int alternateNumber = i % layer.Alternates.Count;
                            int colorNumber = i / layer.Alternates.Count % layer.Alternates[alternateNumber].Colors.Count;
                            theList.Add(layer.Alternates[alternateNumber].Colors[colorNumber]);
                        }
                    }
                }
                catch { Console.WriteLine("UpdateColorDisplay Failed"); }
            }
            return theList;
        }

        public static List<Color4> TrueColorList(Layer layer)
        {
            List<Color4> theList = new List<Color4>();
            Layer theLayer = FadeColorList(layer);
            //Console.WriteLine($"True Color Start!");
            //Console.WriteLine(theLayer.LayerNumber);
            foreach(var alternate in theLayer.Alternates)
            {
                //Console.WriteLine("Alternate");
                foreach(var color in alternate.Colors)
                {
                    //Console.WriteLine($"{color.R} {color.G} {color.B}");
                }
            }
            try
            {
                int colorAmount = 0;
                colorAmount = ColorAmountLayer(theLayer);
                //Console.WriteLine($"Color Amount: {colorAmount}");
                for (int i = 0; i < colorAmount; i++)
                {
                    int alternateNumber = i % theLayer.Alternates.Count;
                    //Console.WriteLine($"Current Layer: {layer.LayerNumber}");
                    //Console.WriteLine($"Layer Alternate Amount: {theLayer.Alternates.Count}");
                    //Console.WriteLine($"Colors in Current Alternate: {theLayer.Alternates[alternateNumber].Colors.Count}");
                    //Console.WriteLine($"Int I = {i}");
                    //Console.WriteLine($"Current Alternate: {alternateNumber}");
                    int colorNumber = 0;
                    if(theLayer.Alternates.Count == 1)
                    {
                        colorNumber = i % theLayer.Alternates[0].Colors.Count;
                    }
                    else
                    {
                        colorNumber = ((int)Math.Floor((double)i / theLayer.Alternates.Count)) % theLayer.Alternates[alternateNumber].Colors.Count;
                    }
                    //Console.WriteLine(colorNumber);
                    //Console.WriteLine("");
                    theList.Add(theLayer.Alternates[alternateNumber].Colors[colorNumber]);
                }
            }
            catch { }
            //Console.WriteLine("True Color End!");
            return theList;
        }

        public static Layer FadeColorList(Layer layer)
        {
            //Console.WriteLine("Fade Color List Start!");
            Layer newLayer = new Layer(layer.LayerNumber, new List<Alternate>(), layer.Shift, layer.Fade);
            foreach (Alternate alternate in layer.Alternates)
            {
                Alternate newalternate = new Alternate(alternate.AlternateNumber, LayerZero.FadeColor(alternate, layer.Fade));
                newLayer.Alternates.Add(newalternate);
            }
            //Console.WriteLine("Fade Color List End!");
            return newLayer;
        }

        public static void AssignNotes(List<Note> notes, int layer, int shiftLevel)
        {
            if(hasColorset == false)
            {
                return;
            }
            foreach (Note note in notes)
            {
                if (layers.Count >= layer)
                {
                    note.layer = layer;
                }
                note.shift = shiftLevel;
            }
            if (layer == 0)
            {
                //Console.WriteLine("Layer 0!");
                int i = 0;
                Colors[0] = TrueColorList(layers[0]);
                //Console.WriteLine($"Layer0set Color: {Colors[0].Count}");
                //Console.WriteLine($"{layers[0].Alternates[0].Colors[0].R}, {layers[0].Alternates[0].Colors[0].G}, {layers[0].Alternates[0].Colors[0].B}");
                foreach (Note note in notes)
                {
                    try 
                    {
                        note.Color = Colors[0][i % Colors[0].Count];
                        i++;
                    }
                    catch { }
                }
            }
            VerifyNotes();
        }

        public static void VerifyNotes()
        {
            foreach(Layer layer in layers)
            {
                layer.Shift.Clear();
                for(int i = 0; i < EditorWindow.Instance.Notes.Count; i++)
                {
                    if (EditorWindow.Instance.Notes[i].layer == layer.LayerNumber)
                    {
                        layer.Shift.Add(new Shift(EditorWindow.Instance.Notes[i], EditorWindow.Instance.Notes[i].shift));
                    }
                }
            }
            SetupColorset();
        }

        public static void SetupColorset()
        {
            //Console.WriteLine("Setting Up Colorset!");
            for (int i = 0; i < Colors.Length; i++)
            {
                Colors[i].Clear();
            }
            List<Note> noteList = EditorWindow.Instance.Notes._notes;
            int[] layerindex = new int[layers.Count];
            List<Color4>[] trueColorList = new List<Color4>[layers.Count];
            for(int i = 0; i < layers.Count; i++)
            {
                layerindex[i] = 0;
                trueColorList[i] = TrueColorList(layers[i]);
            }
            Colors = trueColorList;
            //Console.WriteLine("Finished Setting Up Colorset!");
            EditorWindow.Instance.Notes.Sort();
        }
    }

    class Layer
    {
        public int LayerNumber;
        public List<Alternate> Alternates;
        public List<Shift> Shift;
        public int Fade;
        public Layer(int layerNumber, List<Alternate> alternates, List<Shift> shift, int fade)
        {
            LayerNumber = layerNumber;
            Alternates = alternates;
            Shift = shift;
            Fade = fade;
        }

        public static List<Alternate> SortAlternate(List<Alternate> alternates)
        {
            int index = 1;
            foreach (var alternate in alternates)
            {
                alternate.AlternateNumber = index;
                index++;
            }
            return alternates;
        }
    }

    class Alternate
    {
        public int AlternateNumber;
        public List<Color4> Colors;

        public Alternate(int alternateNumber, List<Color4> colors)
        {
            AlternateNumber = alternateNumber;
            Colors = colors;
        }
    }

    class Shift
    {
        public Note Note;
        public int ShiftLevel;

        public Shift(Note note, int shiftLevel)
        {
            Note = note;
            ShiftLevel = shiftLevel;
        }
    }

    class LayerZero
    {
        //This class used to have more in it, just think of it as a funny wrapper
        //Because I ended up scrapping a lot of the stuff I made for it, but this thing needed to stay since it's like a quarter of the program.
        public static List<Color4> FadeColor(Alternate alternate, int divisor)
        {
            //Console.WriteLine("Fade Color Start!");
            List<string> hexOutput = new List<string>();
            List<Color4> color4Output = new List<Color4>();
            for (int i = 0; i < alternate.Colors.Count; i++)
            {
                float[] color1 = { 0, 0, 0 };
                float[] color2 = { 0, 0, 0 };

                if (i == alternate.Colors.Count - 1)
                {
                    color1[0] = alternate.Colors[0].R * 255;
                    color1[1] = alternate.Colors[0].G * 255;
                    color1[2] = alternate.Colors[0].B * 255;
                    color2[0] = alternate.Colors[i].R * 255;
                    color2[1] = alternate.Colors[i].G * 255;
                    color2[2] = alternate.Colors[i].B * 255;
                    int Ired = (int)color2[0];
                    int Igreen = (int)color2[1];
                    int Iblue = (int)color2[2];
                    string Hred = Ired.ToString("X2");
                    string Hgreen = Igreen.ToString("X2");
                    string Hblue = Iblue.ToString("X2");
                }
                else
                {
                    color1[0] = alternate.Colors[i + 1].R * 255;
                    color1[1] = alternate.Colors[i + 1].G * 255;
                    color1[2] = alternate.Colors[i + 1].B * 255;
                    color2[0] = alternate.Colors[i].R * 255;
                    color2[1] = alternate.Colors[i].G * 255;
                    color2[2] = alternate.Colors[i].B * 255;
                }
                //Console.WriteLine(divisor);
                //Console.WriteLine($"{color1[0]}, {color1[1]}, {color1[2]}. {color2[0]}, {color2[1]}, {color2[2]}");
                for (int j = 0; j <= divisor; j++)
                {
                    double Ored = 0;
                    double Ogreen = 0;
                    double Oblue = 0;
                    if (color1[0] - color2[0] > 0) { Ored = ((color1[0] - color2[0]) / (divisor + 1) * j) + color2[0]; }
                    else if (color1[0] - color2[0] < 0) { Ored = ((color1[0] - color2[0]) / (divisor + 1) * j) + color2[0]; }
                    else {Ored = color1[0]; }

                    if (color1[1] - color2[1] > 0) { Ogreen = ((color1[1] - color2[1]) / (divisor + 1) * j) + color2[1]; }
                    else if (color1[1] - color2[1] < 0) { Ogreen = ((color1[1] - color2[1]) / (divisor + 1) * j) + color2[1]; }
                    else { Ogreen = color1[1]; }

                    if (color1[2] - color2[2] > 0) { Oblue = ((color1[2] - color2[2]) / (divisor + 1) * j) + color2[2]; }
                    else if (color1[2] - color2[2] < 0) { Oblue = ((color1[2] - color2[2]) / (divisor + 1) * j) + color2[2]; }
                    else { Oblue = color1[2]; }
                    int Ired = (int)Ored;
                    int Igreen = (int)Ogreen;
                    int Iblue = (int)Oblue;
                    string Hred = Ired.ToString("X2");
                    string Hgreen = Igreen.ToString("X2");
                    string Hblue = Iblue.ToString("X2");
                    //Console.WriteLine(j);
                    //Console.WriteLine($"{Ored}, {Ogreen}, {Oblue}");
                    //Console.WriteLine($"{Ired}, {Igreen}, {Iblue}");
                    //Console.WriteLine("#" + Hred + Hgreen + Hblue);
                    hexOutput.Add("#" + Hred + Hgreen + Hblue);
                }
            }
            foreach (string hexNumber in hexOutput)
            {
                color4Output.Add(HexColor.HextoColor4(hexNumber));
            }
            //Console.WriteLine("Fade Color End!");
            return color4Output;
        }
    }
    
    class LZNote
    {
        public Note Note;
        public Color4 Color;

        public LZNote(Note note, Color4 color4)
        {
            Note = note;
            Color = color4;
        }
    }

    class HexColor
    {
        public static string Color4toHex(Color4 co)
        {
            byte r = (byte)(co.R * 255);
            byte g = (byte)(co.G * 255);
            byte b = (byte)(co.B * 255);
            string hexR = r.ToString("X2");
            string hexG = g.ToString("X2");
            string hexB = b.ToString("X2");
            return hexR+hexG+hexB;
        }

        public static Color4 HextoColor4(string co)
        {
            try
            {
                int intA = int.Parse("FF", System.Globalization.NumberStyles.HexNumber);
                int intR = int.Parse(co.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
                int intG = int.Parse(co.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
                int intB = int.Parse(co.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
                float A = (float)intA / 255;
                float R = (float)intR / 255;
                float G = (float)intG / 255;
                float B = (float)intB / 255;
                Color4 color = new Color4(R, G, B, A);
                return color;
            }
            catch(ArgumentException e)
            {
                Console.WriteLine($"Hex to Color4 has failed! {e.Message}");
            }
            return Color4.White;
        }
    }
}
