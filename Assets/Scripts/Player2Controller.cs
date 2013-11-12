using UnityEngine;
using System.Collections;


public class Player2Controller : PersonBehaviour {
	
	private GameController gameController;
	private Rigidbody characterController;
	
	private Transform shootParent;
	private Renderer shootRenderer;
	private tk2dSpriteAnimator shootSprite;
	
	private float playerHitboxY, playerHitboxX = 30f;

	public override void Start () {
		base.Start();
		
		gameController = GameObject.Find("GameController").GetComponent<GameController>();
		characterController = GetComponent<Rigidbody>();
		
		shootParent = transform.Find("shoot parent");
		shootRenderer = shootParent.Find("shoot").renderer;
		shootSprite = shootParent.Find("shoot").GetComponent<tk2dSpriteAnimator>();
	}

	public void Update () {	
		base.Update();
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
	        }
	    } else {
	    	velocityX = Input.GetAxisRaw("Horizontal");
			velocityY = Input.GetAxisRaw("Vertical");
	    }
		
		if (velocityX > 0) {
			velocityX = 1;
		}
		if (velocityX < 0) {
			velocityX = -1;
		}
		if (velocityY > 0) {
			velocityY = 1;
			isMovingDown = false;
			isMovingUp = true;
		}
		if (velocityY < 0) {
			velocityY = -1;
			isMovingDown = true;
			isMovingUp = false;
		}
		if (velocityY == 0) {
			isMovingDown = false;
			isMovingUp = false;
		}
		
		if (velocityX > 0) {
			lookRight = true;
		}
		if (velocityX < 0) {
			lookRight = false;
		}
		
		if (IsGrounded()) {
			isGrounded = true;
		} else {
			isGrounded = false;
		}

		if ((Input.GetKeyDown("space") || Input.touchCount > 1) && !onRope && !onLadder && !isFalling) {
			StartCoroutine(Shoot());
		}
	}
	
	bool IsGrounded() {
		RaycastHit hit;
		bool result = Physics.Raycast(characterController.transform.position, Vector3.down, out hit, 17f);
		Debug.DrawLine(characterController.transform.position, hit.point, Color.green);
  		return result;
	}
	
	void Move() {
		if (!isShooting) {
			Vector3 moveDirection = GetMoveDirection(velocityX, velocityY);
			Vector3 newPosition = new Vector3(rigidbody.position.x + moveDirection.x, rigidbody.position.y + moveDirection.y, rigidbody.position.z + moveDirection.z);
			characterController.MovePosition(newPosition);
			//characterController.Move(GetMoveDirection(velocityX, velocityY));
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
