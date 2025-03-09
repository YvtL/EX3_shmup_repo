using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class TouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {

    public float smoothing;

    Vector2 origin;
    Vector2 direction;
    Vector2 smoothDirection;

    bool touched;
    int pointerID;

    void Awake()
    {
        direction = Vector2.zero;
        touched = false;
    }

    public void OnPointerDown(PointerEventData data)//IPointerDownHandler
    {
        if(!touched)
        {
            touched = true;
            pointerID = data.pointerId;
            //start point of the touch (will be the center of the virtual joystick)
            origin = data.position;
        }
    }


    public void OnDrag(PointerEventData data)//IDragHandler
    {
        if(data.pointerId == pointerID)//use only one touch
        {
            //move direction
            Vector2 currentPosition = data.position;
            Vector2 directionRaw = currentPosition - origin;
            direction = directionRaw.normalized;//max magnitude = 1
        }
    }


    public void OnPointerUp(PointerEventData data)//IPointerUpHandler
    {
        //reset
       if (data.pointerId == pointerID)
            {
            touched = false;
            direction = Vector2.zero;
            }
    }

    public Vector2 GetDirection()
    {
        smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothing);
        return smoothDirection;
    }
}
