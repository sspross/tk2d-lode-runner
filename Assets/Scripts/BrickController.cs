using UnityEngine;
using System.Collections;

public class BrickController : MonoBehaviour {
	
	public float brokenTime = 6.0f;
	
	private tk2dSpriteAnimator spriteController;
	private bool isBreaking = false;

	// Use this for initialization
	void Start () {
		spriteController = GetComponent<tk2dSpriteAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void Break () {
		if (!isBreaking) {
			StartCoroutine(BreakAndFill());
		}
	}
	
	IEnumerator BreakAndFill()
	{	
		isBreaking = true;
		spriteController.Play("break");
		yield return new WaitForSeconds(0.7f);
		transform.collider.enabled = false;
		
		yield return new WaitForSeconds(brokenTime);
		spriteController.Play("fill");
		transform.collider.enabled = true;
		isBreaking = false;
	}
	
}
