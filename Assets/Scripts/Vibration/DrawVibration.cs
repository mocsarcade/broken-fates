	using UnityEngine;

	[RequireComponent(typeof(LineRenderer))]

	public class DrawVibration : MonoBehaviour
	{

			//Ring growth settings
			public const float Y_MULTIPLIER = 1.5f;
			protected const float BEGINNING_THICKNESS = 0.04f;

			//Vibration collision
			protected VibrationCollision _colliderScript;
			protected const int WALL_LAYERMASK = 256;

			//Vibration Lastability settings
			public int time;
			protected int beginningTime;

			//Object that made this vibration
			protected GameObject parent;

			public enum Axis { X, Y, Z };

			[SerializeField]
			[Tooltip("The speed the circle grows. Increase for it to grow faster.")]
			[Range(0f, 0.25f)]
			private float _ringSpeed = 0.06f;

			[SerializeField]
			[Tooltip("This value is inversely related to the life of the ring. Increase to make ring die faster")]
			[Range(0, 20)]
			private int _inverseRingLife = 7;

			[SerializeField]
			[Tooltip("The number of lines that will be used to draw the circle. The more lines, the more the circle will be \"flexible\".")]
			[Range(0, 1000)]
			private int _segments = 60;

			[SerializeField]
			[Tooltip("The radius of the horizontal axis.")]
			private float _horizRadius = 0.01f;

			[SerializeField]
			[Tooltip("The radius of the vertical axis.")]
			private float _vertRadius = 0.01f;

			[SerializeField]
			[Tooltip("The axis about which the circle is drawn.")]
			private Axis _axis = Axis.Z;

			[SerializeField]
			[Tooltip("If checked, the circle will be rendered again each time one of the parameters change.")]
			private bool _checkValuesChanged = true;

			[SerializeField]
			[Tooltip("If checked, the circle will be affected by walls and will look like it slopes up them.")]
			private bool _climbWalls = true;

			private int _previousSegmentsValue;
			private float _previousHorizRadiusValue;
			private float _previousVertRadiusValue;
			private Axis _previousAxisValue;

			private LineRenderer _line;

			void OnEnable() {
				_line = gameObject.GetComponent<LineRenderer>();
				_colliderScript = GetComponent<VibrationCollision>();
			}

			void Start()
			{
					//_line.SetVertexCount(_segments + 1);
					_line.positionCount = _segments + 1;
					_line.useWorldSpace = false;

					UpdateValuesChanged();

					CreatePoints();
			}

			public virtual void Initialize(int timer, GameObject _parent) {
				_colliderScript.Initialize(_parent);
				time = timer/_inverseRingLife;
				beginningTime = time;
				parent = _parent;
			}

			private void FixedUpdate()
			{
				_horizRadius += _ringSpeed;
				_vertRadius += _ringSpeed;
				time--;
				if (time > 0) {
					_line.startWidth = time * BEGINNING_THICKNESS/ beginningTime;
					_line.endWidth = time * BEGINNING_THICKNESS/ beginningTime;
				}
				else {
					Vibration.Vibrator().RemoveVibration(gameObject);
				}
			}

			void Update()
			{
					if (_checkValuesChanged)
					{
							if (_previousSegmentsValue != _segments ||
									_previousHorizRadiusValue != _horizRadius ||
									_previousVertRadiusValue != _vertRadius ||
									_previousAxisValue != _axis)
							{
									CreatePoints();
							}

							UpdateValuesChanged();
					}
			}

			void UpdateValuesChanged()
			{
					_previousSegmentsValue = _segments;
					_previousHorizRadiusValue = _horizRadius;
					_previousVertRadiusValue = _vertRadius;
					_previousAxisValue = _axis;
			}

			void CreatePoints()
			{
					Collider2D result;
					if (_previousSegmentsValue != _segments)
					{
							//_line.SetVertexCount(_segments + 1);
							_line.positionCount = _segments + 1;
					}

					float x;
					float y;
					float z = 0;

					float angle = 0f;

					for (int i = 0; i < (_segments + 1); i++)
					{
							x = Mathf.Sin(Mathf.Deg2Rad * angle) * _horizRadius;
							y = Mathf.Cos(Mathf.Deg2Rad * angle) * _vertRadius / Y_MULTIPLIER;

							if(_climbWalls) {
								//Update Y depending on whether colliding with a wall
								Vector2 vertex = new Vector2(x, y);
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
										y = result.bounds.max.y - transform.position.y - 0.0000001f*i;
									} else {
										//Add to vertex.y by distToSide*Y_MULTIPLIER to make the "climbing wall" effect
										y = vertex.y + (distToEnd*Y_MULTIPLIER);
									}
								}
							}

							switch(_axis)
							{
									case Axis.X: _line.SetPosition(i, new Vector3(z, y, x));
											break;
									case Axis.Y: _line.SetPosition(i, new Vector3(y, z, x));
											break;
									case Axis.Z: _line.SetPosition(i, new Vector3(x, y, z));
											break;
									default:
											break;
							}

							angle += (360f / _segments);
					}

					// Update the collider
					Vector3[] positions = new Vector3[_line.positionCount];
					//This method fills positions with the array of vertices
					_line.GetPositions(positions);
					//Update method
					_colliderScript.UpdateEllipse(positions);
			}
	}
