﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using SSQE_Player.GUI;
using System.Reflection;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using SSQE_Player.Models;
using OpenTK.Graphics;

namespace SSQE_Player
{
    internal class MainWindow : GameWindow
    {
        public static MainWindow Instance;
        public MusicPlayer MusicPlayer = new();
        public SoundPlayer SoundPlayer = new();
        public ModelManager ModelManager = new();

        public List<Note> Notes = new();

        public static readonly Vector3 NoteSize = Vector3.One * 0.875f;
        public static readonly Vector3 CursorSize = new(0.275f, 0.275f, 0f);
        public Vector3 CursorPos = new();
        public Vector3 LastCursorPos = new();

        public GuiWindowMain CurrentWindow;
        public Camera Camera = new();

        public float StartTime;
        public float Tempo;

        private bool isFullscreen = false;
        private Vector2i startSize;

        private unsafe void SetInputMode()
        {
            GLFW.SetInputMode(WindowPtr, RawMouseMotionAttribute.RawMouseMotion, true);
        }

        public MainWindow(bool fromStart) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (1920, 1080),
            Title = $"Sound Space Map Player {Assembly.GetExecutingAssembly().GetName().Version}",
            NumberOfSamples = 32,
            WindowState = WindowState.Fullscreen
        })
        {
            startSize = Size;
            SwitchFullscreen();

            VSync = VSyncMode.Off;

            CursorState = CursorState.Grabbed;
            SetInputMode();

            Shader.Init();
            FontRenderer.Init();

            Settings.Load();

            Instance = this;

            if (Settings.settings["limitPlayerFPS"])
            {
                if (Settings.settings["useVSync"])
                    VSync = VSyncMode.On;
                else
                {
                    var fps = Settings.settings["fpsLimit"].Value;
                    var max = Settings.settings["fpsLimit"].Max;

                    RenderFrequency = Math.Round(fps) == Math.Round(max) ? 0f : fps + 60f;
                    UpdateFrequency = RenderFrequency;
                }
            }

            if (!Settings.settings["fullscreenPlayer"])
                SwitchFullscreen();

            if (fromStart)
                Settings.settings["currentTime"].Value = 0f;
            StartTime = Settings.settings["currentTime"].Value;

            var startIndex = LoadMap(File.ReadAllText("assets/temp/tempmap.txt"));
            RegisterModels();

            MusicPlayer.Volume = Settings.settings["masterVolume"].Value;
            SoundPlayer.Volume = Settings.settings["sfxVolume"].Value;

            SetTempo(Settings.settings["tempo"].Value);

            Camera = new();
            CurrentWindow = new(startIndex);
        }

        private Vector2 MouseDelta = new();

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MouseDelta += MouseState.Delta;
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            if (MusicPlayer.IsPlaying)
                Settings.settings["currentTime"].Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;

            LastCursorPos = CursorPos;

            Camera.Update(MouseDelta.X, MouseDelta.Y);
            CurrentWindow?.Render((float)args.Time);

            MouseDelta = new();

            SwapBuffers();
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 1f);

            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            var w = Math.Max(e.Width, 1280);
            var h = Math.Max(e.Height, 720);
            Size = new Vector2i(w, h);

            GL.Viewport(0, 0, w, h);
            base.OnResize(new ResizeEventArgs(w, h));

            Shader.UploadOrtho(Shader.FontTexProgram, w, h);

            CurrentWindow?.OnResize(Size);

            if (Instance == null)
                return;

            Camera.CalculateProjection();
        }

        private void SwitchFullscreen()
        {
            isFullscreen ^= true;

            WindowState = isFullscreen ? WindowState.Normal : WindowState.Maximized;
            WindowBorder = isFullscreen ? WindowBorder.Hidden : WindowBorder.Resizable;

            if (isFullscreen)
            {
                Size = startSize;
                Location = (0, 0);
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Escape:
                case Keys.F4 when e.Alt:
                    Close();
                    break;

                case Keys.F11:
                    SwitchFullscreen();
                    break;

                case Keys.R:
                    CurrentWindow.Resetting = true;
                    break;

                case Keys.Tab:
                    CurrentWindow.Reset();
                    break;

                case Keys.Space:
                    if (CurrentWindow.Paused && !CurrentWindow.Unpausing)
                    {
                        Settings.settings["currentTime"].Value = CurrentWindow.PauseTime - 750f;
                        MusicPlayer.Play();
                        CurrentWindow.Unpausing = true;
                    }
                    else if (!CurrentWindow.Unpausing && MusicPlayer.IsPlaying && CurrentWindow.PauseTime + 1500f <= Settings.settings["currentTime"].Value)
                    {
                        MusicPlayer.Pause();

                        CurrentWindow.PauseTime = Settings.settings["currentTime"].Value;
                        CurrentWindow.Pauses++;
                        CurrentWindow.Paused = true;
                    }
                    break;

                case Keys.U when e.Control:
                    CursorState = CursorState.Normal;
                    break;
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.Button == MouseButton.Left && IsFocused)
                CursorState = CursorState.Grabbed;
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.R:
                    CurrentWindow.Resetting = false;
                    break;

                case Keys.Space:
                    if (CurrentWindow.Paused)
                    {
                        MusicPlayer.Pause();
                        Settings.settings["currentTime"].Value = CurrentWindow.PauseTime;
                    }

                    CurrentWindow.Unpausing = false;
                    break;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            CurrentWindow.Offset += (int)e.OffsetY;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MusicPlayer.Dispose();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            CursorState = IsFocused ? CursorState.Grabbed : CursorState.Normal;
        }




        private int LoadMap(string data)
        {
            var currentTime = Settings.settings["currentTime"].Value;
            var noteColors = Settings.settings["noteColors"];
            var colorCount = noteColors.Count;

            var startIndex = 0;

            var split = data.Split(',');

            try
            {
                var id = split[0];

                for (int i = 1; i < split.Length; i++)
                {
                    var subsplit = split[i].Split('|');

                    var x = float.Parse(subsplit[0]);
                    var y = float.Parse(subsplit[1]);
                    var ms = long.Parse(subsplit[2]);

                    if (ms < currentTime)
                        startIndex++;

                    Notes.Add(new Note(x, y, ms, noteColors[i % colorCount]));
                }

                MusicPlayer.Load($"cached/{id}.asset");
            }
            catch
            {
                CursorState = CursorState.Normal;
                Close();
            }

            return startIndex;
        }

        private void RegisterModels()
        {
            var noteModel = ObjModel.FromFile("assets/models/note.obj");
            var cursorModel = ObjModel.FromFile("assets/models/cursor.obj");

            ModelManager.RegisterModel("note", noteModel.GetVertices(), Settings.settings["noteScale"]);
            ModelManager.RegisterModel("cursor", cursorModel.GetVertices(), Settings.settings["cursorScale"]);
        }

        private void SetTempo(float newTempo)
        {
            var tempoA = Math.Min(newTempo, 0.9f);
            var tempoB = (newTempo - tempoA) * 2f;

            Tempo = tempoA + tempoB + 0.1f;
            MusicPlayer.Tempo = Tempo;
        }
    }
}
