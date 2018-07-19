using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawEllipse : DrawShape {

	//Vibration display settings
	private EllipseVibrationCollision colliderScript;

	public const int INVERSE_RING_SIZE = 4;
	public const float Y_MULTIPLIER = 1.5f;
	private const int WALL_LAYERMASK = 256;

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
			base.Initialize(timer/INVERSE_RING_SIZE, _parent);
			colliderScript.Initialize(_parent);
		}

	public override void UpdateShape(Vector2 newVertex)
	{
		_vertices[_vertices.Count - 1] = newVertex;

		// Update the mesh relative to the transform
		var v0Relative = Vector2.zero;
		var v1Relative = _vertices[1] - _vertices[0];

		float radiusx = Vector2.Distance(v0Relative, v1Relative);
		float radiusy = radiusx/Y_MULTIPLIER;
		Mesh ellipse = EllipseMesh(radiusx, radiusy, FillColor);

		//Check each piece of the array for if it is colliding with a wall, and turn it circular if it is
		Mesh w_ellipse = CheckWalls(ellipse.vertices.ToVector2(), FillColor);

		_lineRenderer.positionCount = w_ellipse.vertices.Length;
		_lineRenderer.SetPositions(w_ellipse.vertices);

		// Update the collider
		colliderScript.UpdateEllipse(w_ellipse.vertices);
	}

			/// <summary>
			/// Builds an elliptical mesh given both radii and uses CreateMesh method to create and return it
			/// </summary>
			private static Mesh EllipseMesh(float radiusx, float radiusy, Color fillColor)
			{
				var radius = radiusx;
				// We want to make sure that the ellipse appears to be curved.
				// This can be approximated by drawing a regular polygon with lots of segments.
				// The number of segments can be increased based on the radius so that large ellipses also appear curved.
				// We use an offset and multiplier to create a tunable linear function.
				// Decrease segmentMultiplier or segmentOffset to make vibration take less CPU time
				const float segmentOffset = 40f;
				const float segmentMultiplier = 4f;
				var numSegments = (int) (radius * segmentMultiplier + segmentOffset);

				// Create an array of points around the ellipse of size radiusx by radiusy
				var ellipseVertices = Enumerable.Range(0, numSegments)
					.Select(i => {
						var theta = 2 * Mathf.PI * i / numSegments;
						return new Vector2(Mathf.Cos(theta) * radiusx, Mathf.Sin(theta) * radiusy);
					})
					.ToArray();

				var mesh = CreateMesh(ellipseVertices, fillColor);

				return mesh;
			}

			/// <summary>
			/// Creates and returns a mesh given an array of Vector2s for each point on the mesh
			/// </summary>
			protected static Mesh CreateMesh(Vector2[] ellipseVertices, Color fillColor) {
				// Find all the triangles in the shape
				var triangles = new Triangulator(ellipseVertices).Triangulate();

				// Assign each vertex the fill color
				var colors = Enumerable.Repeat(fillColor, ellipseVertices.Length).ToArray();

				var mesh = new Mesh {
					name = "Ellipse",
					vertices = ellipseVertices.ToVector3(),
					//triangles = triangles,
					colors = colors
				};

				mesh.RecalculateNormals();
				mesh.RecalculateBounds();

				return mesh;
			}

			protected Mesh CheckWalls(Vector2[] ellipseVertices, Color fillColor) {
				Collider2D result;
				for(int i=0; i<ellipseVertices.Length; i++) {
					Vector2 vertex = ellipseVertices[i];
					//Checks if there is a wall within 0.1 units of this segement, and returns null if none exists
					result = Physics2D.OverlapCircle(vertex + (Vector2) transform.position, 0.02f, WALL_LAYERMASK);
					if(result != null) {
						//Set distToEnd according to wall side closest to vibration point (left, right or down)
						float distToEnd = Mathf.Min(
							Mathf.Min(
								//Left and Right sides
								Vector2.Distance(vertex + (Vector2) transform.position, new Vector2(result.bounds.center.x + result.bounds.extents.x, vertex.y + transform.position.y)),
								Vector2.Distance(vertex + (Vector2) transform.position, new Vector2(result.bounds.center.x - result.bounds.extents.x, vertex.y + transform.position.y))),
							//Bottom side
							Vector2.Distance(vertex + (Vector2) transform.position, new Vector2(vertex.x + transform.position.x, result.bounds.center.y - result.bounds.extents.y))
						);

						//If the increased amount is over the top of the wall, just set it equal to the top of the wall
						if((transform.position.y > result.bounds.max.y) || (transform.position.y + vertex.y + (distToEnd*Y_MULTIPLIER) > result.bounds.max.y) ) {
							//The small increase is for varaition, as meshes cannot be drawn with too many of the exact same point on the line
							ellipseVertices[i] = new Vector2(vertex.x, result.bounds.max.y - transform.position.y - 0.0000001f*i);
						} else {
							//Add to vertex.y by distToSide*Y_MULTIPLIER
							ellipseVertices[i] = new Vector2(vertex.x, vertex.y + (distToEnd*Y_MULTIPLIER));//result.bounds.min.y-transform.position.y+(vertex.y+transform.position.y-result.bounds.min.y)*Y_MULTIPLIER);
						}

						int beforeIndex = i-1;
						if(beforeIndex < 0) {
							beforeIndex = ellipseVertices.Length-1;
						}
						int afterIndex = i+1;
						if(afterIndex > ellipseVertices.Length-1) {
							afterIndex = 0;
						}
						//Check if point before is to the left of wall's left end
						if(ellipseVertices[beforeIndex].x + transform.position.x < result.bounds.min.x) {
							//set X of right point equal to wall's left end
							ellipseVertices[beforeIndex] = new Vector2(result.bounds.min.x-transform.position.x, ellipseVertices[beforeIndex].y);
						}
						//Check if point after is to the right of wall's right end
						if(ellipseVertices[afterIndex].x + transform.position.x > result.bounds.max.x) {
							//set X of next point equal to wall's right end
							ellipseVertices[afterIndex] = new Vector2(result.bounds.max.x-transform.position.x, ellipseVertices[afterIndex].y);
							//skip next point
							i++;
						}
					}
				}
				return CreateMesh(ellipseVertices, fillColor);
			}

}
