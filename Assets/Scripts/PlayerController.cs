using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {
	
	public float gravity = 0.8F;
	public float speed = 10F;
	
	private float velocity = 0F;
	private CharacterController characterController;
	private tk2dSpriteAnimator spriteController;
	private Vector3 moveDirection = Vector3.zero;
	
	private bool lookRight = true;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController>();
		spriteController = GetComponent<tk2dSpriteAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
		InputCheck();
		Move();
		SetAnimation();
	}
	
	void InputCheck () {
		velocity = Input.GetAxis("Horizontal") * speed;
		if (velocity > 0) {
			lookRight = true;
		}
		if (velocity < 0) {
			lookRight = false;
		}
	}
	
	void Move () {
		moveDirection.x = velocity;
		moveDirection.y -= gravity;
		characterController.Move(moveDirection * Time.deltaTime);
	}
	
	void SetAnimation () {
		if (velocity > 0) {
			spriteController.Play("walkRight");
		}
		if (velocity < 0) {
			spriteController.Play("walkLeft");
		}
		if (velocity == 0) {
			if (lookRight) {
				spriteController.Play("stayRight");
			} else {
				spriteController.Play("stayLeft");
			}
		}
	}

}
