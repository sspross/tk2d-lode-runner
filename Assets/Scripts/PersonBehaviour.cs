using UnityEngine;
using System.Collections;

public class PersonBehaviour : MonoBehaviour {
	
	public float gravity = 1.5f;
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
		// get self sprite controller
		spriteController = GetComponent<tk2dSpriteAnimator>();
	}
	
	public virtual void Update() {
		// evaluate if person is falling
		currentPosition = transform.position;
		if (currentPosition.y < lastPosition.y && !isGrounded && !onLadder) {
			isFalling = true;
		} else {
			isFalling = false;	
		}
	}
	
	public virtual void LateUpdate() {
		// save last position to support falling evaluation
		lastPosition = currentPosition;
	}
		
	public virtual Vector3 GetMoveDirection(float x, float y) {
		if (onLadder) {
			if (isMovingUp || isMovingDown) {
				// if on ladder and moving vertical, fix horizontal
				moveDirection = new Vector3(0f, y, 0f);
			} else {
				// if on ladder and not moving vertical, release horizontal too
				moveDirection = new Vector3(x, y, 0f);	
			}
		} else if (onRope) {
			// if on rope, fix vertical
			moveDirection = new Vector3(x, 0f, 0f);
			if (isMovingDown) {
				// exit rope if moving down
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
		if (onLadder && (isMovingUp || isMovingDown)) {
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
	
	public virtual void OnTriggerStay(Collider other) {
		if(other.gameObject.CompareTag("ladder"))
		{
			if (isMovingUp || isMovingDown) {
				// snap on ladder
				Vector3 position = new Vector3(other.gameObject.transform.position.x - 4f, transform.position.y, transform.position.z);
				transform.position = position;
			}
		}
		if(other.gameObject.CompareTag("rope"))
		{
			// snap on rope
			Vector3 position = new Vector3(transform.position.x, other.gameObject.transform.position.y, transform.position.z);
			transform.position = position;
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
