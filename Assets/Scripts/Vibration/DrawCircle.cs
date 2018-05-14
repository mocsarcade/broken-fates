using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawCircle : DrawShape {

	//Vibration display settings
	private CircleVibrationCollision colliderScript;

	public override void Awake()
	{
		base.Awake();
		_lineRenderer = GetComponent<LineRenderer>();
		colliderScript = GetComponent<CircleVibrationCollision>();
		edge = transform.position;
		time = 250;
		beginningTime = time;
	}

		public override void Initialize(int timer, GameObject _parent) {
			base.Initialize(timer, _parent);
			colliderScript.Initialize(_parent);
		}

	public override void UpdateShape(Vector2 newVertex)
	{
		_vertices[_vertices.Count - 1] = newVertex;

		// Update the mesh relative to the transform
		var v0Relative = Vector2.zero;
		var v1Relative = _vertices[1] - _vertices[0];
		Mesh circle = CircleMesh(v0Relative, v1Relative, FillColor);

		_lineRenderer.positionCount = circle.vertices.Length;
		_lineRenderer.SetPositions(circle.vertices);

		// Update the collider
		colliderScript.UpdateCircle(_vertices);
	}

	/// <summary>
	/// Creates and returns a circle mesh given two vertices on its center
	/// and any outer edge point.
	/// </summary>
	private static Mesh CircleMesh(Vector2 v0, Vector2 v1, Color fillColor)
	{
		var radius = Vector2.Distance(v0, v1);

		// We want to make sure that the circle appears to be curved.
		// This can be approximated by drawing a regular polygon with lots of segments.
		// The number of segments can be increased based on the radius so that large circles also appear curved.
		// We use an offset and multiplier to create a tunable linear function.
		const float segmentOffset = 40f;
		const float segmentMultiplier = 5f;
		var numSegments = (int) (radius * segmentMultiplier + segmentOffset);

		// Create an array of points arround a cricle
		var circleVertices = Enumerable.Range(0, numSegments)
			.Select(i => {
				var theta = 2 * Mathf.PI * i / numSegments;
				return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)) * radius;
			})
			.ToArray();

		// Find all the triangles in the shape
		var triangles = new Triangulator(circleVertices).Triangulate();

		// Assign each vertex the fill color
		var colors = Enumerable.Repeat(fillColor, circleVertices.Length).ToArray();

		var mesh = new Mesh {
			name = "Circle",
			vertices = circleVertices.ToVector3(),
			triangles = triangles,
			colors = colors
		};

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}

}
