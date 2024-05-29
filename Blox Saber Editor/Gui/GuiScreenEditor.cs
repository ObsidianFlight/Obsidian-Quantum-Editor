﻿using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using Sound_Space_Editor.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace Sound_Space_Editor.Gui
{
    class GuiScreenEditor : GuiScreen
	{
		public GuiScreen GuiScreen { get; private set; }

		private List<GuiTextBox> Boxes = new List<GuiTextBox>();

		public readonly GuiGrid Grid = new GuiGrid(300, 300);
		public readonly GuiTrack Track = new GuiTrack(0, 80);
		public readonly GuiSlider Tempo;
		public readonly GuiSlider MasterVolume;
		public readonly GuiSlider SfxVolume;
		public readonly GuiSlider BeatSnapDivisor;
		public readonly GuiSliderTimeline Timeline;
		public readonly GuiSlider NoteAlign;
		public readonly GuiTextBox Offset;
		public readonly GuiTextBox SfxOffset;
		public readonly GuiTextBox JumpMSBox;
		public readonly GuiTextBox RotateBox;
		public readonly GuiTextBox BezierBox;
		public readonly GuiCheckBox Autoplay;
		public readonly GuiCheckBox ApproachSquares;
		public readonly GuiCheckBox GridNumbers;
		public readonly GuiCheckBox GridLetters;
		public readonly GuiCheckBox Quantum;
		public readonly GuiCheckBox Numpad;
		public readonly GuiCheckBox AutoAdvance;
		public readonly GuiCheckBox QuantumGridLines;
		public readonly GuiCheckBox QuantumGridSnap;
		public readonly GuiCheckBox Metronome;
		//public readonly GuiCheckBox DynamicBezier;
		public readonly GuiCheckBox CurveBezier;
		//public readonly GuiCheckBox ClickToPlace;
		public readonly GuiCheckBox SeparateClickTools;
		//public readonly GuiCheckBox LegacyBPM;

		public readonly GuiButton BackButton;
		public readonly GuiButton CopyButton;
        public readonly GuiButton PlayButton;

        public static GuiLabel playLabel = new GuiLabel(0, 0, "If you hold SHIFT, the map will be played from the current time", "main", 15);
        public static GuiLabel noplayLabel = new GuiLabel(0, 0, "Not available, because the map is using a ROBLOX audio.", "main", 15);

        public readonly GuiButton JumpMSButton;
		public readonly GuiButton RotateButton;
		public readonly GuiButton BezierButton;
		public readonly GuiButton BezierStoreButton;
		public readonly GuiButton BezierClearButton;

		public readonly GuiButton OpenTimings;
		public readonly GuiButton OpenBookmarks;
		public readonly GuiButton UseCurrentMs;

		public readonly GuiButton HFlip;
		public readonly GuiButton VFlip;

		public readonly GuiButton CreateColorset;
		public readonly GuiButton OpenColorset;
		public readonly GuiButton ExportColorset; //New method to export txt file when ready.
		public readonly GuiButton VisualizeColors; //Button lets you see all the Alternates in the current layer, toggleable.
		public readonly GuiButton ManageLayers; //Lets you copy/paste layers, add layers, and delete layers.
		public readonly GuiTextBox LayerPicker; //Chooses which layer to add colors/alternates on. Any new notes placed will be set to the layer this is on.
		public readonly GuiLabel LayerWarning; //Displays a warning "Layer does not exist" when setting to a layer that doesn't exist
											   //For example if you only have 4 layers but you pick layer 5, or if you pick layer "sadasda" cause you accidently clicked on the box.
		public readonly GuiTextBox AlternatePicker; //Picks which Alternate to insert/change color on.
		public readonly GuiTextBox ColorPicker; //Needs better name, for picking where to insert/change
		public readonly GuiTextBox ColorR;
		public readonly GuiTextBox ColorG;
		public readonly GuiTextBox ColorB;
		public readonly GuiSlider ColorRSlider;
		public readonly GuiSlider ColorGSlider;
		public readonly GuiSlider ColorBSlider;
		public readonly GuiButton AddAlternate; //Creates new alternate and sets AlternatePicker to that Alternate, default is whatever AddColor would give.
		public readonly GuiButton AddColor; //Adds a new color in a slot depending on ColorPicker
		public readonly GuiButton ChangeColor; //Changes the color that ColorPicker is selecting
		public readonly GuiButton DeleteColor; //Removes the color in the slot that ColorPicker is on.
		public readonly GuiButton DeleteAlternate; //Removes the Alternate and all colors in it.
		public readonly GuiButton ConfirmDelete; //Will display on the other side of the screen, used for confirm delete of the last pressed Delete Button
												 //Only required on actual layers.
		public readonly GuiTextBox ColorHex; //Displays the current color in hex, if entered manually will translate the hex to the slider and textboxes
		public readonly GuiButton SetNotes; //Sets any selected notes to the layer in LayerPicker
											//If layer 0 is selected, sets to that layer and gives them whatever color is in the layer.
		public readonly GuiButton SetColor; //Sets 1st Color display to the current color
		public readonly GuiButton SetComparison; //Sets 2nd Color display to the current color
		public readonly GuiButton ReverseSelection; //Button lets you select colors from textboxes instead of sliders.

		public readonly GuiTextBox ShiftLevel;
		public readonly GuiButton ShiftDefault;
		public readonly GuiButton ApplyShift;

		public readonly GuiButton OptionsNav;
		public readonly GuiButton TimingNav;
		public readonly GuiButton PatternsNav;
		public readonly GuiButton ColorsNav;

		public readonly GuiTextBox ScaleBox;
		public readonly GuiButton ScaleButton;

		public readonly GuiSlider TrackHeight;
		public readonly GuiSlider TrackCursorPos;
		public readonly GuiSlider ApproachRate;

		public readonly GuiTextBox MSBoundLower;
		public readonly GuiTextBox MSBoundHigher;
		public readonly GuiButton SelectBound;

		private bool OptionsNavEnabled = false;
		private bool TimingNavEnabled = false;
		private bool PatternsNavEnabled = false;
		private bool ColorsNavEnabled = false;
		private bool ColorsDisplayEnabled = false;
		int delete = 0;

		public float AutoSaveTimer { get; private set; } = 0f;

		private readonly GuiLabel _toast;
		private float _toastTime;
		private readonly int _textureId;
		private bool bgImg = false;

		public List<Note> beziernodes;

		//private object TimingPanel;
		//TimingPoints TimingPoints;

		public BigInteger FactorialApprox(int k)
        {
			var result = new BigInteger(1);
			if (k < 10)
            {
				for (int i = 1; i <= k; i++)
                {
					result *= i;
                }
            }
			else
            {
				result = (BigInteger)(Math.Sqrt(2 * Math.PI * k) * Math.Pow(k / Math.E, k));
			}
			return result;
		}

		public BigInteger BinomialCoefficient(int k, int v)
        {
			var bic = FactorialApprox(k) / (FactorialApprox(v) * FactorialApprox(k - v));
			return bic;
		}

		public GuiScreenEditor() : base(0, EditorWindow.Instance.ClientSize.Height - 64, EditorWindow.Instance.ClientSize.Width - 512 - 64, 64)
		{
			if (File.Exists(Path.Combine(EditorWindow.Instance.LauncherDir, "background_editor.png")))
			{
				this.bgImg = true;
				using (Bitmap img = new Bitmap(Path.Combine(EditorWindow.Instance.LauncherDir, "background_editor.png")))
				{
					this._textureId = TextureManager.GetOrRegister("bg", img, true);
				}
			}

			_toast = new GuiLabel(0, 0, "", false)
			{
				Centered = true,
				FontSize = 36
			};

			LayerWarning = new GuiLabel(0, 0, "", false)
			{
				Centered = true,
				FontSize = 24
			};

			var playPause = new GuiButtonPlayPause(0, EditorWindow.Instance.ClientSize.Width - 512 - 64, EditorWindow.Instance.ClientSize.Height - 64, 64, 64);
			Offset = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false
			};
			JumpMSBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			MSBoundLower = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			MSBoundHigher = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			RotateBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "90",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			BezierBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "4",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			SfxOffset = new GuiTextBox(EditorWindow.Instance.ClientSize.Width - 128, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = true
			};
			NoteAlign = new GuiSlider(0, 0, 256, 40)
			{
				MaxValue = 59,
                Value = 2
			};
			BeatSnapDivisor = new GuiSlider(0, 0, 256, 40);
			Timeline = new GuiSliderTimeline(0, 0, EditorWindow.Instance.ClientSize.Width, 64);
			Tempo = new GuiSlider(0, 0, 512, 64)
			{
				MaxValue = 26,
				Value = 16
			};

			Timeline.Snap = false;
			BeatSnapDivisor.Value = GuiTrack.BeatDivisor - 1;
			BeatSnapDivisor.MaxValue = 31;

			MasterVolume = new GuiSlider(0, 0, 40, 256)
			{
				MaxValue = 50
			};
			SfxVolume = new GuiSlider(0, 0, 40, 256)
			{
				MaxValue = 50
			};

			TrackHeight = new GuiSlider(0, 0, 32, 128)
			{
				MaxValue = 32,
				Value = 16,
			};

			TrackCursorPos = new GuiSlider(0, 0, 200, 32)
			{
				MaxValue = 100,
				Value = 40,
			};

			ApproachRate = new GuiSlider(0, 0, 32, 128)
			{
				MaxValue = 29,
				Value = 9,
			};

			BackButton = new GuiButton(3, 0, 0, Grid.ClientRectangle.Width + 1, 42, "BACK TO MENU", false);
			CopyButton = new GuiButton(4, 0, 0, Grid.ClientRectangle.Width + 1, 42, "COPY MAP DATA", false);
            PlayButton = new GuiButton(21, 0, 0, Grid.ClientRectangle.Width + 1, 42, "PLAY MAP", false);

            JumpMSButton = new GuiButton(6, 0, 0, 64, 32, "JUMP", false);
			RotateButton = new GuiButton(7, 0, 0, 64, 32, "ROTATE", false);
			BezierButton = new GuiButton(10, 0, 0, 64, 32, "DRAW", false);
			BezierStoreButton = new GuiButton(13, 0, 0, 128, 32, "STORE NODES", false);
			BezierClearButton = new GuiButton(14, 0, 0, 128, 32, "CLEAR NODES", false);

			OpenTimings = new GuiButton(8, 0, 0, 200, 32, "OPEN BPM SETUP", false);
			OpenBookmarks = new GuiButton(20, 0, 0, 200, 32, "EDIT BOOKMARKS", false);
			UseCurrentMs = new GuiButton(9, 0, 0, 200, 32, "USE CURRENT MS", false);

			HFlip = new GuiButton(11, 0, 0, 128, 32, "HORIZONTAL FLIP", false);
			VFlip = new GuiButton(12, 0, 0, 128, 32, "VERTICAL FLIP", false);

			//Colors
			CreateColorset = new GuiButton(23, 0, 0, 128, 32, "CREATE COLORSET", false);
			OpenColorset = new GuiButton(24, 0, 0, 128, 32, "OPEN COLORSET", false);
			ExportColorset = new GuiButton(25, 0, 0, 128, 32, "EXPORT COLORSET", false);
			VisualizeColors = new GuiButton(26, 0, 0, 128, 32, "VISUALIZE COLORS", false);
			ManageLayers = new GuiButton(27, 0, 0, 128, 32, "MANAGE LAYERS", false);
			LayerPicker = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			AlternatePicker = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ColorPicker = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ColorR = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ColorG = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ColorB = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ColorRSlider = new GuiSlider(0, 0, 200, 32)
			{
				MaxValue = 255,
				Value = 125,
			};
			ColorGSlider = new GuiSlider(0, 0, 200, 32)
			{
				MaxValue = 255,
				Value = 125,
			};
			ColorBSlider = new GuiSlider(0, 0, 200, 32)
			{
				MaxValue = 255,
				Value = 125,
			};
			AddAlternate = new GuiButton(28, 0, 0, 128, 32, "ADD ALTERNATE", false);
			AddColor = new GuiButton(29, 0, 0, 128, 32, "ADD COLOR", false);
			ChangeColor = new GuiButton(30, 0, 0, 128, 32, "CHANGE COLOR", false);
			DeleteColor = new GuiButton(31, 0, 0, 128, 32, "DELETE COLOR", false);
			DeleteAlternate = new GuiButton(32, 0, 0, 128, 32, "DELETE ALTERNATE", false);
			ConfirmDelete = new GuiButton(33, 0, 0, 128, 32, "CONFIRM DELETE", false);
			ColorHex = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "#",
				Centered = true,
				Numeric = false,
			};
			SetNotes = new GuiButton(34, 0, 0, 128, 32, "SET NOTES", false);
			SetColor = new GuiButton(37, 0, 0, 128, 32, "SET COLOR", false);
			SetComparison = new GuiButton(35, 0, 0, 128, 32, "SET COMPARISON", false);
			ReverseSelection = new GuiButton(36, 0, 0, 128, 32, "", false);
			ShiftLevel = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "0",
				Centered = true,
				Numeric = true,
				CanBeNegative = true
			};
			ShiftDefault = new GuiButton(38, 0, 0, 128, 32, "DEFAULT SHIFT", false);
			ApplyShift = new GuiButton(39, 0, 0, 128, 32, "APPLY SHIFT", false);

			OptionsNav = new GuiButton(15, 0, 0, 200, 50, "OPTIONS >", false);
			TimingNav = new GuiButton(16, 0, 0, 200, 50, "TIMING >", false);
			PatternsNav = new GuiButton(17, 0, 0, 200, 50, "PATTERNS >", false);
			ColorsNav = new GuiButton(22, 0, 0, 200, 50, "COLORS >", false);

			SelectBound = new GuiButton(19, 0, 0, 64, 32, "SELECT", false);

			Autoplay = new GuiCheckBox(5, "Autoplay", 0, 0, 32, 32, Settings.Default.Autoplay);
			ApproachSquares = new GuiCheckBox(5, "Approach Squares", 0, 0, 32, 32, Settings.Default.ApproachSquares);
			GridNumbers = new GuiCheckBox(5, "Grid Numbers", 0, 0, 32, 32, Settings.Default.GridNumbers);
			GridLetters = new GuiCheckBox(5, "Grid Letters", 0, 0, 32, 32, Settings.Default.GridLetters);
			Quantum = new GuiCheckBox(5, "Quantum", 0, 0, 32, 32, Settings.Default.Quantum);
			AutoAdvance = new GuiCheckBox(5, "Auto-Advance", 0, 0, 32, 32, Settings.Default.AutoAdvance);
			Numpad = new GuiCheckBox(5, "Use Numpad", 0, 0, 32, 32, Settings.Default.Numpad);
			QuantumGridLines = new GuiCheckBox(5, "Quantum Grid Lines", 0, 0, 32, 32, Settings.Default.QuantumGridLines);
			QuantumGridSnap = new GuiCheckBox(5, "Snap to Grid", 0, 0, 32, 32, Settings.Default.QuantumGridSnap);
			Metronome = new GuiCheckBox(5, "Metronome", 0, 0, 32, 32, Settings.Default.Metronome);
			//DynamicBezier = new GuiCheckBox(5, "Show Bezier Preview", 0, 0, 32, 32, Settings.Default.DynamicBezier);
			CurveBezier = new GuiCheckBox(5, "Curve Bezier", 0, 0, 32, 32, Settings.Default.CurveBezier);
			//ClickToPlace = new GuiCheckBox(5, "Click to Place", 0, 0, 32, 32, Settings.Default.ClickToPlace);
			SeparateClickTools = new GuiCheckBox(5, "Separate Click Functions", 0, 0, 32, 32, Settings.Default.SeparateClickTools);
			//LegacyBPM = new GuiCheckBox(5, "Use Legacy Panel", 0, 0, 24, 24, Settings.Default.LegacyBPM);

			ScaleBox = new GuiTextBox(0, 0, 128, 32)
			{
				Text = "150",
				Centered = true,
				Numeric = true,
				CanBeNegative = false,
			};
			ScaleButton = new GuiButton(18, 0, 0, 200, 50, "SCALE", false);

			Offset.Focused = true;
			SfxOffset.Focused = true;
			JumpMSBox.Focused = true;
			RotateBox.Focused = true;
			BezierBox.Focused = true;
			ScaleBox.Focused = true;
			MSBoundLower.Focused = true;
			MSBoundHigher.Focused = true;
			LayerPicker.Focused = true;
			AlternatePicker.Focused = true;
			ColorPicker.Focused = true;
			ColorR.Focused = true;
			ColorG.Focused = true;
			ColorB.Focused = true;
			ShiftLevel.Focused = true;
			ColorHex.Focused = true;

			Offset.OnKeyDown(Key.Right, false);
			SfxOffset.OnKeyDown(Key.Right, false);
			JumpMSBox.OnKeyDown(Key.Right, false);
			RotateBox.OnKeyDown(Key.Right, false);
			BezierBox.OnKeyDown(Key.Right, false);
			ScaleBox.OnKeyDown(Key.Right, false);
			MSBoundLower.OnKeyDown(Key.Right, false);
			MSBoundHigher.OnKeyDown(Key.Right, false);
			LayerPicker.OnKeyDown(Key.Right, false);
			AlternatePicker.OnKeyDown(Key.Right, false);
			ColorPicker.OnKeyDown(Key.Right, false);
			ColorR.OnKeyDown(Key.Right, false);
			ColorG.OnKeyDown(Key.Right, false);
			ColorB.OnKeyDown(Key.Right, false);
			ShiftLevel.OnKeyDown(Key.Right, false);
			ColorHex.OnKeyDown(Key.Right, false);

			Offset.Focused = false;
			SfxOffset.Focused = false;
			JumpMSBox.Focused = false;
			RotateBox.Focused = false;
			BezierBox.Focused = false;
			ScaleBox.Focused = false;
			MSBoundLower.Focused = false;
			MSBoundHigher.Focused = false;
			LayerPicker.Focused = false;
			AlternatePicker.Focused = false;
			ColorPicker.Focused = false;
			ColorR.Focused = false;
			ColorG.Focused = false;
			ColorB.Focused = false;
			ShiftLevel.Focused = false;
			ColorHex.Focused = false;

			Buttons.Add(playPause);
			Buttons.Add(Timeline);
			Buttons.Add(Tempo);
			Buttons.Add(NoteAlign);
			Buttons.Add(MasterVolume);
			Buttons.Add(SfxVolume);
			Buttons.Add(BeatSnapDivisor);
			Buttons.Add(Autoplay);
			Buttons.Add(ApproachSquares);
			Buttons.Add(GridNumbers);
			Buttons.Add(GridLetters);
			Buttons.Add(Quantum);
			Buttons.Add(AutoAdvance);
			Buttons.Add(Numpad);
			Buttons.Add(QuantumGridLines);
			Buttons.Add(QuantumGridSnap);
			Buttons.Add(Metronome);
			//Buttons.Add(DynamicBezier);
			Buttons.Add(CurveBezier);
			//Buttons.Add(ClickToPlace);
			Buttons.Add(SeparateClickTools);
			//Buttons.Add(LegacyBPM);
			Buttons.Add(BackButton);
			Buttons.Add(CopyButton);
            Buttons.Add(PlayButton);
            Buttons.Add(JumpMSButton);
			Buttons.Add(RotateButton);
			Buttons.Add(OpenTimings);
			Buttons.Add(OpenBookmarks);
			Buttons.Add(UseCurrentMs);
			Buttons.Add(BezierButton);
			Buttons.Add(BezierClearButton);
			Buttons.Add(BezierStoreButton);
			Buttons.Add(HFlip);
			Buttons.Add(VFlip);
			Buttons.Add(CreateColorset);
			Buttons.Add(OpenColorset);
			Buttons.Add(ExportColorset);
			Buttons.Add(VisualizeColors);
			Buttons.Add(ManageLayers);
			Buttons.Add(AddAlternate);
			Buttons.Add(AddColor);
			Buttons.Add(ColorRSlider);
			Buttons.Add(ColorGSlider);
			Buttons.Add(ColorBSlider);
			Buttons.Add(SetColor);
			Buttons.Add(SetComparison);
			Buttons.Add(ReverseSelection);
			Buttons.Add(ShiftDefault);
			Buttons.Add(ApplyShift);
			Buttons.Add(ChangeColor);
			Buttons.Add(DeleteColor);
			Buttons.Add(DeleteAlternate);
			Buttons.Add(ConfirmDelete);
			Buttons.Add(SetNotes);
			Buttons.Add(OptionsNav);
			Buttons.Add(TimingNav);
			Buttons.Add(PatternsNav);
			Buttons.Add(ColorsNav);
			Buttons.Add(ScaleButton);
			Buttons.Add(TrackHeight);
			Buttons.Add(TrackCursorPos);
			Buttons.Add(ApproachRate);
			Buttons.Add(SelectBound);

			playLabel.Color = Color.FromArgb(255, 255, 255);
            noplayLabel.Color = Color.FromArgb(255, 255, 255);

            Boxes.Add(Offset);
			Boxes.Add(SfxOffset);
			Boxes.Add(JumpMSBox);
			Boxes.Add(RotateBox);
			Boxes.Add(BezierBox);
			Boxes.Add(ScaleBox);
			Boxes.Add(MSBoundLower);
			Boxes.Add(MSBoundHigher);
			Boxes.Add(LayerPicker);
			Boxes.Add(AlternatePicker);
			Boxes.Add(ColorPicker);
			Boxes.Add(ColorR);
			Boxes.Add(ColorG);
			Boxes.Add(ColorB);
			Boxes.Add(ShiftLevel);
			Boxes.Add(ColorHex);
			Boxes.Add(ShiftLevel);

			HideShowElements();

			EditorWindow.Instance.MusicPlayer.Volume = (float)Settings.Default.MasterVolume;

			SfxOffset.Text = Settings.Default.SfxOffset;
			MasterVolume.Value = (int)(Settings.Default.MasterVolume * MasterVolume.MaxValue);
			SfxVolume.Value = (int)(Settings.Default.SFXVolume * SfxVolume.MaxValue);
			TrackHeight.Value = Settings.Default.TrackHeight;
			TrackCursorPos.Value = Settings.Default.CursorPos;
			ApproachRate.Value = Settings.Default.ApproachRate;
			// NoteAlign.Value = (int)(Settings.Default.NoteAlign * NoteAlign.MaxValue);

			OnResize(EditorWindow.Instance.ClientSize);

			SfxOffset.OnChanged += (_, value) =>
			{
				Settings.Default.SfxOffset = SfxOffset.Text;
			};
		}

		public bool CanClick(Point pos)
        {
			foreach (var button in Buttons)
            {
				if (button.ClientRectangle.Contains(pos))
					return false;
            }
			foreach (var box in Boxes)
            {
				if (box.ClientRectangle.Contains(pos))
					return false;
            }
			return true;
        }

		public override void Render(float delta, float mouseX, float mouseY)
		{
			var rl = EditorWindow.Instance.inconspicuousvar;

			_toastTime = Math.Min(2, _toastTime + delta);

			var toastOffY = 1f;

			if (_toastTime <= 0.5)
			{
				toastOffY = (float)Math.Sin(Math.Min(0.5, _toastTime) / 0.5 * MathHelper.PiOver2);
			}
			else if (_toastTime >= 1.75)
			{
				toastOffY = (float)Math.Cos(Math.Min(0.25, _toastTime - 1.75) / 0.25 * MathHelper.PiOver2);
			}

			if (long.TryParse(Offset.Text, out var exportoffset) && exportoffset != GuiTrack.NoteOffset)
				GuiTrack.NoteOffset = exportoffset;

			var size = EditorWindow.Instance.ClientSize;
			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;
			var fr = EditorWindow.Instance.FontRenderer;
			var h = fr.GetHeight(_toast.FontSize);

			int bgdim = EditorSettings.EditorBGOpacity;

			if (bgImg)
			{
				GL.Color4(Color.FromArgb(bgdim, 255, 255, 255));
				Glu.RenderTexturedQuad(0, 0, size.Width, size.Height, 0, 0, 1, 1, _textureId);
			}

			_toast.ClientRectangle.Y = size.Height - toastOffY * h * 3.25f + h / 2;
			_toast.Color = Color.FromArgb((int)(Math.Pow(toastOffY, 3) * 255), _toast.Color);

			Color Color1 = EditorWindow.Instance.Color1;
			Color Color2 = EditorWindow.Instance.Color2;
			Color Color3 = EditorWindow.Instance.Color3;

			GL.Color3(Color1);
			var zoomW = fr.GetWidth("Zoom: ", 24);
			if (rl)
            {
				zoomW = fr.GetWidth("Zoom~ ", 24);
				fr.Render("Zoom~ ", (int)OptionsNav.ClientRectangle.Right + 10, (int)OptionsNav.ClientRectangle.Y, 24);
			}
			else
            {
				fr.Render("Zoom: ", (int)OptionsNav.ClientRectangle.Right + 10, (int)OptionsNav.ClientRectangle.Y, 24);
			}
			
			GL.Color3(Color2);
			fr.Render($"{Math.Round(EditorWindow.Instance.Zoom, 2) * 100}%", (int)OptionsNav.ClientRectangle.Right + zoomW + 10, (int)OptionsNav.ClientRectangle.Y, 24);
			GL.Color3(Color1);

			var th = 64 + (32 - TrackHeight.Value);
			var ar = ApproachRate.Value + 1;

			if (Settings.Default.SeparateClickTools)
            {
				if (rl)
					fr.Render("Cwick Mode~ " + (Settings.Default.SelectTool ? "Sewect" : "Pwace"), (int)BackButton.ClientRectangle.X, (int)BackButton.ClientRectangle.Bottom + 20, 24);
				else
					fr.Render("Click Mode: " + (Settings.Default.SelectTool ? "Select" : "Place"), (int)BackButton.ClientRectangle.X, (int)BackButton.ClientRectangle.Bottom + 20, 24);
			}

			if (OptionsNavEnabled)
            {
				if (rl)
                {
					var thw = fr.GetWidth("Twack Height~ 00", (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					fr.Render($"Twack Height~ {th}", (int)TrackHeight.ClientRectangle.Left - thw, (int)(SeparateClickTools.ClientRectangle.Bottom + 10 * heightdiff), (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					fr.Render($"Cuwsow Pos~ {TrackCursorPos.Value}%", (int)TrackCursorPos.ClientRectangle.X, (int)(SeparateClickTools.ClientRectangle.Bottom + 10 * heightdiff), (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					var arw = fr.GetWidth($"Appwoach Wate~ 00", (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					fr.Render($"Appwoach Wate~ {ar}", (int)ApproachRate.ClientRectangle.Left - arw, (int)(Quantum.ClientRectangle.Y + 10 * heightdiff), (int)Math.Min(24 * widthdiff, 24 * heightdiff));
				}
				else
                {
					var thw = fr.GetWidth("Track Height: 00", (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					fr.Render($"Track Height: {th}", (int)TrackHeight.ClientRectangle.Left - thw, (int)(SeparateClickTools.ClientRectangle.Bottom + 10 * heightdiff), (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					fr.Render($"Cursor Pos: {TrackCursorPos.Value}%", (int)TrackCursorPos.ClientRectangle.X, (int)(SeparateClickTools.ClientRectangle.Bottom + 10 * heightdiff), (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					var arw = fr.GetWidth($"Approach Rate: 00", (int)Math.Min(24 * widthdiff, 24 * heightdiff));
					fr.Render($"Approach Rate: {ar}", (int)ApproachRate.ClientRectangle.Left - arw, (int)(Quantum.ClientRectangle.Y + 10 * heightdiff), (int)Math.Min(24 * widthdiff, 24 * heightdiff));
				}
			}
			if (rl)
            {
				if (TimingNavEnabled)
					fr.Render("Export Offset[ms]~", (int)Offset.ClientRectangle.X, (int)Offset.ClientRectangle.Y - 24, 24);
				fr.Render("SFX Offset[ms]~", (int)SfxOffset.ClientRectangle.X, (int)SfxOffset.ClientRectangle.Y - 24, 24);
				fr.Render("Jump to MS~", (int)JumpMSBox.ClientRectangle.X, (int)JumpMSBox.ClientRectangle.Y - 24, 24);
				fr.Render("Sewect between MS~", (int)MSBoundLower.ClientRectangle.X, (int)MSBoundLower.ClientRectangle.Y - 24, 24);
			}
			else
            {
				if (TimingNavEnabled)
					fr.Render("Export Offset[ms]:", (int)Offset.ClientRectangle.X, (int)Offset.ClientRectangle.Y - 24, 24);
				fr.Render("SFX Offset[ms]:", (int)SfxOffset.ClientRectangle.X, (int)SfxOffset.ClientRectangle.Y - 24, 24);
				fr.Render("Jump to MS:", (int)JumpMSBox.ClientRectangle.X, (int)JumpMSBox.ClientRectangle.Y - 24, 24);
				fr.Render("Select between MS:", (int)MSBoundLower.ClientRectangle.X, (int)MSBoundLower.ClientRectangle.Y - 24, 24);
			}
			if (PatternsNavEnabled)
            {
				if (rl)
                {
					fr.Render("Dwaw Beziew with Divisow~", (int)BezierBox.ClientRectangle.X, (int)BezierBox.ClientRectangle.Y - 24, 24);
					fr.Render("Wotate by Degwees~", (int)RotateBox.ClientRectangle.X, (int)RotateBox.ClientRectangle.Y - 24, 24);
					fr.Render("Scawe by Pewcent~", (int)ScaleBox.ClientRectangle.X, (int)ScaleBox.ClientRectangle.Y - 24, 24);
				}
				else
                {
					fr.Render("Draw Bezier with Divisor:", (int)BezierBox.ClientRectangle.X, (int)BezierBox.ClientRectangle.Y - 24, 24);
					fr.Render("Rotate by Degrees:", (int)RotateBox.ClientRectangle.X, (int)RotateBox.ClientRectangle.Y - 24, 24);
					fr.Render("Scale by Percent:", (int)ScaleBox.ClientRectangle.X, (int)ScaleBox.ClientRectangle.Y - 24, 24);
				}
			}
			if (ColorsNavEnabled)
            {
				if (rl)
                {
					fr.Render("Pwease ask ow seawch on how to use these featuwes~", (int)ColorsNav.ClientRectangle.X, (int)ColorsNav.ClientRectangle.Y + 62, 16);
					fr.Render("Layew", (int)LayerPicker.ClientRectangle.X, (int)LayerPicker.ClientRectangle.Y - 24, 24);
					fr.Render("Awtewnate", (int)LayerPicker.ClientRectangle.X, (int)LayerPicker.ClientRectangle.Y - 24, 24);
					fr.Render("Cowow", (int)LayerPicker.ClientRectangle.X, (int)LayerPicker.ClientRectangle.Y - 24, 24);
				}
                else
                {
					fr.Render("Please ask or search on how to use these features.", (int)ColorsNav.ClientRectangle.X, (int)ColorsNav.ClientRectangle.Y + 62, 16);
					fr.Render("Layer", (int)LayerPicker.ClientRectangle.X, (int)LayerPicker.ClientRectangle.Y - 24, 24);
					fr.Render("Alternate", (int)AlternatePicker.ClientRectangle.X, (int)AlternatePicker.ClientRectangle.Y - 24, 24);
					fr.Render("Color", (int)ColorPicker.ClientRectangle.X, (int)ColorPicker.ClientRectangle.Y - 24, 24);
				}
				GL.Color4(Colorset.currentColor);
				Glu.RenderQuad(OptionsNav.ClientRectangle.X + 10, LayerPicker.ClientRectangle.Bottom + 10, 60, 60);
				GL.Color4(Colorset.previousColor);
				Glu.RenderQuad(OptionsNav.ClientRectangle.X + 10, LayerPicker.ClientRectangle.Bottom + 80, 60, 60);
				if(ColorsDisplayEnabled == true)
                {
					int colorAmount = 0;
					int layerNumber = 0;
					try 
					{
						layerNumber = Int32.Parse(LayerPicker.Text);
						colorAmount = Colorset.ColorAmount(Int32.Parse(LayerPicker.Text));
					}
					catch { Console.WriteLine("Wrong Layer!"); }
					List<OpenTK.Graphics.Color4> colorList = Colorset.ColorList(layerNumber);
					try
					{
						for (int i = 0; i < colorAmount; i++)
						{
							try
							{
								GL.Color4(colorList[i]);
								Glu.RenderQuad(OpenColorset.ClientRectangle.X + 200, ColorsNav.ClientRectangle.Bottom + 10 + (i * 8), 30, 7);
							}
							catch { Console.WriteLine("Visualizing Colorset Failed!"); }
						}
						for (int i = 0; i < Colorset.layers[Int32.Parse(LayerPicker.Text)].Alternates.Count; i++)
						{
							for (int k = 0; k < Colorset.layers[Int32.Parse(LayerPicker.Text)].Alternates[i].Colors.Count; k++)
							{
								try
								{
									GL.Color4(Colorset.layers[Int32.Parse(LayerPicker.Text)].Alternates[i].Colors[k]);
									Glu.RenderQuad(OpenColorset.ClientRectangle.X + 232 + (i * 12), ColorsNav.ClientRectangle.Bottom + 10 + (k * 8), 10, 7);
								}
								catch { Console.WriteLine("Rendering Color Failed!"); }
							}
						}
                    }
                    catch { Console.WriteLine("Major Failure in Visualizing Colorset!"); }
                }
			}
			GL.Color3(Color1);
			var divisor = rl ? $"Beat Divisow~ {BeatSnapDivisor.Value + 1}" : $"Beat Divisor: {BeatSnapDivisor.Value + 1}";
			var divisorW = fr.GetWidth(divisor, 24);
            var align = rl ? $"Snapping~ 3/{(float)(NoteAlign.Value + 1)}" : $"Snapping: 3/{(float)(NoteAlign.Value + 1)}";
            var alignW = fr.GetWidth(align, 24);

            fr.Render(divisor, (int)(BeatSnapDivisor.ClientRectangle.X + BeatSnapDivisor.ClientRectangle.Width / 2 - divisorW / 2f), (int)BeatSnapDivisor.ClientRectangle.Y - 20, 24);
            fr.Render(align, (int)(NoteAlign.ClientRectangle.X + NoteAlign.ClientRectangle.Width / 2 - alignW / 2f), (int)NoteAlign.ClientRectangle.Y - 20, 24);

			var tempoval = Tempo.Value;
			if (tempoval > 15)
				tempoval = (tempoval - 16) * 2 + 16;
            var tempo = rl ? $"PWAYBACK SPEED ~ {tempoval * 5 + 20}%" : $"PLAYBACK SPEED - {tempoval * 5 + 20}%";
			var tempoW = fr.GetWidth(tempo, 24);

			fr.Render(tempo, (int)(Tempo.ClientRectangle.X + Tempo.ClientRectangle.Width / 2 - tempoW / 2f), (int)Tempo.ClientRectangle.Bottom - 24, 24);

			var masterW = rl ? fr.GetWidth("Mastew", 18) : fr.GetWidth("Master", 18);
			var sfxW = fr.GetWidth("SFX", 18);

			var masterP = $"{(int)(MasterVolume.Value * 100f) / MasterVolume.MaxValue}";
			var sfxP = $"{(int)(SfxVolume.Value * 100f) / SfxVolume.MaxValue}";

			var masterPw = fr.GetWidth(masterP, 18);
			var sfxPw = fr.GetWidth(sfxP, 18);

			fr.Render("Music", (int)(MasterVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - masterW / 2f), (int)MasterVolume.ClientRectangle.Y - 2, 18);
			fr.Render("SFX", (int)(SfxVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - sfxW / 2f), (int)SfxVolume.ClientRectangle.Y - 2, 18);

			fr.Render(masterP, (int)(MasterVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - masterPw / 2f), (int)MasterVolume.ClientRectangle.Bottom - 16, 18);
			fr.Render(sfxP, (int)(SfxVolume.ClientRectangle.X + SfxVolume.ClientRectangle.Width / 2 - sfxPw / 2f), (int)SfxVolume.ClientRectangle.Bottom - 16, 18);

			var rect = Timeline.ClientRectangle;

			var timelinePos = new System.Numerics.Vector2(rect.Height / 2f, rect.Height / 2f - 5);
			var time = EditorWindow.Instance.totalTime;
			var currentTime = EditorWindow.Instance.currentTime;

			if (Timeline.Dragging)
				currentTime = TimeSpan.FromMilliseconds(MathHelper.Clamp(time.TotalMilliseconds * Timeline.Progress, 0, time.TotalMilliseconds - 1));

			var timeString = $"{time.Minutes}:{time.Seconds:0#}";
			var currentTimeString = $"{currentTime.Minutes}:{currentTime.Seconds:0#}";
			var currentMsString = $"{(long) currentTime.TotalMilliseconds:##,###}ms";
			if ((long) currentTime.TotalMilliseconds == 0)
				currentMsString = "0ms";

			var notesString = $"{EditorWindow.Instance.Notes.Count} Notes";
			var notesW = fr.GetWidth(notesString, 24);

			var timeW = fr.GetWidth(timeString, 20);
			var currentTimeW = fr.GetWidth(currentTimeString, 20);
			var currentMsW = fr.GetWidth(currentMsString, 20);

			fr.Render(notesString, (int)(rect.X + rect.Width / 2 - notesW / 2f), (int)(rect.Y + timelinePos.Y + 12), 24);

			GL.Color3(Color1);
			fr.Render(timeString, (int)(rect.X + timelinePos.X - timeW / 2f + rect.Width - rect.Height), (int)(rect.Y + timelinePos.Y + 12), 20);
			fr.Render(currentTimeString, (int)(rect.X + timelinePos.X - currentTimeW / 2f), (int)(rect.Y + timelinePos.Y + 12), 20);
			fr.Render(currentMsString, (int)(rect.X + rect.Height / 2 + (rect.Width - rect.Height) * Timeline.Progress - currentMsW / 2f), (int)rect.Y, 20);

			base.Render(delta, mouseX, mouseY);

			_toast.Render(delta, mouseX, mouseY);
			Grid.Render(delta, mouseX, mouseY);
			Track.Render(delta, mouseX, mouseY);
			NoteAlign.Render(delta, mouseX, mouseY);

			foreach (var box in Boxes)
            {
				box.Render(delta, mouseX, mouseY);
            }

			//bezier lines
			GL.LineWidth(2);
			if (int.TryParse(BezierBox.Text, out var bezdivisor) && bezdivisor > 0 && (beziernodes != null && beziernodes.Count > 1))
			{
				var anchored = new List<int>() { 0 };

				for (int i = 0; i < beziernodes.Count; i++)
				{
					if (beziernodes[i].Anchored && !anchored.Contains(i))
						anchored.Add(i);
				}
				if (!anchored.Contains(beziernodes.Count - 1))
					anchored.Add(beziernodes.Count - 1);

				for (int i = 1; i < anchored.Count; i++)
				{
					var newnodes = new List<Note>();
					for (int j = anchored[i - 1]; j <= anchored[i]; j++)
					{
						newnodes.Add(beziernodes[j]);
					}
					Bezier(newnodes, bezdivisor);
				}
			}
		}

		private void Bezier(List<Note> finalnodes, int bezdivisor)
        {
			try
			{
				var k = finalnodes.Count - 1;
				double d = 1f / (bezdivisor * k);
				float xprev = finalnodes[0].X * Grid.ClientRectangle.Width / 3 + Grid.ClientRectangle.X + Grid.ClientRectangle.Width / 6;
				float yprev = finalnodes[0].Y * Grid.ClientRectangle.Width / 3 + Grid.ClientRectangle.Y + Grid.ClientRectangle.Width / 6;
				if (!Settings.Default.CurveBezier)
					d = 1f / bezdivisor;
				if (Settings.Default.CurveBezier)
				{
					for (double t = 0; t <= 1; t += d)
					{
						float xg = 0;
						float yg = 0;
						float xf = 0;
						float yf = 0;
						for (int v = 0; v <= k; v++)
						{
							var note = finalnodes[v];
							var bez = (double)BinomialCoefficient(k, v) * (Math.Pow(1 - t, k - v) * Math.Pow(t, v));

							xf += (float)(bez * note.X);
							yf += (float)(bez * note.Y);
							xg = xf;
							yg = yf;
						}

						xf *= Grid.ClientRectangle.Width / 3;
						yf *= Grid.ClientRectangle.Width / 3;

						xf += Grid.ClientRectangle.X + Grid.ClientRectangle.Width / 6;
						yf += Grid.ClientRectangle.Y + Grid.ClientRectangle.Width / 6;

						GL.Color3(Color.FromArgb(255, 255, 255));
						GL.Begin(PrimitiveType.Lines);
						GL.Vertex2(xprev, yprev);
						GL.Vertex2(xf, yf);
						GL.End();
						//GL.Color3(Color1);
						//Glu.RenderCircle(xf, yf, 4);
						Grid.RenderFakeNote(xg, yg, EditorWindow.Instance.Color3);

						xprev = xf;
						yprev = yf;
					}
				}
				else
				{
					for (int v = 0; v < k; v++)
					{
						var note = finalnodes[v];
						var nextnote = finalnodes[v + 1];
						var xdist = nextnote.X - note.X;
						var ydist = nextnote.Y - note.Y;

						for (double t = 0; t <= 1; t += d)
						{
							float xf = note.X + xdist * (float)t;
							float yf = note.Y + ydist * (float)t;
							float xg = xf;
							float yg = yf;

							xf *= Grid.ClientRectangle.Width / 3;
							yf *= Grid.ClientRectangle.Width / 3;

							xf += Grid.ClientRectangle.X + Grid.ClientRectangle.Width / 6;
							yf += Grid.ClientRectangle.Y + Grid.ClientRectangle.Width / 6;

							GL.Color3(Color.FromArgb(255, 255, 255));
							GL.Begin(PrimitiveType.Lines);
							GL.Vertex2(xprev, yprev);
							GL.Vertex2(xf, yf);
							GL.End();
							//GL.Color3(Color1);
							//Glu.RenderCircle(xf, yf, 4);
							Grid.RenderFakeNote(xg, yg, EditorWindow.Instance.Color3);

							xprev = xf;
							yprev = yf;
						}
					}
				}
			}
			catch
			{
				beziernodes.Clear();
			}
		}

		public override bool AllowInput()
		{
			foreach (var box in Boxes)
            {
				if (box.Focused)
					return false;
            }
			return true;
		}

		public override void OnKeyTyped(char key)
		{
			Offset.OnKeyTyped(key);
			SfxOffset.OnKeyTyped(key);
			JumpMSBox.OnKeyTyped(key);
			RotateBox.OnKeyTyped(key);
			BezierBox.OnKeyTyped(key);
			ScaleBox.OnKeyTyped(key);
			MSBoundLower.OnKeyTyped(key);
			MSBoundHigher.OnKeyTyped(key);
			LayerPicker.OnKeyTyped(key);
			AlternatePicker.OnKeyTyped(key);
			ColorPicker.OnKeyTyped(key);
			ColorR.OnKeyTyped(key);
			ColorG.OnKeyTyped(key);
			ColorB.OnKeyTyped(key);
			ShiftLevel.OnKeyTyped(key);
			ColorHex.OnKeyTyped(key);

			UpdateTrack();
		}

		public override void OnKeyDown(Key key, bool control)
		{
			Offset.OnKeyDown(key, control);
			SfxOffset.OnKeyDown(key, control);
			JumpMSBox.OnKeyDown(key, control);
			RotateBox.OnKeyDown(key, control);
			BezierBox.OnKeyDown(key, control);
			ScaleBox.OnKeyDown(key, control);
			MSBoundLower.OnKeyDown(key, control);
			MSBoundHigher.OnKeyDown(key, control);
			LayerPicker.OnKeyDown(key, control);
			AlternatePicker.OnKeyDown(key, control);
			ColorPicker.OnKeyDown(key, control);
			ColorR.OnKeyDown(key, control);
			ColorG.OnKeyDown(key, control);
			ColorB.OnKeyDown(key, control);
			ShiftLevel.OnKeyDown(key, control);
			ColorHex.OnKeyDown(key, control);

			UpdateTrack();
		}

		public override void OnMouseClick(float x, float y)
		{
			Offset.OnMouseClick(x, y);
			SfxOffset.OnMouseClick(x, y);
			JumpMSBox.OnMouseClick(x, y);
			RotateBox.OnMouseClick(x, y);
			BezierBox.OnMouseClick(x, y);
			ScaleBox.OnMouseClick(x, y);
			MSBoundLower.OnMouseClick(x, y);
			MSBoundHigher.OnMouseClick(x, y);
			LayerPicker.OnMouseClick(x, y);
			AlternatePicker.OnMouseClick(x, y);
			ColorPicker.OnMouseClick(x, y);
			ColorR.OnMouseClick(x, y);
			ColorG.OnMouseClick(x, y);
			ColorB.OnMouseClick(x, y);
			ShiftLevel.OnMouseClick(x, y);
			ColorHex.OnMouseClick(x, y);

			if (Timeline.SelectedBookmark != null)
				EditorWindow.Instance.currentTime = TimeSpan.FromMilliseconds(Timeline.SelectedBookmark.MS);

			base.OnMouseClick(x, y);
		}
		public override void OnMouseMove(float x, float y)
		{
			Timeline.OnMouseMove(x, y);
			base.OnMouseMove(x, y);
		}

		private void HideShowElements()
        {
			//button placement
			var heightdiff = EditorWindow.Instance.ClientSize.Height / 1080f;
			TimingNav.ClientRectangle.Y = OptionsNav.ClientRectangle.Bottom + 10 * heightdiff;
			PatternsNav.ClientRectangle.Y = TimingNav.ClientRectangle.Bottom + 10 * heightdiff;
			ColorsNav.ClientRectangle.Y = PatternsNav.ClientRectangle.Bottom + 10 * heightdiff;

			//options
			Autoplay.Visible = false;
			ApproachSquares.Visible = false;
			GridNumbers.Visible = false;
			GridLetters.Visible = false;
			Quantum.Visible = false;
			Numpad.Visible = false;
			QuantumGridLines.Visible = false;
			QuantumGridSnap.Visible = false;
			Metronome.Visible = false;
			TrackHeight.Visible = false;
			TrackCursorPos.Visible = false;
			ApproachRate.Visible = false;
			//ClickToPlace.Visible = false;
			SeparateClickTools.Visible = false;

			//timing
			Offset.Visible = false;
			UseCurrentMs.Visible = false;
			OpenTimings.Visible = false;
			OpenBookmarks.Visible = false;

			//patterns
			RotateBox.Visible = false;
			BezierBox.Visible = false;
			//DynamicBezier.Visible = false;
			CurveBezier.Visible = false;
			RotateButton.Visible = false;
			BezierButton.Visible = false;
			BezierStoreButton.Visible = false;
			BezierClearButton.Visible = false;
			HFlip.Visible = false;
			VFlip.Visible = false;
			ScaleBox.Visible = false;
			ScaleButton.Visible = false;

			//colors
			CreateColorset.Visible = false;
			OpenColorset.Visible = false;
			ExportColorset.Visible = false;
			VisualizeColors.Visible = false;
			ManageLayers.Visible = false;
			LayerPicker.Visible = false;
			LayerWarning.Visible = false;
			AlternatePicker.Visible = false;
			ColorPicker.Visible = false;
			ColorR.Visible = false;
			ColorG.Visible = false;
			ColorB.Visible = false;
			ColorRSlider.Visible = false;
			ColorGSlider.Visible = false;
			ColorBSlider.Visible = false;
			AddAlternate.Visible = false;
			AddColor.Visible = false;
			ChangeColor.Visible = false;
			DeleteColor.Visible = false;
			DeleteAlternate.Visible = false;
			ConfirmDelete.Visible = false;
			ColorHex.Visible = false;
			SetNotes.Visible = false;
			SetColor.Visible = false;
			SetComparison.Visible = false;
			ReverseSelection.Visible = false;
			ShiftLevel.Visible = false;
			ShiftDefault.Visible = false;
			ApplyShift.Visible = false;

			//button text
			OptionsNav.Text = "OPTIONS >";
			TimingNav.Text = "TIMING >";
			PatternsNav.Text = "PATTERNS >";
			ColorsNav.Text = "COLORS >";


			if (OptionsNavEnabled)
            {
				Autoplay.Visible = true;
				ApproachSquares.Visible = true;
				GridNumbers.Visible = true;
				GridLetters.Visible = true;
				Quantum.Visible = true;
				Numpad.Visible = true;
				QuantumGridLines.Visible = true;
				QuantumGridSnap.Visible = true;
				Metronome.Visible = true;
				TrackHeight.Visible = true;
				TrackCursorPos.Visible = true;
				ApproachRate.Visible = true;
				//ClickToPlace.Visible = true;
				SeparateClickTools.Visible = true;

				TimingNav.ClientRectangle.Y = TrackCursorPos.ClientRectangle.Bottom + 20 * heightdiff;
				PatternsNav.ClientRectangle.Y = TimingNav.ClientRectangle.Bottom + 10 * heightdiff;
				ColorsNav.ClientRectangle.Y = PatternsNav.ClientRectangle.Bottom + 10 * heightdiff;

				OptionsNav.Text = "OPTIONS <";
			}

			if (TimingNavEnabled)
            {
				Offset.Visible = true;
				UseCurrentMs.Visible = true;
				OpenTimings.Visible = true;
				OpenBookmarks.Visible = true;

				PatternsNav.ClientRectangle.Y = OpenBookmarks.ClientRectangle.Bottom + 20 * heightdiff;
				ColorsNav.ClientRectangle.Y = PatternsNav.ClientRectangle.Bottom + 10 * heightdiff;

				TimingNav.Text = "TIMING <";
			}

			if (PatternsNavEnabled)
            {
				RotateBox.Visible = true;
				BezierBox.Visible = true;
				//DynamicBezier.Visible = true;
				CurveBezier.Visible = true;
				RotateButton.Visible = true;
				BezierButton.Visible = true;
				BezierStoreButton.Visible = true;
				BezierClearButton.Visible = true;
				HFlip.Visible = true;
				VFlip.Visible = true;
				ScaleBox.Visible = true;
				ScaleButton.Visible = true;

				ColorsNav.ClientRectangle.Y = ScaleButton.ClientRectangle.Bottom + 20 * heightdiff;

				PatternsNav.Text = "PATTERNS <";
			}

			if (ColorsNavEnabled)
            {
				CreateColorset.Visible = true;
				OpenColorset.Visible = true;
				ExportColorset.Visible = true;
				VisualizeColors.Visible = true;
				ManageLayers.Visible = true;
				LayerPicker.Visible = true;
				LayerWarning.Visible = true;
				AlternatePicker.Visible = true;
				ColorPicker.Visible = true;
				ColorR.Visible = true;
				ColorG.Visible = true;
				ColorB.Visible = true;
				ColorRSlider.Visible = true;
				ColorGSlider.Visible = true;
				ColorBSlider.Visible = true;
				AddAlternate.Visible = true;
				AddColor.Visible = true;
				ChangeColor.Visible = true;
				DeleteColor.Visible = true;
				DeleteAlternate.Visible = true;
				ConfirmDelete.Visible = true;
				ColorHex.Visible = true;
				SetNotes.Visible = true;
				SetColor.Visible = true;
				SetComparison.Visible = true;
				ReverseSelection.Visible = true;
				SetColor.Visible = true;
				ShiftLevel.Visible = true;
				ShiftDefault.Visible = true;
				ApplyShift.Visible = true;

				ColorsNav.Text = "COLORS <";
            }
        }

		protected override void OnButtonClicked(int id)
		{
			int ad1 = Int32.Parse(LayerPicker.Text);
			int ad2 = Int32.Parse(AlternatePicker.Text);
			int ad3 = Int32.Parse(ColorPicker.Text);
			switch (id)
			{
				case 0:
					if (EditorWindow.Instance.MusicPlayer.IsPlaying)
						EditorWindow.Instance.MusicPlayer.Pause();
					else
                    {
						if (EditorWindow.Instance.currentTime.TotalMilliseconds >= EditorWindow.Instance.totalTime.TotalMilliseconds - 1)
							EditorWindow.Instance.currentTime = TimeSpan.FromMilliseconds(0);
						EditorWindow.Instance.MusicPlayer.Play();
					}
					break;
				case 3:
					if (EditorWindow.Instance.WillClose())
					{
						EditorWindow.Instance.UndoRedo.Clear();
						EditorWindow.Instance.Notes.Clear();
						EditorWindow.Instance.SelectedNotes.Clear();
						EditorWindow.Instance.MusicPlayer.Reset();
						EditorWindow.Instance.OpenGuiScreen(new GuiScreenMenu());
						EditorWindow.Instance.UpdateActivity("Sitting in the menu");
					}
					break;
				case 4:
                    try
                    {
                        Clipboard.SetText(EditorWindow.Instance.ParseData(true));
                        ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
                    }
                    catch
                    {
						ShowToast("FAILED TO COPY", Color.FromArgb(255, 200, 0));
                    }
					break;
                case 21:
					// play button
					if (EditorWindow.Instance.ActualAudio.Length == 0) return;
                    EditorWindow.Instance.ToggleGrid(false);
                    try
					{
                        string path = EditorWindow.Instance.ActualAudioPath.Replace(".", "{DOT}").Replace(" ", "{SPC}");

                        File.WriteAllText("temp.txt", EditorWindow.Instance.ParseData(false));
                        string color1 = string.Format("{0:X2}{1:X2}{2:X2}", EditorSettings.Color1.R, EditorSettings.Color1.G, EditorSettings.Color1.B);
                        string color2 = string.Format("{0:X2}{1:X2}{2:X2}", EditorSettings.Color2.R, EditorSettings.Color2.G, EditorSettings.Color2.B);

                        string at = EditorSettings.ApproachTime.ToString().Replace(",", ".");
                        string vol = EditorWindow.Instance.MusicPlayer.Volume.ToString().Replace(',', '.');


                        if (EditorWindow.Instance._shiftDown)
                        {
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = "MapPlayer.exe";
                            p.StartInfo.Arguments = string.Format(">>md={0} >>aud={1} >>at={2} >>ad={3} >>sf={4} >>sm={5} >>chroma={6},{7} >>sens={8} >>motion={9} >>vol={10} >>thirds", "temp.txt", path, at, EditorSettings.ApproachDistance, EditorWindow.Instance.currentTime.TotalSeconds, EditorWindow.Instance.tempo, color1, color2, EditorSettings.Sensitivity, EditorSettings.Parallax, vol);
                            Console.WriteLine(p.StartInfo.Arguments);
                            p.Start();
                            p.WaitForExit();
                            File.Delete("temp.txt");
                            EditorWindow.Instance._shiftDown = false;
                        }
                        else
                        {
                            System.Diagnostics.Process p = new System.Diagnostics.Process();
                            p.StartInfo.FileName = "MapPlayer.exe";
                            p.StartInfo.Arguments = string.Format(">>md={0} >>aud={1} >>at={2} >>ad={3} >>sf={4} >>sm={5} >>chroma={6},{7} >>sens={8} >>motion={9} >>vol={10} >>thirds", "temp.txt", path, at, EditorSettings.ApproachDistance, 0, EditorWindow.Instance.tempo, color1, color2, EditorSettings.Sensitivity, EditorSettings.Parallax, vol);
                            Console.WriteLine(p.StartInfo.Arguments);
                            p.Start();
                            p.WaitForExit();
                            File.Delete("temp.txt");
                        }
                    } catch
					{
                        EditorWindow.Instance.ToggleGrid(true);
                    }
                    EditorWindow.Instance.ToggleGrid(true);
                    break;
                case 5:
					Settings.Default.Autoplay = Autoplay.Toggle;
					Settings.Default.ApproachSquares = ApproachSquares.Toggle;
					Settings.Default.GridNumbers = GridNumbers.Toggle;
					Settings.Default.GridLetters = GridLetters.Toggle;
					Settings.Default.Quantum = Quantum.Toggle;
					Settings.Default.AutoAdvance = AutoAdvance.Toggle;
					Settings.Default.Numpad = Numpad.Toggle;
					Settings.Default.QuantumGridLines = QuantumGridLines.Toggle;
					Settings.Default.QuantumGridSnap = QuantumGridSnap.Toggle;
					Settings.Default.Metronome = Metronome.Toggle;
					Settings.Default.SfxOffset = SfxOffset.Text;
					//Settings.Default.DynamicBezier = DynamicBezier.Toggle;
					Settings.Default.CurveBezier = CurveBezier.Toggle;
					//Settings.Default.ClickToPlace = ClickToPlace.Toggle;
					Settings.Default.SeparateClickTools = SeparateClickTools.Toggle;
					//Settings.Default.LegacyBPM = LegacyBPM.Toggle;
					Settings.Default.Save();
					EditorSettings.RefreshKeymapping();
					break;
				case 6:
					if (long.TryParse(JumpMSBox.Text, out var time))
                    {
						if (time <= EditorWindow.Instance.totalTime.TotalMilliseconds)
							EditorWindow.Instance.currentTime = TimeSpan.FromMilliseconds(time);
					}
					break;
				case 7:
					var degrees = float.Parse(RotateBox.Text);
					var nodes = EditorWindow.Instance.SelectedNotes.ToList();

					foreach (var node in nodes)
                    {
						var angle = MathHelper.RadiansToDegrees(Math.Atan2(node.Y - 1, node.X - 1));
						var distance = Math.Sqrt(Math.Pow(node.X - 1, 2) + Math.Pow(node.Y - 1, 2));
						var finalradians = MathHelper.DegreesToRadians(angle + degrees);

						node.X = (float)(Math.Cos(finalradians) * distance + 1);
						node.Y = (float)(Math.Sin(finalradians) * distance + 1);
                    }
					EditorWindow.Instance.UndoRedo.AddUndoRedo("ROTATE " + degrees.ToString(), () =>
					{
						var undodeg = 360 - degrees;

						foreach (var node in nodes)
						{
							var angle = MathHelper.RadiansToDegrees(Math.Atan2(node.Y - 1, node.X - 1));
							var distance = Math.Sqrt(Math.Pow(node.X - 1, 2) + Math.Pow(node.Y - 1, 2));
							var finalradians = MathHelper.DegreesToRadians(angle + undodeg);

							node.X = (float)(Math.Cos(finalradians) * distance + 1);
							node.Y = (float)(Math.Sin(finalradians) * distance + 1);
						}
					}, () =>
					{
						foreach (var node in nodes)
						{
							var angle = MathHelper.RadiansToDegrees(Math.Atan2(node.Y - 1, node.X - 1));
							var distance = Math.Sqrt(Math.Pow(node.X - 1, 2) + Math.Pow(node.Y - 1, 2));
							var finalradians = MathHelper.DegreesToRadians(angle + degrees);

							node.X = (float)(Math.Cos(finalradians) * distance + 1);
							node.Y = (float)(Math.Sin(finalradians) * distance + 1);
						}
					});
					break;
				case 8:
					/*
					void openGui()
					{
						if (TimingPoints != null)
						{
							TimingPoints.Close();
						}
					TimingPoints = new TimingPoints();
					TimingPoints.Run();
					}

					Thread t = new Thread(new ThreadStart(openGui));
					t.Start();
					*/
					if (TimingsWindow.inst != null)
						TimingsWindow.inst.Close();
					new TimingsWindow().Show();
					break;
				case 20:
					if (BookmarkSetup.inst != null)
						BookmarkSetup.inst.Close();
					new BookmarkSetup().Show();
					break;
				case 9:
					Offset.Text = ((long)EditorWindow.Instance.currentTime.TotalMilliseconds).ToString();
					break;
				case 10:
					if (int.TryParse(BezierBox.Text, out var divisor) && divisor > 0 && ((beziernodes != null && beziernodes.Count > 1) || EditorWindow.Instance.SelectedNotes.Count > 1))
					{
						var success = true;
						var finalnodes = EditorWindow.Instance.SelectedNotes.ToList();
						if (beziernodes != null && beziernodes.Count > 1)
							finalnodes = beziernodes;
						var finalnotes = new List<Note>();

						var anchored = new List<int>() { 0 };

						for (int i = 0; i < finalnodes.Count; i++)
						{
							if (finalnodes[i].Anchored && !anchored.Contains(i))
								anchored.Add(i);
						}
						if (!anchored.Contains(finalnodes.Count - 1))
							anchored.Add(finalnodes.Count - 1);

						for (int i = 1; i < anchored.Count; i++)
						{
							var newnodes = new List<Note>();
							for (int j = anchored[i - 1]; j <= anchored[i]; j++)
							{
								newnodes.Add(finalnodes[j]);
							}
							var finalbez = EditorWindow.Instance.Bezier(newnodes, divisor);
							success = finalbez != null;
							if (success)
								finalnotes = EditorWindow.Instance.CombineLists(finalnotes, EditorWindow.Instance.Bezier(newnodes, divisor));
						}

						EditorWindow.Instance.SelectedNotes.Clear();
						if (!Settings.Default.CurveBezier)
							finalnodes = new List<Note>();
						else
							finalnotes.Add(finalnodes[0]);

						if (success)
							EditorWindow.Instance.UndoRedoBezier(finalnotes, finalnodes);
					}
					break;
				case 11:
					var selectedH = EditorWindow.Instance.SelectedNotes.ToList();
					foreach (var node in selectedH)
					{
						node.X = 2 - node.X;
					}

					EditorWindow.Instance.UndoRedo.AddUndoRedo("HORIZONTAL FLIP", () =>
					{
						foreach (var node in selectedH)
						{
							node.X = 2 - node.X;
						}

					}, () =>
					{
						foreach (var node in selectedH)
						{
							node.X = 2 - node.X;
						}

					});
					break;
				case 12:
					var selectedV = EditorWindow.Instance.SelectedNotes.ToList();
					foreach (var node in selectedV)
					{
						node.Y = 2 - node.Y;
					}

					EditorWindow.Instance.UndoRedo.AddUndoRedo("VERTICAL FLIP", () =>
					{
						foreach (var node in selectedV)
						{
							node.Y = 2 - node.Y;
						}

					}, () =>
					{
						foreach (var node in selectedV)
						{
							node.Y = 2 - node.Y;
						}

					});
					break;
				case 13:
					if (EditorWindow.Instance.SelectedNotes.Count > 1)
						beziernodes = EditorWindow.Instance.SelectedNotes.ToList();
					break;
				case 14:
					if (beziernodes != null)
						beziernodes.Clear();
					break;
				case 15:
					OptionsNavEnabled = !OptionsNavEnabled;
					TimingNavEnabled = false;
					PatternsNavEnabled = false;
					ColorsNavEnabled = false;
					HideShowElements();
					break;
				case 16:
					OptionsNavEnabled = false;
					TimingNavEnabled = !TimingNavEnabled;
					PatternsNavEnabled = false;
					ColorsNavEnabled = false;
					HideShowElements();
					break;
				case 17:
					OptionsNavEnabled = false;
					TimingNavEnabled = false;
					PatternsNavEnabled = !PatternsNavEnabled;
					ColorsNavEnabled = false;
					HideShowElements();
					break;
				case 18:
					if (int.TryParse(ScaleBox.Text, out var scale))
                    {
						var scalef = scale / 100f;
						var selected = EditorWindow.Instance.SelectedNotes.ToList();
						foreach (var note in selected)
                        {
							note.X = (note.X - 1) * scalef + 1;
							note.Y = (note.Y - 1) * scalef + 1;
                        }

						EditorWindow.Instance.UndoRedo.AddUndoRedo($"SCALE {scale}%", () =>
						{
							foreach (var note in selected)
							{
								note.X = (note.X - 1) / scalef + 1;
								note.Y = (note.Y - 1) / scalef + 1;
							}

						}, () =>
						{
							foreach (var note in selected)
							{
								note.X = (note.X - 1) * scalef + 1;
								note.Y = (note.Y - 1) * scalef + 1;
							}

						});
                    }
					break;
				case 19:
					if (long.TryParse(MSBoundHigher.Text, out var mshigh) && long.TryParse(MSBoundLower.Text, out var mslow))
                    {
						var mstop = mshigh;
						var msbot = mslow;

						EditorWindow.Instance.SelectedNotes.Clear();

						foreach (var note in EditorWindow.Instance.Notes.ToList())
                        {
							if ((note.Ms > msbot && note.Ms < mstop) || (note.Ms < msbot && note.Ms > mstop))
                            {
								EditorWindow.Instance.SelectedNotes.Add(note);
                            }
                        }

						EditorWindow.Instance._draggedNotes = EditorWindow.Instance.SelectedNotes;
                    }
					break;
				case 22:
					OptionsNavEnabled = false;
					TimingNavEnabled = false;
					PatternsNavEnabled = false;
					ColorsNavEnabled = !ColorsNavEnabled;
					HideShowElements();
					break;
				case 23:
					if (EditorWindow.Instance.PromptColorsetSave())
					{
						EditorWindow.Instance.Notes.SetNotesWhite();
					}
					break;
				case 24:
					using (var dialog = new OpenFileDialog
					{
						Title = "Select Colorset File",
						Filter = "Text Documents (*.colorset)|*.colorset"
					})
					{
						if (dialog.ShowDialog() == DialogResult.OK)
						{
							Colorset.LoadColorset(dialog.FileName);
						}
					}
					break;
				case 25:
					//EXPORT COLORSET
					if(Colorset.hasColorset == true)
                    {
						using (var sfd = new SaveFileDialog
						{
							Title = "Save Colorset",
							Filter = "Text Documents (*.txt)|*.txt"
						})
						{
							var wasFullscreen = EditorWindow.Instance.IsFullscreen;

							if (EditorWindow.Instance.IsFullscreen)
							{
								EditorWindow.Instance.ToggleFullscreen();
							}

							var result = sfd.ShowDialog();

							if (wasFullscreen)
							{
								EditorWindow.Instance.ToggleFullscreen();
							}

							if (result == DialogResult.OK)
							{

								EditorWindow.Instance.ExportColorsetFile(sfd.FileName);
							}
						}
					}
					break;
				case 26:
					//VISUALIZE COLORS
					if (Colorset.hasColorset == true)
					{
						if (ColorsDisplayEnabled == true) { ColorsDisplayEnabled = false; }
						else if (ColorsDisplayEnabled == false) { ColorsDisplayEnabled = true; };
					}
					break;
				case 27:
					//MANAGE LAYERS
					if (Colorset.hasColorset == true)
					{
						if (LayerViewer.inst != null)
							LayerViewer.inst.Close();
						LayerViewer.inst = new LayerViewer();
						LayerViewer.inst.Show();
					}

					break;
				case 28:
					//ADD ALTERNATE
					if (Colorset.hasColorset == true)
					{
						Colorset.AddAlternate(ad1, ad2);
						Colorset.DefaultColors();
						Colorset.SetupColorset();
					}
					break;
				case 29:
					//ADD COLOR
					if (Colorset.hasColorset == true)
					{
						Colorset.AddColor(ad1, ad2, ad3, Colorset.currentColor);
						Colorset.DefaultColors();
						Colorset.SetupColorset();
					}
					break;
				case 30:
					//CHANGE COLOR
					if (Colorset.hasColorset == true)
					{
						Colorset.ChangeColor(ad1, ad2, ad3);
						Colorset.SetupColorset();
					}
					break;
				case 31:
					//DELETE COLOR
					delete = 1;
					break;
				case 32:
					//DELETE ALTERNATE
					delete = 2;
                    break;
                case 33:
					if (Colorset.hasColorset == true)
					{
						if (delete == 1)
						{
							Colorset.DeleteColor(ad1, ad2, ad3);
							delete = 0;
						}
						if (delete == 2)
						{
							Colorset.DeleteAlternate(ad1, ad2);
							delete = 0;
						}
						Colorset.DefaultColors();
						Colorset.SetupColorset();
					}
					break;
				case 34:
					//SET NOTES
					if (Colorset.hasColorset == true)
					{
						Console.WriteLine("Set Notes");
						var selectedNotes = EditorWindow.Instance.SelectedNotes.ToList();
						Colorset.AssignNotes(selectedNotes, Int32.Parse(LayerPicker.Text), Int32.Parse(ShiftLevel.Text));
					}
					break;
				case 35:
					//SET COMPARISON
					Colorset.previousColor = new OpenTK.Graphics.Color4((float)Int32.Parse(ColorR.Text) / 255, (float)Int32.Parse(ColorG.Text) / 255, (float)Int32.Parse(ColorB.Text) / 255, 255);
					break;
				case 36:
					//REVERSE SELECTION (FOR RGB) (UNMARKED BUTTON)
					//I CHANGED MY FUCKING MIND
					//THIS BUTTON MAKES THE HEX CODE TURN TO SLIDER
					OpenTK.Graphics.Color4 obama = HexColor.HextoColor4(ColorHex.Text);
					ColorRSlider.Value = (int)(obama.R * 255);
					ColorGSlider.Value = (int)(obama.G * 255);
					ColorBSlider.Value = (int)(obama.B * 255);
					ColorR.Text = (obama.R * 255).ToString();
					ColorG.Text = (obama.G * 255).ToString();
					ColorB.Text = (obama.B * 255).ToString();
					break;
				case 37:
					//SET COLOR 
					Colorset.currentColor = new OpenTK.Graphics.Color4((float)Int32.Parse(ColorR.Text) / 255, (float)Int32.Parse(ColorG.Text) / 255, (float)Int32.Parse(ColorB.Text) / 255, 255);
					break;
				case 38:
					//SHIFT DEFAULT
					if (Colorset.hasColorset == true)
					{
						var selectedNotes = EditorWindow.Instance.SelectedNotes.ToList();
						bool firstnote = true;
						foreach (Note note in selectedNotes)
						{
							if (firstnote == true)
							{
								note.shift = -999999;
								firstnote = false;
							}
							else
							{
								note.shift = 0;
							}
							Console.WriteLine(note.shift);
						}
						Colorset.VerifyNotes();
					}
					break;
				case 39:
					//APPLY SHIFT
					if (Colorset.hasColorset == true)
					{
						var selectedNotes = EditorWindow.Instance.SelectedNotes.ToList();
						foreach (Note note in selectedNotes)
						{
							note.shift += Int32.Parse(ShiftLevel.Text);
							Console.WriteLine(note.shift);
						}
						Colorset.VerifyNotes();
					}
					break;
			}
		}

		public override void OnResize(Size size)
		{
			Buttons[0].ClientRectangle = new RectangleF(size.Width - 512 - 64, size.Height - 64, 64, 64);

			ClientRectangle = new RectangleF(0, size.Height - 64, size.Width - 512 - 64, 64);

			Track.ClientRectangle.Height = 64 + (32 - TrackHeight.Value);

			Track.OnResize(size);
			Tempo.OnResize(size);
			MasterVolume.OnResize(size);
			NoteAlign.OnResize(size);
			TrackHeight.OnResize(size);
			TrackCursorPos.OnResize(size);
			ApproachRate.OnResize(size);
			ColorRSlider.OnResize(size);
			ColorGSlider.OnResize(size);
			ColorBSlider.OnResize(size);

			MasterVolume.ClientRectangle.Location = new PointF(EditorWindow.Instance.ClientSize.Width - 64, EditorWindow.Instance.ClientSize.Height - MasterVolume.ClientRectangle.Height - 64);
			SfxVolume.ClientRectangle.Location = new PointF(MasterVolume.ClientRectangle.X - 64, EditorWindow.Instance.ClientSize.Height - SfxVolume.ClientRectangle.Height - 64);

			Grid.ClientRectangle = new RectangleF((int)(size.Width / 2f - Grid.ClientRectangle.Width / 2), (int)((size.Height + Track.ClientRectangle.Height - 64) / 2 - Grid.ClientRectangle.Height / 2), Grid.ClientRectangle.Width, Grid.ClientRectangle.Height);
			BeatSnapDivisor.ClientRectangle.Location = new PointF(EditorWindow.Instance.ClientSize.Width - BeatSnapDivisor.ClientRectangle.Width, Grid.ClientRectangle.Y + 28);
			Timeline.ClientRectangle = new RectangleF(0, EditorWindow.Instance.ClientSize.Height - 64, EditorWindow.Instance.ClientSize.Width - 512 - 64, 64);
			Tempo.ClientRectangle = new RectangleF(EditorWindow.Instance.ClientSize.Width - 512, EditorWindow.Instance.ClientSize.Height - 64, 512, 64);

			BeatSnapDivisor.ClientRectangle.Y = Grid.ClientRectangle.Y + 28;
            NoteAlign.ClientRectangle.Y = BeatSnapDivisor.ClientRectangle.Bottom + 5 + 24;
			NoteAlign.ClientRectangle.X = BeatSnapDivisor.ClientRectangle.X;

			_toast.ClientRectangle.X = size.Width / 2f;


			var widthdiff = size.Width / 1920f;
			var heightdiff = size.Height / 1080f;

			OptionsNav.ClientRectangle.Size = new SizeF(400 * widthdiff, 50 * heightdiff);
			TimingNav.ClientRectangle.Size = OptionsNav.ClientRectangle.Size;
			PatternsNav.ClientRectangle.Size = OptionsNav.ClientRectangle.Size;
			ColorsNav.ClientRectangle.Size = OptionsNav.ClientRectangle.Size;

			//timing
			Offset.ClientRectangle.Size = new SizeF(128 * widthdiff, 40 * heightdiff);
			UseCurrentMs.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 2f + 5 * widthdiff, Offset.ClientRectangle.Height);
			OpenTimings.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			OpenBookmarks.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;

			//options
			Autoplay.ClientRectangle.Size = new SizeF(40 * widthdiff, 40 * heightdiff);
			ApproachSquares.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			GridNumbers.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			GridLetters.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			Quantum.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			Numpad.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			QuantumGridLines.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			QuantumGridSnap.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			Metronome.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			//ClickToPlace.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			SeparateClickTools.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			TrackHeight.ClientRectangle.Size = new SizeF(32 * widthdiff, 256 * heightdiff);
			TrackCursorPos.ClientRectangle.Size = new SizeF(OptionsNav.ClientRectangle.Width, 32 * heightdiff);
			ApproachRate.ClientRectangle.Size = TrackHeight.ClientRectangle.Size;

			//patterns
			HFlip.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			VFlip.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			BezierStoreButton.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			BezierClearButton.ClientRectangle.Size = UseCurrentMs.ClientRectangle.Size;
			//DynamicBezier.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			CurveBezier.ClientRectangle.Size = Autoplay.ClientRectangle.Size;
			BezierBox.ClientRectangle.Size = Offset.ClientRectangle.Size;
			BezierButton.ClientRectangle.Size = Offset.ClientRectangle.Size;
			RotateBox.ClientRectangle.Size = Offset.ClientRectangle.Size;
			RotateButton.ClientRectangle.Size = Offset.ClientRectangle.Size;
			ScaleBox.ClientRectangle.Size = Offset.ClientRectangle.Size;
			ScaleButton.ClientRectangle.Size = Offset.ClientRectangle.Size;

			//colors
			CreateColorset.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 1.5f * widthdiff, Offset.ClientRectangle.Height * 0.8f);
			OpenColorset.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			ExportColorset.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			VisualizeColors.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			ManageLayers.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 3.08f * widthdiff, Offset.ClientRectangle.Height);
			LayerPicker.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 1f * widthdiff, Offset.ClientRectangle.Height);
			LayerWarning.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			AlternatePicker.ClientRectangle.Size = LayerPicker.ClientRectangle.Size;
			ColorPicker.ClientRectangle.Size = LayerPicker.ClientRectangle.Size;
			ColorR.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 0.64f * widthdiff, Offset.ClientRectangle.Height);
			ColorG.ClientRectangle.Size = ColorR.ClientRectangle.Size;
			ColorB.ClientRectangle.Size = ColorR.ClientRectangle.Size;
			ColorRSlider.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 2f * widthdiff, Offset.ClientRectangle.Height * 1.2f);
			ColorGSlider.ClientRectangle.Size = ColorRSlider.ClientRectangle.Size;
			ColorBSlider.ClientRectangle.Size = ColorRSlider.ClientRectangle.Size;
			AddAlternate.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			AddColor.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			ChangeColor.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			DeleteColor.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			DeleteAlternate.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			ConfirmDelete.ClientRectangle.Size = CreateColorset.ClientRectangle.Size;
			ColorHex.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Width * 1.2f * widthdiff, Offset.ClientRectangle.Height * 0.8f);
			SetNotes.ClientRectangle.Size = ConfirmDelete.ClientRectangle.Size;
			SetColor.ClientRectangle.Size = ConfirmDelete.ClientRectangle.Size;
			SetComparison.ClientRectangle.Size = ConfirmDelete.ClientRectangle.Size;
			ReverseSelection.ClientRectangle.Size = new SizeF(Offset.ClientRectangle.Height * 0.8f * widthdiff, Offset.ClientRectangle.Height * 0.8f);
			ShiftLevel.ClientRectangle.Size = LayerPicker.ClientRectangle.Size;
			ShiftDefault.ClientRectangle.Size = LayerPicker.ClientRectangle.Size;
			ApplyShift.ClientRectangle.Size = LayerPicker.ClientRectangle.Size;

			//etc
			JumpMSButton.ClientRectangle.Size = new SizeF(192 * widthdiff, 40 * heightdiff);
			SfxOffset.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			JumpMSBox.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			AutoAdvance.ClientRectangle.Size = Autoplay.ClientRectangle.Size;

			MSBoundLower.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			MSBoundHigher.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			SelectBound.ClientRectangle.Size = JumpMSButton.ClientRectangle.Size;
			

			OptionsNav.ClientRectangle.Location = new PointF(10 * widthdiff, Track.ClientRectangle.Bottom + 60);
			TimingNav.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, OptionsNav.ClientRectangle.Bottom + 10 * heightdiff);
			PatternsNav.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, TimingNav.ClientRectangle.Bottom + 10 * heightdiff);
			ColorsNav.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, PatternsNav.ClientRectangle.Bottom + 10 * heightdiff);

			//timing
			Offset.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, TimingNav.ClientRectangle.Bottom + 40 * heightdiff);
			UseCurrentMs.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, Offset.ClientRectangle.Bottom + 10 * heightdiff);
			OpenTimings.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, UseCurrentMs.ClientRectangle.Bottom + 10 * heightdiff);
			OpenBookmarks.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, OpenTimings.ClientRectangle.Bottom + 10 * heightdiff);

			//options
			Autoplay.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, OptionsNav.ClientRectangle.Bottom + 20 * heightdiff);
			ApproachSquares.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Autoplay.ClientRectangle.Bottom + 10 * heightdiff);
			GridNumbers.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ApproachSquares.ClientRectangle.Bottom + 10 * heightdiff);
			GridLetters.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, GridNumbers.ClientRectangle.Bottom + 10 * heightdiff);
			Quantum.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, GridLetters.ClientRectangle.Bottom + 10 * heightdiff);
			Numpad.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Quantum.ClientRectangle.Bottom + 10 * heightdiff);
			QuantumGridLines.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Numpad.ClientRectangle.Bottom + 10 * heightdiff);
			QuantumGridSnap.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, QuantumGridLines.ClientRectangle.Bottom + 10 * heightdiff);
			Metronome.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, QuantumGridSnap.ClientRectangle.Bottom + 10 * heightdiff);
			//ClickToPlace.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Metronome.ClientRectangle.Bottom + 10 * heightdiff);
			SeparateClickTools.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, Metronome.ClientRectangle.Bottom + 10 * heightdiff);
			TrackHeight.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.Right - TrackHeight.ClientRectangle.Width, SeparateClickTools.ClientRectangle.Bottom + 50 * heightdiff - TrackHeight.ClientRectangle.Height);
			TrackCursorPos.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, SeparateClickTools.ClientRectangle.Bottom + 36 * heightdiff);
			ApproachRate.ClientRectangle.Location = new PointF(TrackHeight.ClientRectangle.X, Quantum.ClientRectangle.Bottom + 10 * heightdiff - TrackHeight.ClientRectangle.Height);

			//patterns
			HFlip.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, PatternsNav.ClientRectangle.Bottom + 20 * heightdiff);
			VFlip.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, HFlip.ClientRectangle.Bottom + 10 * heightdiff);
			BezierStoreButton.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, VFlip.ClientRectangle.Bottom + 20 * heightdiff);
			BezierClearButton.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierStoreButton.ClientRectangle.Bottom + 10 * heightdiff);
			//DynamicBezier.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierClearButton.ClientRectangle.Bottom + 10 * heightdiff);
			CurveBezier.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierClearButton.ClientRectangle.Bottom + 10 * heightdiff);
			BezierBox.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, CurveBezier.ClientRectangle.Bottom + 32 * heightdiff);
			BezierButton.ClientRectangle.Location = new PointF(BezierBox.ClientRectangle.Right + 5 * widthdiff, BezierBox.ClientRectangle.Y);
			RotateBox.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, BezierBox.ClientRectangle.Bottom + 35 * heightdiff);
			RotateButton.ClientRectangle.Location = new PointF(RotateBox.ClientRectangle.Right + 5 * widthdiff, RotateBox.ClientRectangle.Y);
			ScaleBox.ClientRectangle.Location = new PointF(Offset.ClientRectangle.X, RotateBox.ClientRectangle.Bottom + 35 * heightdiff);
			ScaleButton.ClientRectangle.Location = new PointF(RotateBox.ClientRectangle.Right + 5 * widthdiff, ScaleBox.ClientRectangle.Y);

			//colors
			CreateColorset.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ColorsNav.ClientRectangle.Bottom + 40 * heightdiff);
			OpenColorset.ClientRectangle.Location = new PointF(CreateColorset.ClientRectangle.X + CreateColorset.ClientRectangle.Size.Width + 10, ColorsNav.ClientRectangle.Bottom + 40 * heightdiff);
			ExportColorset.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, CreateColorset.ClientRectangle.Bottom + 8 * heightdiff);
			VisualizeColors.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X, CreateColorset.ClientRectangle.Bottom + 8 * heightdiff);
			ManageLayers.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ExportColorset.ClientRectangle.Bottom + 8 * heightdiff);
			LayerPicker.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ManageLayers.ClientRectangle.Bottom + 38 * heightdiff);
			LayerWarning.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, CreateColorset.ClientRectangle.Bottom + 8 * heightdiff);
			AlternatePicker.ClientRectangle.Location = new PointF(LayerPicker.ClientRectangle.X + 133, ManageLayers.ClientRectangle.Bottom + 38 * heightdiff);
			ColorPicker.ClientRectangle.Location = new PointF(AlternatePicker.ClientRectangle.X + 133, ManageLayers.ClientRectangle.Bottom + 38 * heightdiff);
			ColorR.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X + 312, LayerPicker.ClientRectangle.Bottom + 8 * heightdiff);
			ColorG.ClientRectangle.Location = new PointF(ColorR.ClientRectangle.X, ColorR.ClientRectangle.Bottom + 8 * heightdiff);
			ColorB.ClientRectangle.Location = new PointF(ColorG.ClientRectangle.X, ColorG.ClientRectangle.Bottom + 8 * heightdiff);
			ColorRSlider.ClientRectangle.Location = new PointF(ColorR.ClientRectangle.X - 250, LayerPicker.ClientRectangle.Bottom + 5 * heightdiff);
			ColorGSlider.ClientRectangle.Location = new PointF(ColorG.ClientRectangle.X - 250, ColorRSlider.ClientRectangle.Bottom + 1 * heightdiff);
			ColorBSlider.ClientRectangle.Location = new PointF(ColorB.ClientRectangle.X - 250, ColorGSlider.ClientRectangle.Bottom + 1 * heightdiff);
			ChangeColor.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ColorB.ClientRectangle.Bottom + 8 * heightdiff);
			ColorHex.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X + 38, ColorB.ClientRectangle.Bottom + 8 * heightdiff);
			AddAlternate.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, ChangeColor.ClientRectangle.Bottom + 8 * heightdiff);
			AddColor.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, AddAlternate.ClientRectangle.Bottom + 8 * heightdiff);
			DeleteColor.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X, AddAlternate.ClientRectangle.Bottom + 8 * heightdiff);
			DeleteAlternate.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X, ChangeColor.ClientRectangle.Bottom + 8 * heightdiff);
			SetNotes.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, DeleteColor.ClientRectangle.Bottom + 8 * heightdiff);
			ConfirmDelete.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X, DeleteColor.ClientRectangle.Bottom + 8 * heightdiff);
			SetColor.ClientRectangle.Location = new PointF(OptionsNav.ClientRectangle.X, SetNotes.ClientRectangle.Bottom + 8 * heightdiff);
			SetComparison.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X, SetNotes.ClientRectangle.Bottom + 8 * heightdiff);
			ReverseSelection.ClientRectangle.Location = new PointF(OpenColorset.ClientRectangle.X, ColorB.ClientRectangle.Bottom + 8 * heightdiff);
			ShiftLevel.ClientRectangle.Location = new PointF(LayerPicker.ClientRectangle.X, SetColor.ClientRectangle.Bottom + 8 * heightdiff);
			ShiftDefault.ClientRectangle.Location = new PointF(AlternatePicker.ClientRectangle.X, SetColor.ClientRectangle.Bottom + 8 * heightdiff);
			ApplyShift.ClientRectangle.Location = new PointF(ColorPicker.ClientRectangle.X, SetColor.ClientRectangle.Bottom + 8 * heightdiff);

			//etc
			BackButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Bottom + 84 * heightdiff);
			CopyButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Y - CopyButton.ClientRectangle.Height - 75 * heightdiff);
            PlayButton.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X, Grid.ClientRectangle.Y - CopyButton.ClientRectangle.Height - 25 * heightdiff);
            playLabel.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X + 315 * widthdiff, Grid.ClientRectangle.Y - CopyButton.ClientRectangle.Height - 10 * heightdiff);
            noplayLabel.ClientRectangle.Location = new PointF(Grid.ClientRectangle.X + 315 * widthdiff, Grid.ClientRectangle.Y - CopyButton.ClientRectangle.Height - 10 * heightdiff);

            SfxOffset.ClientRectangle.Location = new PointF(SfxVolume.ClientRectangle.Left - SfxOffset.ClientRectangle.Width - 10 * widthdiff, Tempo.ClientRectangle.Top - SfxOffset.ClientRectangle.Height - 15 * heightdiff);
			JumpMSButton.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, SfxOffset.ClientRectangle.Top - JumpMSButton.ClientRectangle.Height - 30);
			JumpMSBox.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, JumpMSButton.ClientRectangle.Top - JumpMSBox.ClientRectangle.Height - 10 * heightdiff);
			AutoAdvance.ClientRectangle.Location = new PointF(BeatSnapDivisor.ClientRectangle.X + 20 * widthdiff, CopyButton.ClientRectangle.Y + 35 * heightdiff);
			SelectBound.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, JumpMSBox.ClientRectangle.Top - SelectBound.ClientRectangle.Height - 30);
			MSBoundHigher.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, SelectBound.ClientRectangle.Top - MSBoundHigher.ClientRectangle.Height - 10 * heightdiff);
			MSBoundLower.ClientRectangle.Location = new PointF(SfxOffset.ClientRectangle.X, MSBoundHigher.ClientRectangle.Top - MSBoundLower.ClientRectangle.Height - 10 * heightdiff);


			HideShowElements();
		}

		public void OnMouseLeave()
		{
			Timeline.Dragging = false;
			Tempo.Dragging = false;
			MasterVolume.Dragging = false;
			SfxVolume.Dragging = false;
			NoteAlign.Dragging = false;
			BeatSnapDivisor.Dragging = false;
			TrackHeight.Dragging = false;
			TrackCursorPos.Dragging = false;
			ApproachRate.Dragging = false;
			ColorRSlider.Dragging = false;
			ColorGSlider.Dragging = false;
			ColorBSlider.Dragging = false;
		}

		private void UpdateTrack()
		{
			/*
			if (Bpm.Focused)
			{
				var text = Bpm.Text;
				var decimalPont = false;

				if (text.Length > 0 && text[text.Length - 1].ToString() ==
				    CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
				{
					text = text + 0;

					decimalPont = true;
				}

				if (text.Contains(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator))
                {
					decimalPont = true;
                }

				decimal.TryParse(text, out var bpm);

				if (bpm < 0)
					bpm = 0;
				else if (bpm > 5000)
					bpm = 5000;
				if (!decimalPont && bpm > 0)
					Bpm.Text = bpm.ToString();
			}
			if (Offset.Focused)
			{
				long.TryParse(Offset.Text, out var offset);

				offset = Math.Max(0, offset);

				if (offset > 0)
					Offset.Text = offset.ToString();
			}
			if (SfxOffset.Focused)
			{
				if (long.TryParse(SfxOffset.Text, out var sfxOffset))
					SfxOffset.Text = sfxOffset.ToString();
			}
			if (JumpMSBox.Focused)
            {
				if (long.TryParse(JumpMSBox.Text, out var jumpMS))
					if (jumpMS > EditorWindow.Instance.totalTime.TotalMilliseconds)
						jumpMS = (long)EditorWindow.Instance.totalTime.TotalMilliseconds;
				if (jumpMS.ToString() != "0")
					JumpMSBox.Text = jumpMS.ToString();
			}
			if (RotateBox.Focused)
            {
				if (float.TryParse(RotateBox.Text, out var degrees))
					RotateBox.Text = degrees.ToString();
            }
			if (BezierBox.Focused)
            {
				if (long.TryParse(BezierBox.Text, out var ints))
					BezierBox.Text = ints.ToString();
            }
			*/
			foreach (var box in Boxes)
				box.Text = box.Text;
		}

		public void ShowToast(string text, Color color)
		{
			_toastTime = 0;

			_toast.Text = text;
			_toast.Color = color;
		}
	}
}