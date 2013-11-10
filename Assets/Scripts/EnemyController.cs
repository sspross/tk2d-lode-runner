using UnityEngine;
using System.Collections;

public class EnemyController : PersonBehaviour {
	
	public override void Start() {
		base.Start();
	}
	
	void Update() {
		base.Update();
		Move();
		base.SetAnimation();
	}
	
	void Move() {
		velocityX = 1f;
		velocityY = 0f;
		Vector3 moveDirection = GetMoveDirection(velocityX, velocityY);
		Vector3 newPosition = new Vector3(rigidbody.position.x + moveDirection.x, rigidbody.position.y + moveDirection.y, rigidbody.position.z + moveDirection.z);
		rigidbody.MovePosition(newPosition);
	}
	
}
