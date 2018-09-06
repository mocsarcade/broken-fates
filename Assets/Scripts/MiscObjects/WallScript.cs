using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WallScript : MonoBehaviour {

	private EdgeCollider2D mainWall;

	private SpriteRenderer myRenderer;
	public List<GameObject> childObjects = new List<GameObject>();
	public int wallRank;

	//Initialize
	protected void Awake() {
		myRenderer = GetComponent<SpriteRenderer>();
	}

	void Start () {
		myRenderer.sortingOrder = (int) ((transform.position.y-myRenderer.bounds.extents.y) * GlobalRegistry.SORTING_Y_MULTIPLIER());
		//Populate Polygon Mesh
			//Call GlobalRegistry for rank of this wall
			//It's possible rank 1 has already called getEntries on this wall, so make sure walLRank hasn't been set yet
			if(wallRank == 0) {
				wallRank = GlobalRegistry.GetWallRank();
			}
			//If this wall's rank is the first one:
			if(wallRank == 1) {
				//Make EdgeCollider for this wall
				List<int> usedRanks = MakeEdgeCollider();
				List<int> wallsInt = Enumerable.Range(1, GlobalRegistry.GetWallNum()).ToList();


				GameObject[] wallsInScene = GameObject.FindGameObjectsWithTag("Wall");
				WallScript makingWall = this;
				int numLoops = 0;
				do {
					//Find if there are wall ranks that weren't used
					foreach(GameObject wall in wallsInScene) {
						WallScript checkingWall = wall.GetComponent<WallScript>();
						if(usedRanks.Contains(checkingWall.GetRank())) {
							wallsInt.Remove(checkingWall.GetRank());
							//Tell each wall that they are in this wall
							if(makingWall.GetEdgeCollider()) {
								checkingWall.SetEdgeCollider(makingWall.GetEdgeCollider());
							}
						}
					}

					if(wallsInt.Count > 0) {
						//Get all ranks and call MakeEdgeCollider on the first one remaining in wallsInt
						foreach(GameObject wall in wallsInScene) {
							makingWall = wall.GetComponent<WallScript>();
							if(wallsInt.Contains(makingWall.GetRank())) {
								usedRanks = makingWall.MakeEdgeCollider();
								break;
							}
						}
					}
					numLoops++;
				} while(wallsInt.Count > 0 && numLoops < GlobalRegistry.GetWallNum());

				foreach(int rank in wallsInt) {
					Debug.LogException(new Exception("Unused rank " + rank), this);
				}
			}
	}

	public EdgeCollider2D GetEdgeCollider() {
		return mainWall;
	}

	public void SetEdgeCollider(EdgeCollider2D _myWall) {
		mainWall = _myWall;
	}

	public int GetRank() {
		if(wallRank == 0) {
			wallRank = GlobalRegistry.GetWallRank();
		}
		return wallRank;
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
			if((checkMe.x > a.x && checkMe.x < b.x && b.x > a.x) || (checkMe.x < a.x && checkMe.x > b.x && a.x > b.x)) {
				return true;
			}
		}
		if(a.x == b.x && checkMe.x == a.x) {
			if((checkMe.y > a.y && checkMe.y < b.y && b.y > a.y) || (checkMe.y < a.y && checkMe.y > b.y && a.y > b.y)) {
				return true;
			}
		}
		return false;
	}

	private Vector3 FindLocalPoint(float x, float y, int rank, Vector2 point, List<Vector3> pointList) {
		foreach(Vector3 _point in pointList) {
			if((int) _point.z == rank) {
				if(x==-1) {
					if(_point.y == y && point.x != _point.x) {
						//Debug.Log("Found point from " + point + " to " + _point);
						return _point;
					}
				}
				if(y==-1) {
					if(_point.x == x && point.y != _point.y) {
						//Debug.Log("Found point from " + point + " to " + _point);
						return _point;
					}
				}
			}
		}
		//If you make it through it all with nothing, just return zero
		Debug.Log("Couldn't find it for rank " + rank);
		return Vector3.zero;
	}

	public List<int> MakeEdgeCollider() {
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
		Vector2 point = start; int curRank = wallRank;
		List<Vector2> edgePoints = new List<Vector2>(); List<Vector3> closePoints; List<int> pointRanks = new List<int>();
		edgePoints.Add(point);
		pointRanks.Add(curRank);
		//Until we get back to start
		Debug.Log("STARTINGSTARTINGSTARTINGSTARTINGSTARTINGSTARTINGSTARTINGSTARTINGSTARTING");
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
					Debug.Log("Already used point " + closePoints[i]);
					blackListPoints.Add(closePoints[i]);
				} //else {
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
						if(! checkedPoint.Equals(closePoints[i])) {
							if(checkedPoint.z == closePoints[i].z && (int) checkedPoint.z == curRank) {
								opposingStatus = true;
							}
							//Check if a box is on the left AND right sides, making it absolutely confirmed that these boxes are touching
							else if(checkedPoint.z == closePoints[i].z) {
								if(reqDirection == border.down && checkedPoint.y <= point.y) {
									opposingStatus = true;
									Vector3 foundPoint = FindLocalPoint(point.x, -1, curRank, point, paths);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.y > point.y) {
										Debug.Log("Removing " + closePoints[i] + "at " + reqDirection);
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.y < point.y) {
										Debug.Log("Removing " + checkedPoint + "at " + reqDirection);
										blackListPoints.Add(checkedPoint);
									}
								}
								if(reqDirection == border.up && checkedPoint.y >= point.y) {
									opposingStatus = true;
									Vector3 foundPoint = FindLocalPoint(point.x, -1, curRank, point, paths);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.y < point.y) {
										Debug.Log("Removing " + closePoints[i] + "at " + reqDirection);
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.y > point.y) {
										Debug.Log("Removing " + checkedPoint + "at " + reqDirection);
										blackListPoints.Add(checkedPoint);
									}
								}
								if(reqDirection == border.left && checkedPoint.x <= point.x) {
									opposingStatus = true;
									Vector3 foundPoint = FindLocalPoint(-1, point.y, curRank, point, paths);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.x > point.x) {
										Debug.Log("Removing " + closePoints[i] + "at " + reqDirection);
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.x < point.x) {
										Debug.Log("Removing " + checkedPoint + "at " + reqDirection);
										blackListPoints.Add(checkedPoint);
									}
								}
								if(reqDirection == border.right && checkedPoint.x >= point.x) {
									opposingStatus = true;
									Vector3 foundPoint = FindLocalPoint(-1, point.y, curRank, point, paths);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.x < point.x) {
										Debug.Log("Removing " + closePoints[i] + "at " + reqDirection);
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.x > point.x) {
										Debug.Log("Removing " + checkedPoint + "at " + reqDirection);
										blackListPoints.Add(checkedPoint);
									}
								}
							}
							//Check if THIS box encompasses the other box's width
							else if((int) checkedPoint.z == curRank) {
								if(reqDirection == border.down && checkedPoint.y >= closePoints[i].y) {
									opposingStatus = true;
									blackListPoints.Add(checkedPoint);
									/*
									Vector3 foundPoint = FindLocalPoint(closePoints[i].x, -1, (int) closePoints[i].z, (Vector2) closePoints[i], paths);
									Debug.Log("Cutting point " + closePoints[i] + " with local point " + foundPoint);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.y < closePoints[i].y) {
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.y > closePoints[i].y) {
										blackListPoints.Add(foundPoint);
									}*/
								}
								if(reqDirection == border.up && checkedPoint.y <= closePoints[i].y) {
									opposingStatus = true;
									blackListPoints.Add(checkedPoint);
									/*
									Vector3 foundPoint = FindLocalPoint(closePoints[i].x, -1, (int) closePoints[i].z, (Vector2) closePoints[i], paths);
									Debug.Log("Cutting point " + closePoints[i] + " with local point " + foundPoint);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.y > closePoints[i].y) {
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.y < closePoints[i].y) {
										blackListPoints.Add(foundPoint);
									}*/
								}
								if(reqDirection == border.left && checkedPoint.x >= closePoints[i].x) {
									opposingStatus = true;
									blackListPoints.Add(checkedPoint);
									/*
									Vector3 foundPoint = FindLocalPoint(-1, closePoints[i].y, (int) closePoints[i].z, (Vector2) closePoints[i], paths);
									Debug.Log("Cutting point " + closePoints[i] + " with local point " + foundPoint);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.x < closePoints[i].x) {
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.x > closePoints[i].x) {
										blackListPoints.Add(foundPoint);
									}*/
								}
								if(reqDirection == border.right && checkedPoint.x <= closePoints[i].x) {
									opposingStatus = true;
									blackListPoints.Add(checkedPoint);
									/*
									Vector3 foundPoint = FindLocalPoint(-1, closePoints[i].y, (int) closePoints[i].z, (Vector2) closePoints[i], paths);
									Debug.Log("Cutting point " + closePoints[i] + " with local point " + foundPoint);
									//Find if this box intersects with current box and cut off the direction in which the two are intersecting
									if(foundPoint.x > closePoints[i].x) {
										blackListPoints.Add(closePoints[i]);
									} else if(foundPoint.x < closePoints[i].x) {
										blackListPoints.Add(foundPoint);
									}*/
								}
							}
							//Check for an intermediary
							//Intermediary across the x axis
							if(closePoints[i].x == point.x && checkedPoint.x == point.x && closePoints[i].z != curRank) {
								Vector3 foundPoint = FindLocalPoint(checkedPoint.x, -1, (int) checkedPoint.z, (Vector2) checkedPoint, paths);
								//Check if checkedPoint is below or above both point and closePoints and sees if its localPoint spans the gap
								if(checkedPoint.y <= point.y && checkedPoint.y <= closePoints[i].y) {
									if(foundPoint.y >= point.y && foundPoint.y >= closePoints[i].y) {
										opposingStatus = true;
									}
								} else if(checkedPoint.y >= point.y && checkedPoint.y >= closePoints[i].y) {
									if(foundPoint.y <= point.y && foundPoint.y <= closePoints[i].y) {
										opposingStatus = true;
									}
								}
							}
							if(closePoints[i].y == point.y && checkedPoint.y == point.y && closePoints[i].z != curRank) {
								Vector3 foundPoint = FindLocalPoint(-1, checkedPoint.y, (int) checkedPoint.z, (Vector2) checkedPoint, paths);
								//Check if checkedPoint is below or above both point and closePoints and sees if its localPoint spans the gap
								if(checkedPoint.x <= point.x && checkedPoint.x <= closePoints[i].x) {
									if(foundPoint.x >= point.x && foundPoint.x >= closePoints[i].x) {
										opposingStatus = true;
									}
								} else if(checkedPoint.x >= point.x && checkedPoint.x >= closePoints[i].x) {
									if(foundPoint.x <= point.x && foundPoint.x <= closePoints[i].x) {
										opposingStatus = true;
									}
								}
							}
						}
					}
					//If we went through every point and COULDN'T find an opposing entry, scrap this point
					if(opposingStatus == false) {
						Debug.Log("Removing " + closePoints[i] + " for not having an equalizer");
						blackListPoints.Add(closePoints[i]);
					} else {
						//Bool value that will let us know we found a "tween" point once for loop is done
						bool betweenStatus = true;

						//Check if there any points between this one and the current one
						for(int j=closePoints.Count-1; j>=0; j--) {
							//If point j is betweeen point i and point k
							if(isBetween(closePoints[j], closePoints[i], point) && j != i) {
								//Set it to be DESTROYED at the end of this loop (removing objects mid-loop is dangerous)!
								betweenStatus = false;
								Debug.Log("curRank is " + curRank + " current point found between is " + closePoints[j] + " so " + closePoints[i] + " is out of the running");
								//Check if closePoints[j] is in the current point's rank. If so, DON'T GO TO THAT POINT
								if((int) closePoints[j].z == curRank) {
									blackListPoints.Add(closePoints[j]);
								}
							}
						}
						if(betweenStatus == false) {
							//Scrap point i
							blackListPoints.Add(closePoints[i]);
						}
					}
				//}
			}
			foreach(Vector3 toRemove in blackListPoints) {
				Debug.Log("Removing " + toRemove);
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
			pointRanks.Add(curRank);

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

		return pointRanks;
	}

	public enum border {
		up = 0,
		right = 1,
		down = 2,
		left = 3
	}

}
