using UnityEngine;
using System.Collections;



namespace shmup
{
    public class PlayerController : MonoBehaviour {

        public Transform scrollingParent;

    public enum InputType
    {
        //only for computer
        Buttons, //keyboard or gamepad
        MouseAndFollowTheTouch,
        //only for mobile:
        Accelerometer,
        VirtulaButtons,
        
    }
    public InputType inputType;
    public bool pauseWhenNoTouch; //for mobile

    public float speed;
    public Transform avatar;
    public float tilt;


    bool pressFire;
    static readonly string Fire1 = "Fire1";
    bool pressSecondaryFire;
    static readonly string Fire2 = "Fire2";
    Arsenal arsenal;

    public TouchPad touchPad;
    public TouchFire touchFire;
    public TouchFire touchFireB;

        Rigidbody rb;
    Quaternion calibrationQuaternion;
    Quaternion startQuaternion;
    public static bool inputOn;

    void Start()
    {
            
        rb = GetComponent<Rigidbody>();
        arsenal = GetComponent<Arsenal>();
        pressFire = false;
        pressSecondaryFire = false;

            inputOn = true;
            startQuaternion = avatar.localRotation;

        if (inputType == InputType.Accelerometer)
            CalibrateAccelerometer();

        if (inputType == InputType.VirtulaButtons)
            touchPad.transform.parent.gameObject.SetActive(true);
        else
            touchPad.transform.parent.gameObject.SetActive(false);
    }

    void CheckPause()
        {
            if (ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.sceneEnd)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {

                bool openPauseScreen = false;
                if (ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.Playing)
                    openPauseScreen = true;

                ShumpSceneManager.sceneManager.Pause(openPauseScreen);
            }

            if (pauseWhenNoTouch)
            {
                if (Input.touchCount == 0 && ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.Playing)
                    ShumpSceneManager.sceneManager.Pause(true);
                else if (Input.touchCount > 0 && ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.Paused)
                        ShumpSceneManager.sceneManager.Pause(false);
            }
        }

    void Update()
    {
        if (ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.WaitingStart)
            return;

            CheckPause();

            if (ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.Paused)
                return;

            if (!inputOn)
                return;

        if (inputType == InputType.Buttons || inputType == InputType.MouseAndFollowTheTouch || inputType == InputType.Accelerometer)
            {
            pressFire = Input.GetButton(Fire1);
            pressSecondaryFire = Input.GetButton(Fire2);
            }
        else if (inputType == InputType.VirtulaButtons)
            {
                pressFire = touchFire.CanFire();
                pressSecondaryFire = touchFireB.CanFire();
            }

            if (pressSecondaryFire)
                pressFire = false;

        arsenal.PressFire(pressFire);
        arsenal.PressSecondaryFire(pressSecondaryFire);
    }

    // use FixedUpdate() because you use rigidbody physic to move the ship
    void FixedUpdate() {


            if (!inputOn || ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.WaitingStart || ShumpSceneManager.sceneManager.currentSceneStatus == ShumpSceneManager.SceneStatus.Paused)
                {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                return;
                }


            if (inputType == InputType.Buttons)
            {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            rb.linearVelocity = ((transform.forward * moveVertical)+ (transform.right * moveHorizontal)) * speed;

            }
        else if (inputType == InputType.MouseAndFollowTheTouch)
            {
            float precision = 0.025f;

            float mousePositionX = Input.mousePosition.x / Screen.width;
            float mousePositionY = Input.mousePosition.y / Screen.height;

            float moveHorizontal = 0;
            float moveVertical = 0;

            float targetHorizontal = 0;
            float targetVertical = 0;

            if (mousePositionX - precision > Camera.main.WorldToViewportPoint(this.transform.position).x)
                targetHorizontal = 1;//print("R");
            else if (mousePositionX + precision < Camera.main.WorldToViewportPoint(this.transform.position).x)
                targetHorizontal = -1;//print("L");
            
            if (mousePositionY - precision > Camera.main.WorldToViewportPoint(this.transform.position).y)
                targetVertical = 1;//print("U");
            else if (mousePositionY + precision < Camera.main.WorldToViewportPoint(this.transform.position).y)
                targetVertical = -1;//print("D");

            moveHorizontal = Mathf.Lerp(moveHorizontal, targetHorizontal, Time.time);
            moveVertical = Mathf.Lerp(moveVertical, targetVertical, Time.time);


                //move
                rb.linearVelocity = ((transform.forward * moveVertical) + (transform.right * moveHorizontal)) * speed;

            }
        else if (inputType == InputType.Accelerometer)
            {
            Vector3 accelerationRaw = Input.acceleration;
            Vector3 acceleration = FixAcceleration(accelerationRaw);
                //rb.velocity = new Vector3(acceleration.x, 0, acceleration.y) * speed;
                rb.linearVelocity = ((transform.forward * acceleration.y) + (transform.right * acceleration.x)) * speed;
            }
        else if (inputType == InputType.VirtulaButtons)
            {
            Vector2 direction = touchPad.GetDirection();
            rb.linearVelocity = ((transform.forward * direction.y) + (transform.right * direction.x)) * speed;
            }





            //tilt
            Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
            avatar.localRotation = Quaternion.Euler(0, scrollingParent.rotation.y, localVelocity.x * -tilt);

        }

    public void InputOnOff(bool _inputOn)
        {
            //print("InputOnOff = " + _inputOn);
            inputOn = _inputOn;
            if (!inputOn)
                {
                pressFire = false;
                pressSecondaryFire = false;
                arsenal.PressFire(pressFire);
                arsenal.PressSecondaryFire(pressSecondaryFire);
                avatar.localRotation = startQuaternion ;
                }
        }

    void CalibrateAccelerometer()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0, 0, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }

    Vector3 FixAcceleration(Vector3 acceleration)
    {
        Vector3 fixedAcceleration = calibrationQuaternion * acceleration;
        return fixedAcceleration;
    }
}
}
