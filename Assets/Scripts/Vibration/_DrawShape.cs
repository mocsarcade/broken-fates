using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class _DrawShape : MonoBehaviour {

		//Vibration display settings
		public Color FillColor = Color.white;
		protected float RING_SPEED = 0.04f;//0.12f;
		protected const float BEGINNING_THICKNESS = 0.04f;
		protected int INVERSE_RING_LIFE = 1;//3;
		protected LineRenderer _lineRenderer;

		//Position
		protected Vector2 edge;

		//Vibration Lastability settings
		public int time;
		protected int beginningTime;

		//Object that made this vibration
		protected GameObject parent;

		// Start and end vertices (in absolute coordinates)
		protected readonly List<Vector2> _vertices = new List<Vector2>(2);

		public bool ShapeFinished { get { return _vertices.Count >= 2; } }

		public virtual void Awake() {
		}

			private void FixedUpdate()
			{
				edge = edge + new Vector2(RING_SPEED,0);
				AddVertex (edge);
				time--;
				if (time > 0) {
					_lineRenderer.startWidth = time * BEGINNING_THICKNESS/ beginningTime;
					_lineRenderer.endWidth = time * BEGINNING_THICKNESS/ beginningTime;
				}
				else {
					Destroy(gameObject);
				}
			}

			public virtual void Initialize(int timer, GameObject _parent) {
				time = timer/INVERSE_RING_LIFE;
				beginningTime = time;
				parent = _parent;
			}

			public void AddVertex(Vector2 vertex)
			{
				if (ShapeFinished) {
					UpdateShape(vertex);
					return;
				}

				_vertices.Add(vertex);
			}

			public virtual void UpdateShape(Vector2 newVertex) {}
}


public static class Util
{
	/// <summary>
	/// Extension that converts an array of Vector2 to an array of Vector3
	/// </summary>
	public static Vector3[] ToVector3(this Vector2[] vectors)
	{
		return System.Array.ConvertAll<Vector2, Vector3>(vectors, v => v);
	}
	public static Vector2[] ToVector2(this Vector3[] vectors)
	{
		return System.Array.ConvertAll<Vector3, Vector2>(vectors, v => v);
	}

	/// <summary>
	/// Extension that, given a collection of vectors, returns a centroid
	/// (i.e., an average of all vectors)
	/// </summary>
	public static Vector2 Centroid(this ICollection<Vector2> vectors)
	{
		return vectors.Aggregate((agg, next) => agg + next) / vectors.Count();
	}

	/// <summary>
	/// Extension returning the absolute value of a vector
	/// </summary>
	public static Vector2 Abs(this Vector2 vector)
	{
		return new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
	}
}
