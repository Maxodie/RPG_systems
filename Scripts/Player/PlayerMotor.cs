using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
	public Camera cam;

	Vector3 velocity;
	Vector3 rotation;
	float cameraRotationX = 0f;
	float currentCameraRotationX = 0f;

	[SerializeField]
	float cameraRotationLimit = 85f;

	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
	}

	public void Rotate(Vector3 _rotation)
	{
		rotation = _rotation;
	}

	public void RotateCamera(float _camerarotationX)
	{
		cameraRotationX = _camerarotationX;
	}

	void FixedUpdate()
	{
		PerformMouvement();
		PerformRotation();
	}

	void PerformMouvement()
	{
		//Déplacer le joueur par raport à sa position + la velosité donné dans PlayerControler
		if(velocity != Vector3.zero)
		{
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}
	}

	void PerformRotation()
	{
		//récuperation de la rotation et clamp de la rotation
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
		currentCameraRotationX -= cameraRotationX;
		currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

		//changement de la cam après le clamp
		cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
	}

	public void PerformJump(float jumpSpeed)
	{
		Vector3 v = new Vector3(0, jumpSpeed, 0);

    	rb.AddForce(v);
	}
}
