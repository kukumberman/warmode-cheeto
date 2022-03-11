using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cheeto
{
	public static class Drawing
	{
		// becames null when scene reloads? (or why it doesnot draw lines)
		private static Texture2D LineTexture = new Texture2D(1, 1);

		public static void SolidBox(Vector2 position, Vector2 size, bool centered = true)
		{
			Vector2 upperLeft = centered ? position - size / 2f : position;
			GUI.DrawTexture(new Rect(upperLeft, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
		}

		public static void Text(Vector2 position, string label, GUIStyle style, bool centered = true)
		{
			var content = new GUIContent(label);
			var size = style.CalcSize(content);
			var upperLeft = centered ? position - size / 2f : position;
			GUI.Label(new Rect(upperLeft, size), content, style);
		}

		public static void Line(Vector2 a, Vector2 b, float thickness)
		{
			if (LineTexture == null)
			{
				LineTexture = new Texture2D(1, 1);
			}

			Matrix4x4 matrix = GUI.matrix;

			float angle = Vector3.Angle(b - a, Vector2.right);

			if (a.y > b.y)
			{
				angle = -angle;
			}

			GUIUtility.ScaleAroundPivot(new Vector2((b - a).magnitude, thickness), new Vector2(a.x, a.y + 0.5f));
			GUIUtility.RotateAroundPivot(angle, a);
			GUI.DrawTexture(new Rect(a.x, a.y, 1, 1), LineTexture);
			GUI.matrix = matrix;
		}

		public static void Box(Vector2 position, Vector2 size, float thickness, bool centered = true)
		{
			Vector2 upperLeft = centered ? position - size / 2f : position;
			Box(upperLeft.x, upperLeft.y, size.x, size.y, thickness);
		}

		public static void Box(float x, float y, float w, float h, float thickness)
		{
			Vector2 a = new Vector2(x, y);
			Vector2 b = new Vector2(x + w, y);
			Vector2 c = new Vector2(x + w, y + h);
			Vector2 d = new Vector2(x, y + h);

			Line(a, b, thickness);
			Line(b, c, thickness);
			Line(c, d, thickness);
			Line(d, a, thickness);
		}

		public static void Circle(Vector2 center, float radius, float thickness)
		{
			const int count = 32;
			float step = 360f / count;
			const int vertexCount = count;

			Vector2[] points = new Vector2[vertexCount];

			for (int i = 0; i < points.Length; i++)
			{
				float angle = i * step;
				float rad = angle * Mathf.Deg2Rad;
				float x = center.x + Mathf.Sin(rad) * radius;
				float y = center.y + Mathf.Cos(rad) * radius;

				points[i] = new Vector2(x, y);
			}

			for (int i = 0; i < points.Length; i++)
			{
				Vector2 a = points[i];
				Vector2 b = points[(i + 1) % points.Length];
				Line(a, b, thickness);
			}
		}
	}
}
