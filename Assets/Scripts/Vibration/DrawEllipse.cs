using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawEllipse : DrawShape {

	//Vibration display settings
	private EllipseVibrationCollision colliderScript;

	public override void Awake()
	{
		base.Awake();
		_lineRenderer = GetComponent<LineRenderer>();
		colliderScript = GetComponent<EllipseVibrationCollision>();
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

		float radiusx = Vector2.Distance(v0Relative, v1Relative);
		float radiusy = radiusx/1.5f;
		Mesh ellipse = EllipseMesh(radiusx, radiusy, FillColor);

		_lineRenderer.positionCount = ellipse.vertices.Length;
		_lineRenderer.SetPositions(ellipse.vertices);

		// Update the collider
		colliderScript.UpdateEllipse(ellipse.vertices);
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
