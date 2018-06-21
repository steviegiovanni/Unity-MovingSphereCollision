using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour {
	[SerializeField]
	private float _radius;
	public float Radius{
		get{ return _radius;}
		set{ _radius = value;}
	}

	public GameObject staticSphere;

	void OnValidate()
	{
		transform.localScale = new Vector3 (Radius * 2, Radius * 2, Radius * 2);
	}

	private void OnDrawGizmos(){
		// draw scope
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine (transform.position + transform.right * Radius, transform.position + transform.right * Radius + transform.forward * 100.0f);
		Gizmos.DrawLine (transform.position - transform.right * Radius, transform.position - transform.right * Radius + transform.forward * 100.0f);
		Gizmos.DrawLine (transform.position + transform.up * Radius, transform.position + transform.up * Radius + transform.forward * 100.0f);
		Gizmos.DrawLine (transform.position - transform.up * Radius, transform.position - transform.up * Radius + transform.forward * 100.0f);

		// draw arrow
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine (transform.position + transform.forward * 3.0f * Radius, transform.position + transform.forward * 2.0f * Radius + transform.right * Radius * 0.75f);
		Gizmos.DrawLine (transform.position + transform.forward * 3.0f * Radius, transform.position + transform.forward * 2.0f * Radius + transform.up * Radius * 0.75f);
		Gizmos.DrawLine (transform.position + transform.forward * 3.0f * Radius, transform.position + transform.forward * 2.0f * Radius - transform.right * Radius * 0.75f);
		Gizmos.DrawLine (transform.position + transform.forward * 3.0f * Radius, transform.position + transform.forward * 2.0f * Radius - transform.up * Radius * 0.75f);

		if(staticSphere != null){
			Vector3 posAtImpact = Vector3.zero;
			Vector3 collisionPoint = Vector3.zero;
			if (CalculateCollisionPoint (this.transform.position,staticSphere.transform.position,Radius,staticSphere.transform.localScale.x/2,transform.forward,out posAtImpact,out collisionPoint)) {
				// draw path
				Gizmos.color = new Color (0,1,0,0.5f);
				Gizmos.DrawLine (transform.position, posAtImpact);

				// draw sphere
				Gizmos.color = new Color (0,1,0,0.5f);
				Gizmos.DrawSphere (posAtImpact, Radius);

				// draw collision
				Gizmos.color = Color.red;
				Gizmos.DrawLine(collisionPoint + Vector3.up, collisionPoint + Vector3.down);
				Gizmos.DrawLine(collisionPoint + Vector3.right, collisionPoint + Vector3.left);
				Gizmos.DrawLine(collisionPoint + Vector3.forward, collisionPoint + Vector3.back);
			}
		}
	}

	private bool CalculateCollisionPoint(Vector3 movingSpherePos, Vector3 staticSpherePos, float movingSphereRadius, float staticSphereRadius, Vector3 direction, out Vector3 posAtImpact, out Vector3 collisionPoint){
		posAtImpact = Vector3.zero;
		collisionPoint = Vector3.zero;

		// get vector from moving sphere to static sphere
		Vector3 D = staticSpherePos - movingSpherePos;

		// get projection of D on direction vector
		Vector3 E = Vector3.Dot(D,direction) * direction;

		// see whether static ball is behind or in front of the moving ball
		if(Vector3.Dot(E,direction) < 0) return false;

		// find shortest distance from center of static sphere, to direction vector
		float d = Vector3.Magnitude(E - D);

		// determine whether the two spheres will collide
		if(d > movingSphereRadius + staticSphereRadius) return false; // does not collide

		// case where the two spheres collide
		// find angle between projection point of static sphere on the movement vector of moving sphere
		// and the position of moving sphere during impact
		float theta = Mathf.Acos(d/(movingSphereRadius + staticSphereRadius));

		// find distance between projection point of static sphere on the movement vector of moving sphere
		// to the position of moving sphere during impact
		float delta = Mathf.Sin(theta) * (movingSphereRadius + staticSphereRadius);

		// find how far moving sphere has moved before impact
		float moveRatio = (Vector3.Magnitude(E) - delta) / Vector3.Magnitude(E);

		// find the position of moving sphere at the point of impact
		posAtImpact = movingSpherePos + E * moveRatio;

		// find the collision point
		collisionPoint = posAtImpact + (staticSpherePos - posAtImpact) * movingSphereRadius / (movingSphereRadius + staticSphereRadius);

		return true;
	}
}
