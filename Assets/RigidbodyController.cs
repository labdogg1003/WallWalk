using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

public class RigidbodyController : MonoBehaviour
{
	Rigidbody rigidbody;
	bool isGrounded;
	bool usingController;

	public float speed = 6.0f;
	public float gravity = 20.0f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 2.0f;
	public float characterHeight = 1.3f;

	public GameObject[] feet;

	//public float runMultiplier;

	// Use this for initialization
	void Start ()
	{
		//Grab Rigidbody so we aren't calling it each cycle
		rigidbody = gameObject.GetComponent<Rigidbody>();

		//We Want To Manually Calculate Gravity. i.e. Be Able To Change The Value Of Gravity.
		rigidbody.useGravity = false;

		//Stop RigidBody From Rotating Freely and Falling down.
		rigidbody.freezeRotation = true;
	}
	
	// Update is called once per phyisics cycle
	void FixedUpdate ()
	{
		//Check If We Are On The Ground
		if(isGrounded)
		{
			Vector3 targetVelocity;

			// Translate Input Into A Vector3
			if(!usingController)
			{
				targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			}
			else
			{
				targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			}

			// Calculate how fast we should be moving
			targetVelocity = transform.TransformDirection(targetVelocity);
			targetVelocity *= speed;

			// Apply a force that attempts to reach our target velocity
			Vector3 velocity = rigidbody.velocity;
			Vector3 velocityChange = (targetVelocity - velocity);

			//Clamp All 3 Axis So That Player Does Not Acclerate Faster Than Velocity Change.
			velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);

			//Don'T Allow Changes In Y Without "Jump" Being Pressed
			velocityChange.y = 0;

			//Attempt To 
			rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);

		    //Jump using "Jump" Button by adding force in y direction.
			if (Input.GetButton("Jump"))
			{
				rigidbody.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
			}
		}

		//Manually Add In Gravity
		rigidbody.AddForce(new Vector3 (0, -gravity * rigidbody.mass, 0));

		isGrounded = false;

		//Check if player is grounded
		//RaycastHit hit;
		//Ray ray = new Ray(transform.position,-transform.up);

		for(int i = 0; i<feet.Length; i++)
		{
			//For Debugging Purposes draw the ray to the ground from the player
			Debug.DrawRay (feet[i].transform.position, -transform.up * (characterHeight/2), Color.green);

			RaycastHit hit;
			Ray ray = new Ray(feet[i].transform.position,-transform.up);

			if (Physics.Raycast(ray, out hit, characterHeight/2))
			{
				//Check If You hit a collider (the "ground")
				if(hit.collider)
				{
					//You Are Grounded.
					isGrounded = true;
				}
			
			}
			if(isGrounded)
			{
				break;
			}
		}
	}

	float CalculateJumpVerticalSpeed ()
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}
	
}