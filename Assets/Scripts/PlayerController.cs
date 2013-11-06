﻿using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {
	
	private float speed = 130F;
	private float velocityX, velocityY = 0F;
	private CharacterController characterController;
	private GameController gameController;
	private tk2dSpriteAnimator spriteController;
	private Vector3 moveDirection = Vector3.zero;
	
	private bool lookRight = true;
	private bool onLadder = false;
	private bool onRope = false;
	private bool isShooting = false;
	private bool isFalling = false;
	
	private int ladderTriggerCount, ropeTriggerCount = 0;
	
	private Transform shootParent;
	private Renderer shootRenderer;
	private tk2dSpriteAnimator shootSprite;
	
	private RaycastHit hit;

	// Use this for initialization
	void Start () {
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		characterController = GetComponent<CharacterController>();
		spriteController = GetComponent<tk2dSpriteAnimator>();
		shootParent = transform.Find("shoot parent");
		shootRenderer = GameObject.Find("shoot").renderer;
		shootSprite = GameObject.Find("shoot").GetComponent<tk2dSpriteAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
		InputCheck();
		Move();
		SetAnimation();
	}
	
	void InputCheck () {
		velocityX = Input.GetAxis("Horizontal");
		velocityY = Input.GetAxis("Vertical");
		if (velocityX > 0) {
			lookRight = true;
		}
		if (velocityX < 0) {
			lookRight = false;
		}
		
		// shooting
		if (Input.GetKeyDown("space") && !onRope && !onLadder) {
			StartCoroutine(Shoot());
		}
	}
	
	void UpdateRaycasts() { 
		float correction = 45f;
		if (!lookRight) {
			correction = -10f;
		}
		Vector3 higherMe = new Vector3(transform.position.x+correction, transform.position.y+15f, transform.position.z);
		if (Physics.Raycast(higherMe, Vector3.down, out hit, 30.0F)) {
			if (hit.transform.tag == "brick") {
				hit.transform.SendMessage("Break", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void Move () {
		if (!isShooting) {
			if (onLadder) {
				moveDirection = new Vector3(velocityX, velocityY, 0f);
				moveDirection *= Time.deltaTime * speed;
			} else if (onRope) {
				moveDirection = new Vector3(velocityX, 0f, 0f);
				moveDirection *= Time.deltaTime * speed;
				if (velocityY < 0) {
					onRope = false;	
				}
			} else {
				if (!characterController.isGrounded) {
					isFalling = true;
				} else {
					isFalling = false;
				}
				moveDirection = new Vector3(velocityX, -1.5f, 0f);
				moveDirection *= Time.deltaTime * speed;
			}
			characterController.Move(moveDirection);
		}
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
		if(other.gameObject.CompareTag("pickup"))
		{
			gameController.SendMessage("PickedUp", SendMessageOptions.DontRequireReceiver);
			other.gameObject.renderer.enabled = false;	
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
	
	IEnumerator Shoot()
	{	
		isShooting = true;
		shootRenderer.enabled = true;
		shootSprite.Play("shoot");
		
		// check facing direction and flip the shoot parent to the correct side
		if (lookRight) {
			shootParent.localPosition = new Vector3(30, 0, 0);
			shootParent.localScale = new Vector3(-1, 1, 1); // right side
		} else {
			shootParent.localPosition = new Vector3(0, 0, 0);
			shootParent.localScale = new Vector3(1, 1,1); // left side
		}
		
		yield return new WaitForSeconds(0.35f);
		UpdateRaycasts();
		yield return new WaitForSeconds(0.15f);
		shootRenderer.enabled = false;
		isShooting = false;
	}

}
