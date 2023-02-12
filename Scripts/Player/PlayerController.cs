using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(Player))]
public class PlayerController : MonoBehaviour
{
	[SerializeField]
	float jumpSpeed;
	[SerializeField]
    float speed = 3;
	[SerializeField]
    float runSpeed = 5;
    [SerializeField]
    float xLookSensitivity = 3;
    [SerializeField]
    float yLookSensitivity = 3;

	public bool isCamCanMove = true;
	public bool canMove = true;

	public Transform[] groundedPosition;
	[SerializeField]
	float jumpRange = 0.2f;

    [SerializeField] PlayerMotor motor;
	[SerializeField] Player player;

	[SerializeField] PlayerCaracteristiqueStats playerCaracteristiqueStats;

    void Start()
    {
		isCamCanMove = true;
    }

    void Update()
    {
		if(!canMove)
		{
			motor.Rotate(new Vector3(0,0,0));
			motor.RotateCamera(0);
			motor.Move(Vector3.zero);
			return;
		}

    	//Vélocité du mouvement avec un vecteur 3D
    	float _xMov = InputManager.instance.horizontalMove;
    	float _zMov = InputManager.instance.verticalMove;

    	Vector3 _movHorizontal = transform.right * _xMov;
    	Vector3 _movVertical = transform.forward * _zMov;

		Vector3 _velocity = Vector3.zero;
		//Run
		if(Input.GetKey(InputManager.instance.run) && _zMov > 0)
		{
			_velocity = (_movHorizontal + _movVertical).normalized * (runSpeed * (1+playerCaracteristiqueStats.GetSpeedBonus(true)/100));
		}
		else if(_xMov != 0 || _zMov != 0)
		{
			_velocity = (_movHorizontal + _movVertical).normalized * (speed * (1+playerCaracteristiqueStats.GetSpeedBonus(true)/100));
		}

    	motor.Move(_velocity);
		if(isCamCanMove)
		{
			//calcule de la rotation du joueur avec un Vecteur 3D
			float _yRot = Input.GetAxisRaw("Mouse X");

			Vector3 _rotation = new Vector3(0, _yRot, 0) * xLookSensitivity;

			motor.Rotate(_rotation);

			//calcule de la rotation de la camera avec un Vecteur 3D
			float _xRot = Input.GetAxisRaw("Mouse Y");

			float _camerarotationX = _xRot * yLookSensitivity;

			motor.RotateCamera(_camerarotationX);
		}
		else
		{
			motor.Rotate(new Vector3(0,0,0));
			motor.RotateCamera(0);
		}
    	//Saut
    	if(Input.GetKeyDown(InputManager.instance.jump) && IsGrounded())
    	{
    		motor.PerformJump(jumpSpeed);
    	}
    }

	bool IsGrounded()
	{
		for(int i=0; i < groundedPosition.Length; i++)
		{
			RaycastHit hit;
			if(Physics.Raycast(groundedPosition[i].position, Vector3.down, out hit, jumpRange))
			{
				if(hit.transform.gameObject.tag == "Ground")
				{
					return true;
				}
				else if(i+1 == groundedPosition.Length)
				{
					return false;
				}
			}
		}
		return false;
	}
}
