using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {

	private static EdgeCollider2D mainWall;

	private SpriteRenderer myRenderer;
	public List<GameObject> childObjects = new List<GameObject>();
	private int wallRank;

	//Initialize
	protected void Awake() {
		myRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		myRenderer.sortingOrder = (int) ((transform.position.y-myRenderer.bounds.extents.y) * GlobalRegistry.SORTING_Y_MULTIPLIER());
		//Populate Polygon Mesh
			//Call GlobalRegistry for rank of this wall
			if(wallRank == 0) {
				wallRank = GlobalRegistry.GetWallRank();
			}
			//If this wall's rank is the first one:
			if(wallRank == 1) {
				//Call "get entries" of all walls and combine into dictionary
				GameObject[] wallsInScene = GameObject.FindGameObjectsWithTag("Wall");
				//Dictionary<Vector2, Vector2> paths = new Dictionary<Vector2, Vector2>();
				List<Vector3> paths = new List<Vector3>();
				foreach(GameObject wall in wallsInScene) {
					wall.GetComponent<WallScript>().GetEntries(ref paths);
				}

				//Combine
				//Start with top left of this wall and set point=start
				Vector2 start = new Vector2(myRenderer.bounds.min.x, myRenderer.bounds.max.y);
				Vector2 point = start; int curRank = 1;
				List<Vector2> edgePoints = new List<Vector2>(); List<Vector3> closePoints;
				edgePoints.Add(point);
				//Until we get back to start
				do {
					//Get all possible points
					closePoints = findPoints(point, paths);
					List<Vector3> blackListPoints = new List<Vector3>();

					for(int i=closePoints.Count-1; i>=0; i--) {
						//if this point is the exact point we're on, get rid of it.
						if(closePoints[i].x == point.x && closePoints[i].y == point.y)
						{
							closePoints.RemoveAt(i);
						}
					}
					//Narrow down to all feasible points
					for(int i=closePoints.Count-1; i>=0; i--) {
						//If this point has already been used, get rid of it (unless we've looped around to the beginning)
						if(edgePoints.Contains(closePoints[i]) && (! ((Vector2) closePoints[i]).Equals(start) || edgePoints.Count <= 2) ) {
							blackListPoints.Add(closePoints[i]);
						} else if(closePoints[i].x != point.x || closePoints[i].y != point.y) {
							bool opposingStatus = false;
							//This variable will change when set according to point's place in comparison
							//It's just set to border.up so unity won't complain
							border reqDirection = border.up;

							if(closePoints[i].x > point.x)
								reqDirection = border.left;
							if(closePoints[i].x < point.x)
								reqDirection = border.right;
							if(closePoints[i].y > point.y)
								reqDirection = border.down;
							if(closePoints[i].y < point.y)
								reqDirection = border.up;

							foreach(Vector3 checkedPoint in closePoints) {
								if(checkedPoint.z == closePoints[i].z) {
									if(reqDirection == border.down && checkedPoint.y < point.y) {
										opposingStatus = true;
									}
									if(reqDirection == border.up && checkedPoint.y > point.y) {
										opposingStatus = true;
									}
									if(reqDirection == border.left && checkedPoint.x < point.x) {
										opposingStatus = true;
									}
									if(reqDirection == border.right && checkedPoint.x > point.x) {
										opposingStatus = true;
									}
								}
								if((int) checkedPoint.z == curRank) {
									if(checkedPoint.x == closePoints[i].x && checkedPoint.y == closePoints[i].y) {
										opposingStatus = true;
									}
								}
							}
							//If we went through every point and COULDN'T find an opposing entry, scrap this point
							if(opposingStatus == false) {
								blackListPoints.Add(closePoints[i]);
							} else {
								//Finally, check if there any points between this one and the current one
								for(int j=closePoints.Count-1; j>=0; j--) {
									//If point j is betweeen point i and point k
									if(isBetween(closePoints[j], closePoints[i], point) && ((int) closePoints[j].z) != curRank && j != i) {
										//Set it to be DESTROYED at the end of this loop (removing objects mid-loop is dangerous)!
										opposingStatus = false;
										Debug.Log("curRank is " + curRank + " current point found between is " + closePoints[j]);
										//Check if closePoints[j] is in the current point's rank. If so, DON'T GO TO THAT POINT
										if((int) closePoints[j].z == curRank) {
											blackListPoints.Add(closePoints[j]);
										}
									}
								}
								if(opposingStatus == false) {
									//Scrap point i
									blackListPoints.Add(closePoints[i]);
								}
							}
						}
					}
					foreach(Vector3 toRemove in blackListPoints) {
						//Make sure I didn't make a mistake and add the same value twice to be removed
						if(closePoints.Contains(toRemove))
							closePoints.Remove(toRemove);
					}
					foreach(Vector3 aPoint in closePoints) {
						Debug.Log("Point " + point + " could go to: " + aPoint);
					}
					//Check that there are points left. If not, raise an error
					if(closePoints.Count == 0) {
						//Leave this while loop and place current edgePoints into the collider
						break;
					} else {
						//If there's a point not in this object's rank, do that one.
						bool foundPoint = false;
						foreach(Vector3 curPoint in closePoints) {
							if(((int) curPoint.z) != curRank) {
								point = (Vector2) curPoint;
								curRank = (int) curPoint.z;
								foundPoint = true;
								break;
							}
						}
						//If foundPoint is false, that means we never found a point not in this point's rank
						if(foundPoint == false) {
							//Just use the first one
							point = (Vector2) closePoints[0];
							curRank = (int) closePoints[0].z;
						}
					}
					//Place new point in array
					edgePoints.Add(point);

					//Keep going until we go full circle
				} while(! point.Equals(start));
					//This may have just gone around the outside of all the walls, so check if this wall's
					//inside point (bottom-left) has been used. If not, start over with bottom-left as start!

				for(int i=0; i<edgePoints.Count; i++) {
					edgePoints[i] = edgePoints[i] - (Vector2) transform.position;
				}
				//Create mainWall by making component
				mainWall = gameObject.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;
				//Make this full array the points for mainWall
				mainWall.points = edgePoints.ToArray();
			}

	}

	public static EdgeCollider2D GetEdgeCollider() {
		return mainWall;
	}

	//public void GetEntries(ref Dictionary<Vector2, Vector2> pointDict) {
	public void GetEntries(ref List<Vector3> pointList) {
		if(wallRank == 0) {
			wallRank = GlobalRegistry.GetWallRank();
		}

		float bottom = myRenderer.bounds.min.y;
		float top = myRenderer.bounds.max.y;
		float left = myRenderer.bounds.min.x;
		float right = myRenderer.bounds.max.x;
		/*pointDict.Add(new Vector2(left, top), new Vector2(right, top));
		pointDict.Add(new Vector2(right, top), new Vector2(right, bottom));
		pointDict.Add(new Vector2(right, bottom), new Vector2(left, bottom));
		pointDict.Add(new Vector2(left, bottom), new Vector2(left, top));*/

		//Wall Rank is entered as z so the algorithm will know which wall this point is from
		pointList.Add(new Vector3(left, top, wallRank));
		pointList.Add(new Vector3(right, top, wallRank));
		pointList.Add(new Vector3(right, bottom, wallRank));
		pointList.Add(new Vector3(left, bottom, wallRank));
	}

	private List<Vector3> findPoints(Vector2 point, List<Vector3> pointList) {
		List<Vector3> returnList = new List<Vector3>();
		foreach(Vector3 wallPoint in pointList) {
			//Find if point is on same line as this one
			if(wallPoint.x == point.x || wallPoint.y == point.y) {
				returnList.Add(wallPoint);
			}
		}

		return returnList;
	}

	private bool isBetween(Vector3 checkMe, Vector3 a, Vector2 b) {
		if(a.y == b.y && checkMe.y == a.y) {
			if((checkMe.x >= a.x && checkMe.x <= b.x && b.x > a.x) || (checkMe.x <= a.x && checkMe.x >= b.x && a.x > b.x)) {
				return true;
			}
		}
		if(a.x == b.x && checkMe.x == a.x) {
			if((checkMe.y >= a.y && checkMe.y <= b.y && b.y > a.y) || (checkMe.y <= a.y && checkMe.y >= b.y && a.y > b.y)) {
				return true;
			}
		}
		return false;
	}

	public enum border {
		up = 0,
		right = 1,
		down = 2,
		left = 3
	}

}
