using OpenTK.Graphics;
using System;
using System.Drawing;

namespace Sound_Space_Editor
{

    [Serializable]
	class Note
	{
		public float X;
		public float Y;
		public long Ms;
		public long DragStartMs;
		public bool Anchored;

		public Color4 Color;
		public int layer;
		public int shift;

		public Note(float x, float y, long ms, Color4 co)
        {
            X = x;
            Y = y;

            Ms = ms;
            Color = co;
			layer = 0;
			shift = 0;
        }

        public Note Clone()
		{
			return new Note(X, Y, Ms, Color);
		}
	}
}