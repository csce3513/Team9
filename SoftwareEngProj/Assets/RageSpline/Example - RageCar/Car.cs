using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour {
	
    public WheelCollider rearWheelCollider;
    public GameObject rearWheel;
    public WheelCollider frontWheelCollider;

    public GameObject frontWheel;
    public GameObject eyebrows;
    public GameObject leftEye;
    public GameObject rightEye;
    public Vector3 startPos;

    private Vector3 eyeBrowsStartPosition;
    private Vector3 leftEyeStartPosition;
    private Vector3 rightEyeStartPosition;

    private Vector3 leftEyeStartScale;
    private Vector3 rightEyeStartScale;
    private float startZ;

    public GameObject[] parallax;
    private Vector3[] parallaxOriginal;
    private Vector3 cameraOriginal;
    public int insideBoundsCount;
	
    void Awake() {
        startPos = transform.position;
        startZ = rearWheel.transform.position.z;
        eyeBrowsStartPosition = eyebrows.transform.localPosition;
        leftEyeStartScale = leftEye.transform.localScale;
        rightEyeStartScale = rightEye.transform.localScale;
        leftEyeStartPosition = leftEye.transform.localPosition;
        rightEyeStartPosition = rightEye.transform.localPosition;

        cameraOriginal = Camera.mainCamera.transform.position; 
        parallaxOriginal = new Vector3[parallax.Length];
        for (int i = 0; i < parallax.Length; i++)
        {
            parallaxOriginal[i] = parallax[i].transform.localPosition;
        }

        
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(rearWheelCollider.transform.position, -rearWheelCollider.transform.up, out hit, rearWheelCollider.suspensionDistance + rearWheelCollider.radius * 1.5f))
        {
            rearWheel.transform.position = Vector3.Lerp(rearWheel.transform.position, hit.point + rearWheel.transform.up * rearWheelCollider.radius * 2f, Time.deltaTime*30f);
            rearWheel.transform.position = new Vector3(rearWheel.transform.position.x, rearWheel.transform.position.y, startZ);
        }
        else
        {
            rearWheel.transform.position = Vector3.Lerp(rearWheel.transform.position, rearWheelCollider.transform.position - (rearWheel.transform.up * rearWheelCollider.suspensionDistance * 0.25f), Time.deltaTime*30f);
            rearWheel.transform.position = new Vector3(rearWheel.transform.position.x, rearWheel.transform.position.y, startZ);
        }

        RaycastHit hit2 = new RaycastHit();
        if (Physics.Raycast(frontWheelCollider.transform.position, -frontWheelCollider.transform.up, out hit2, frontWheelCollider.suspensionDistance + frontWheelCollider.radius * 1.5f))
        {
            frontWheel.transform.position = Vector3.Lerp(frontWheel.transform.position, hit2.point + frontWheel.transform.up * frontWheelCollider.radius * 2f, Time.deltaTime*30f);
            frontWheel.transform.position = new Vector3(frontWheel.transform.position.x, frontWheel.transform.position.y, startZ);
        }
        else
        {
            frontWheel.transform.position = Vector3.Lerp(frontWheel.transform.position, frontWheelCollider.transform.position - (frontWheel.transform.up * frontWheelCollider.suspensionDistance * 0.25f), Time.deltaTime*30f);
            frontWheel.transform.position = new Vector3(frontWheel.transform.position.x, frontWheel.transform.position.y, startZ);
        }

        if (Vector3.Angle(eyebrows.transform.up, Camera.main.transform.up) < 70f && rigidbody.angularVelocity.magnitude < 2f)
        {
            eyebrows.transform.localPosition = new Vector3(eyeBrowsStartPosition.x, eyeBrowsStartPosition.y - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.07f+2f, -20f, 0f), eyeBrowsStartPosition.z);
            leftEye.transform.localPosition = new Vector3(leftEyeStartPosition.x, leftEyeStartPosition.y - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.02f, -20f, 0f), leftEyeStartPosition.z);
            rightEye.transform.localPosition = new Vector3(rightEyeStartPosition.x, rightEyeStartPosition.y - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.02f, -20f, 0f), rightEyeStartPosition.z);
            leftEye.transform.localScale = new Vector3(leftEyeStartScale.x - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.00005f, -0.2f, 0f), leftEyeStartScale.y - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.0001f, -0.2f, 0f), leftEyeStartScale.z);
            rightEye.transform.localScale = new Vector3(rightEyeStartScale.x - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.00005f, -0.2f, 0f), rightEyeStartScale.y - Mathf.Clamp(transform.InverseTransformDirection(rigidbody.velocity).y * 0.0001f, -0.2f, 0f), rightEyeStartScale.z);
        }
        else
        {
            eyebrows.transform.localPosition = Vector3.Lerp(eyebrows.transform.localPosition, eyeBrowsStartPosition, Time.deltaTime*5f);
            leftEye.transform.localPosition = Vector3.Lerp(leftEye.transform.localPosition, leftEyeStartPosition, Time.deltaTime * 5f);
            rightEye.transform.localPosition = Vector3.Lerp(rightEye.transform.localPosition, rightEyeStartPosition, Time.deltaTime * 5f);
            rightEye.transform.localScale = Vector3.Lerp(rightEye.transform.localScale, rightEyeStartScale, Time.deltaTime * 5f);
        }

        for (int i = 0; i < parallax.Length; i++)
        {
            parallax[i].transform.localPosition = parallaxOriginal[i] - (Camera.mainCamera.transform.position - cameraOriginal) * 0.0033f;
        }

    }

    void FixedUpdate()
	{

        if(Vector3.Angle(transform.up, Camera.main.transform.up) > 90f && rigidbody.angularVelocity.magnitude < 1f && rigidbody.velocity.magnitude < 1f) {
            ResetLevel();
        }

        if (rearWheelCollider.isGrounded)
        {
            
            if (Input.GetKey(KeyCode.Space))
            {
                if (transform.position.x < 300f)
                {
                    rearWheelCollider.motorTorque = Mathf.Clamp(rearWheelCollider.motorTorque + Time.fixedDeltaTime * 25f, 0f, 44f);
                }
                else
                {
                    rearWheelCollider.motorTorque = Mathf.Clamp(rearWheelCollider.motorTorque + Time.fixedDeltaTime * 25f, 0f, 20f);
                }
            }
            else
            {
                rearWheelCollider.motorTorque = Mathf.Clamp(rearWheelCollider.motorTorque - Time.fixedDeltaTime * 100f, 0f, 44f);
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidbody.AddTorque(new Vector3(0f, 0f, Time.fixedDeltaTime * 160f), ForceMode.Acceleration);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody.AddTorque(new Vector3(0f, 0f, -Time.fixedDeltaTime * 160f), ForceMode.Acceleration);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetLevel();
        }
	}

    public void ResetLevel()
    {
        for (int i = 0; i < parallax.Length; i++)
        {
            parallax[i].transform.position = parallaxOriginal[i];
        }

        transform.position = startPos;
        rigidbody.velocity = new Vector3();
        rigidbody.angularVelocity = new Vector3();
        transform.rotation = Quaternion.identity;
        insideBoundsCount = 0;
    }
}
