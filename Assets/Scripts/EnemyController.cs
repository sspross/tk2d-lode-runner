using UnityEngine;
using System.Collections;

public class EnemyController : PersonBehaviour {
	
	public override void Start() {
		base.Start();
	}
	
	void Update() {
		Move();
		base.SetAnimation();
	}
	
	bool IsGrounded() {
  		return Physics.Raycast(transform.position, -Vector3.up, 1f);
	}
	
	void Move() {
		if (!IsGrounded()) {
			isFalling = true;
		} else {
			isFalling = false;
		}
		rigidbody.velocity = GetMoveDirection(1f, 0f);
	}
	
}
