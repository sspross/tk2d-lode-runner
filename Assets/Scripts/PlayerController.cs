using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {
	
	private float speed = 100F;
	private float velocityX, velocityY = 0F;
	private CharacterController characterController;
	private tk2dSpriteAnimator spriteController;
	private tk2dSpriteAnimator leftShootSprite, rightShootSprite;
	private Vector3 moveDirection = Vector3.zero;
	
	private bool lookRight = true;
	private bool onLadder = false;
	private bool onRope = false;
	
	private int ladderTriggerCount, ropeTriggerCount = 0;


	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController>();
		spriteController = GetComponent<tk2dSpriteAnimator>(); ;
		leftShootSprite = transform.Find("leftShooter").GetComponent<tk2dSpriteAnimator>();
		rightShootSprite = transform.Find("rightShooter").GetComponent<tk2dSpriteAnimator>();
		leftShootSprite.gameObject.SetActive(false);
		rightShootSprite.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		InputCheck();
		Move();
		SetAnimation();
	}
	
	void InputCheck () {
		velocityX = Input.GetAxis("Horizontal");
		if (velocityX > 0) {
			lookRight = true;
		}
		if (velocityX < 0) {
			lookRight = false;
		}
		velocityY = Input.GetAxis("Vertical");
		if (Input.GetKeyDown("space") && !onRope && !onLadder) {
			if (lookRight) {
				rightShootSprite.gameObject.SetActive(true);
				rightShootSprite.Play();
				rightShootSprite.gameObject.SetActive(false);
			} else {
				leftShootSprite.gameObject.SetActive(true);
				leftShootSprite.Play();
			}
		}
	}
	
	void Move () {
		if(onLadder) 
		{
			moveDirection = new Vector3(velocityX, velocityY, 0f);
			moveDirection *= Time.deltaTime * speed;
		} else if (onRope) {
			moveDirection = new Vector3(velocityX, 0f, 0f);
			moveDirection *= Time.deltaTime * speed;
			if (velocityY < 0) {
				onRope = false;	
			}
		} else {
			moveDirection = new Vector3(velocityX, -1f, 0f);
			moveDirection *= Time.deltaTime * speed;
		}
		characterController.Move(moveDirection);
	}
	
	void SetAnimation () {
		if (onLadder) {
			if (velocityY != 0) {
				if (spriteController.IsPlaying("climb")) {
					spriteController.Resume();
				} else {
					spriteController.Play("climb");
				}
			} else {
				if (!spriteController.IsPlaying("climb")) {
					spriteController.Play("climb");
				}
				spriteController.Stop();
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
		} else {
			if (velocityX > 0) {
				spriteController.Play("walkRight");
			}
			if (velocityX < 0) {
				spriteController.Play("walkLeft");
			}
			if (velocityX == 0) {
				if (lookRight) {
					spriteController.Play("stayRight");
				} else {
					spriteController.Play("stayLeft");
				}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
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
	
	void OnTriggerExit(Collider other)
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
