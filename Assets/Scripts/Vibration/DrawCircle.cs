using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DrawCircle : MonoBehaviour {

	public Color FillColor = Color.white;
	public const float RING_SPEED = 0.03f;
	public const float BEGINNING_THICKNESS = 0.05f;

	public int time;
	private int beginningTime;
	private LineRenderer _lineRenderer;
	private CircleCollider2D _circleCollider2D;
	private Vector2 edge;

	// Start and end vertices (in absolute coordinates)
	private readonly List<Vector2> _vertices = new List<Vector2>(2);

	public bool ShapeFinished { get { return _vertices.Count >= 2; } }

	private void Awake()
	{
		_lineRenderer = GetComponent<LineRenderer>();
		_circleCollider2D = GetComponent<CircleCollider2D>();
		edge = transform.position;
		time = 250;
		beginningTime = time;
	}

	public void Initialize(int timer) {
		time = timer;
		beginningTime = time;
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
		Mesh circle = CircleMesh(v0Relative, v1Relative, FillColor);

		_lineRenderer.positionCount = circle.vertices.Length;
		_lineRenderer.SetPositions(circle.vertices);

		// Update the collider
		_circleCollider2D.radius = Vector2.Distance(_vertices[0], _vertices[1]);
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

	/*
	// Use this for initialization
	void Start () {
		//Initialize RigidBody, which will move the object when it is moved
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		this.MakeCircle (count);
	}

	public void MakeCircle(int numOfPoints)
	{
		float angleStep = 360.0f / (float)numOfPoints;
		List<Vector3> vertexList = new List<Vector3>();
		List<int> triangleList = new List<int>();
		Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
		// Make first triangle.
		vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.
		vertexList.Add(new Vector3(0.0f, 10.0f, 0.0f));  // 2. First vertex on circle outline (radius = 0.5f)
		vertexList.Add(quaternion * vertexList[1]);     // 3. First vertex on circle outline rotated by angle)
		// Add triangle indices.
		triangleList.Add(0);
		triangleList.Add(1);
		triangleList.Add(2);
		for (int i = 0; i < numOfPoints - 1; i++)
		{
			triangleList.Add(0);                      // Index of circle center.
			triangleList.Add(vertexList.Count - 1);
			triangleList.Add(vertexList.Count);
			vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
		}

		// Assign each vertex the fill color
		var colors = Enumerable.Repeat(fillColor, rectangleVertices.Length).ToArray();

		Mesh mesh = new Mesh();
		mesh.vertices = vertexList.ToArray();
		mesh.triangles = triangleList.ToArray();


		// Set up game object with mesh;
		var meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = new Material(Shader.Find("Sprites/Default"));

		var filter = gameObject.AddComponent<MeshFilter>();
		filter.mesh = mesh;
	}*/

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