using UnityEngine;
using System.Collections;

namespace Pixel.Inc
{
	public class TankController : MonoBehaviour
	{
		[SerializeField]
		private float speed;

		[SerializeField]
		private float rotationSpeed;
		public bool isTestMode = false;
		float horizontalInput, verticalInput;
		public bool isMoving = false;

		public TankNetworkPlayer player;
		IEnumerator Start()
		{
			yield return null;

		}

		private void Update()
		{
			
		}

		private void FixedUpdate()
		{


			if (player.player.isPlayer || isTestMode)
			{
				horizontalInput = Input.GetAxis("Horizontal");
				verticalInput = Input.GetAxis("Vertical");
				if (horizontalInput != 0 || verticalInput != 0)
				{
					isMoving = true;
				}
				else
				{
					isMoving = false;
				}
				Vector2 movementDirection = new Vector2(horizontalInput, verticalInput);
				float inputMagnitude = Mathf.Clamp01(movementDirection.magnitude);
				movementDirection.Normalize();

				transform.Translate(movementDirection * speed * inputMagnitude * Time.deltaTime, Space.World);

				if (movementDirection != Vector2.zero)
				{
					Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, movementDirection);
					transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
				}
			}

			//pos = Camera.main.WorldToViewportPoint(transform.position);
			//Debug.Log("I am left of the camera's view. " + pos.x);

			//Debug.Log("I am below the camera's view." + pos.y);
			//if (pos.x < 0.0) Debug.Log("I am left of the camera's view. " + pos.x);
			//if (1.0 < pos.x) Debug.Log("I am right of the camera's view." + pos.x);
			//if (pos.y < 0.01f) Debug.Log("I am below the camera's view." + pos.y);
			//if (0.9f < pos.y)
			//{
			//	Debug.Log("I am above the camera's view." + pos.y);
			//	transform.position = ;
			//}
		}

		public Vector3 pos;
	}


}

