﻿using UnityEngine;
using System.Collections;


public class PlayerController : PersonBehaviour {
	
	private GameController gameController;
	private CharacterController characterController;
	
	private Transform shootParent;
	private Renderer shootRenderer;
	private tk2dSpriteAnimator shootSprite;

	public override void Start () {
		base.Start();
		
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		characterController = GetComponent<CharacterController>();
		
		shootParent = transform.Find("shoot parent");
		shootRenderer = GameObject.Find("shoot").renderer;
		shootSprite = GameObject.Find("shoot").GetComponent<tk2dSpriteAnimator>();
	}

	void Update () {
		InputCheck();
		Move();
		base.SetAnimation();
	}

	void InputCheck() {
		if (Input.touchCount == 1) {
	        Touch touch = Input.GetTouch(0);
	        if (touch.phase == TouchPhase.Moved) {
	            velocityX = touch.deltaPosition.x;
	            velocityY = touch.deltaPosition.y;
				if (velocityX > 1) {
					velocityX = 1;
				}
				if (velocityX < -1) {
					velocityX = -1;
				}
				if (velocityY > 1) {
					velocityY = 1;
				}
				if (velocityY < -1) {
					velocityY = -1;
				}
	        }
	    } else {
	    	velocityX = Input.GetAxis("Horizontal");
			velocityY = Input.GetAxis("Vertical");
	    }
		if (velocityX > 0) {
			lookRight = true;
		}
		if (velocityX < 0) {
			lookRight = false;
		}

		if ((Input.GetKeyDown("space") || Input.touchCount > 1) && !onRope && !onLadder) {
			StartCoroutine(Shoot());
		}
	}
	
	void Move() {
		if (!isShooting) {
			if (!characterController.isGrounded) {
				isFalling = true;
			} else {
				isFalling = false;
			}
			characterController.Move(GetMoveDirection(velocityX, velocityY));
		}
	}
	
	public override void OnTriggerEnter(Collider other) {
		base.OnTriggerEnter(other);
		if(other.gameObject.CompareTag("pickup"))
		{
			gameController.SendMessage("PickedUp", SendMessageOptions.DontRequireReceiver);
			other.gameObject.renderer.enabled = false;
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
