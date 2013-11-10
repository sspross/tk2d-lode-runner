using UnityEngine;
using System.Collections;

public class PersonBehaviour : MonoBehaviour {
	
	public float gravity = 1f;
	public float speed = 130f;
	public float velocityX, velocityY = 0f;
	
	public tk2dSpriteAnimator spriteController;
	public Vector3 moveDirection = Vector3.zero;
	public Vector3 currentPosition, lastPosition;
	
	public bool lookRight = true;
	public bool onLadder = false;
	public bool onRope = false;
	public bool isFalling = false;
	public bool isShooting = false;
	public bool isGrounded = false;
	public bool isMovingDown = false;
	public bool isMovingUp = false;
	
	public int ladderTriggerCount, ropeTriggerCount = 0;
	
	public RaycastHit hit;

	public virtual void Start() {
		spriteController = GetComponent<tk2dSpriteAnimator>();
	}
	
	public virtual void LateUpdate() {
		lastPosition = currentPosition;
	}
		
	public virtual Vector3 GetMoveDirection(float x, float y) {
		if (onLadder) {
			moveDirection = new Vector3(x, y, 0f);
		} else if (onRope) {
			moveDirection = new Vector3(x, 0f, 0f);
			if (y < 0) {
				onRope = false;
			}
		} else {
			if (isFalling) {
				moveDirection = new Vector3(0f, -1f * gravity, 0f);
			} else {
				moveDirection = new Vector3(x, -1f * gravity, 0f);
			}
		}
		moveDirection *= Time.deltaTime * speed;
		return moveDirection;
	}
	
	public virtual void SetAnimation() {
		if (onLadder) {
			if (velocityY != 0) {
				if (spriteController.IsPlaying("climb")) {
					spriteController.Resume();
				} else {
					spriteController.Play("climb");
				}
			}
		} else if (onRope) {
			if (velocityX > 0) {
				spriteController.Play("ropeRight");
			}
			if (velocityX < 0) {
				spriteController.Play("ropeLeft");
			}
			if (velocityX == 0) {
				spriteController.Stop();
			}
		} else if (isFalling) {
			if (lookRight) {
				spriteController.Play("fallRight");
			} else {
				spriteController.Play("fallLeft");
			}
		} else {
			if (velocityX > 0 && !isShooting) {
				spriteController.Play("walkRight");
			}
			if (velocityX < 0 && !isShooting) {
				spriteController.Play("walkLeft");
			}
			if (velocityX == 0 || isShooting) {
				if (lookRight) {
					spriteController.Play("stayRight");
				} else {
					spriteController.Play("stayLeft");
				}
			}
		}
	}

	public virtual void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("ladder"))
		{
			ladderTriggerCount++;
			onLadder = true;
		}
		if(other.gameObject.CompareTag("rope"))
		{
			ropeTriggerCount++;
			onRope = true;
		}
	}

	public virtual void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("ladder"))
		{
			ladderTriggerCount--;
			if (ladderTriggerCount <= 0) {
				onLadder = false;
			}
		}
		if (other.gameObject.CompareTag("rope"))
		{
			ropeTriggerCount--;
			if (ropeTriggerCount <= 0) {
				onRope = false;
			}
		}
	}

}
