using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TouchFire : MonoBehaviour, IPointerDownHandler, IPointerUpHandler{

    bool touched;
    int pointerID;
    bool canFire;


    void Awake()
    {
        canFire = false;
        touched = false;
    }

    public void OnPointerDown(PointerEventData data)//IPointerDownHandler
    {
        if (!touched)
        {
            touched = true;
            pointerID = data.pointerId;
            canFire = true;
        }
    }


    public void OnPointerUp(PointerEventData data)//IPointerUpHandler
    {
        //reset
        if (data.pointerId == pointerID)
        {
            touched = false;
            canFire = false;
        }
    }

    public bool CanFire()
    {
        return canFire;
    }

}
