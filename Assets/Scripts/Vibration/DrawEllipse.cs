using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawEllipse : MonoBehaviour {

	//Vibration display settings
	public Color FillColor = Color.white;
	public const float RING_SPEED = 0.03f;
	public const float BEGINNING_THICKNESS = 0.05f;
	private LineRenderer _lineRenderer;
	private EllipseVibrationCollision colliderScript;
	//Position
	private Vector2 edge;

	//Vibration Lastability settings
	public int time;
	private int beginningTime;

	//Object that made this vibration
	private GameObject parent;

	// Start and end vertices (in absolute coordinates)
	private readonly List<Vector2> _vertices = new List<Vector2>(2);

	public bool ShapeFinished { get { return _vertices.Count >= 2; } }

	private void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		colliderScript = GetComponent<EllipseVibrationCollision>();
		edge = transform.position;
		time = 250;
		beginningTime = time;
	}

	public void Initialize(int timer, GameObject _parent) {
		time = timer;
		beginningTime = time;
		parent = _parent;
		colliderScript.Initialize(_parent);
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

	public void AddVertex(Vector2 vertex)
	{
		if (ShapeFinished) {
			UpdateShape(vertex);
			return;
		}

		_vertices.Add(vertex);
	}

	public void UpdateShape(Vector2 newVertex)
	{
		_vertices[_vertices.Count - 1] = newVertex;

		// Update the mesh relative to the transform
		var v0Relative = Vector2.zero;
		var v1Relative = _vertices[1] - _vertices[0];

		float radiusx = Vector2.Distance(v0Relative, v1Relative);
		float radiusy = 2*radiusx;
		Mesh ellipse = EllipseMesh(radiusx, radiusy, FillColor);

		_lineRenderer.positionCount = ellipse.vertices.Length;
		_lineRenderer.SetPositions(ellipse.vertices);

		// Update the collider
		colliderScript.UpdateEllipse(_vertices);
	}

			/// <summary>
			/// Creates and returns an elliptical mesh given both radii
			/// </summary>
			private static Mesh EllipseMesh(float radiusx, float radiusy, Color fillColor)
			{
				var radius = radiusx;
				// We want to make sure that the ellipse appears to be curved.
				// This can be approximated by drawing a regular polygon with lots of segments.
				// The number of segments can be increased based on the radius so that large ellipses also appear curved.
				// We use an offset and multiplier to create a tunable linear function.
				const float segmentOffset = 40f;
				const float segmentMultiplier = 5f;
				var numSegments = (int) (radius * segmentMultiplier + segmentOffset);

				// Create an array of points arround a cricle
				var ellipseVertices = Enumerable.Range(0, numSegments)
					.Select(i => {
						var theta = 2 * Mathf.PI * i / numSegments;
						return new Vector2(Mathf.Cos(theta) * radiusx, Mathf.Sin(theta) * radiusy);
					})
					.ToArray();

				// Find all the triangles in the shape
				var triangles = new Triangulator(ellipseVertices).Triangulate();

				// Assign each vertex the fill color
				var colors = Enumerable.Repeat(fillColor, ellipseVertices.Length).ToArray();

				var mesh = new Mesh {
					name = "Ellipse",
					vertices = ellipseVertices.ToVector3(),
					triangles = triangles,
					colors = colors
				};

				mesh.RecalculateNormals();
				mesh.RecalculateBounds();

				return mesh;
			}

}

/*
public static class Util
{
	/// <summary>
	/// Extension that converts an array of Vector2 to an array of Vector3
	/// </summary>
	public static Vector3[] ToVector3(this Vector2[] vectors)
	{
		return System.Array.ConvertAll<Vector2, Vector3>(vectors, v => v);
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
*/
