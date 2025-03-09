using UnityEngine;
using System.Collections;

public class CheckIfVisible : MonoBehaviour {

   // public bool destroyWhenOutScreen = false;
    public GameObject mainObject;
    public Behaviour[] components;
  //  MeshRenderer myRenderer;
   // bool visible;

    void Awake()
    {
       // myRenderer = GetComponent<MeshRenderer>();
        //visible = false;

        for (int i = 0; i < components.Length; i++)
            components[i].enabled = false;

    }

    void Start()
    {


    }


    void OnBecameVisible()
    {
      //  print(name + " OnBecameVisible");

        for (int i = 0; i < components.Length; i++)
            components[i].enabled = true;

        //mainObject.SetActive(true);

    }

    void OnBecameInvisible()
    {
       // print(name + " OnBecameInvisible");

        for (int i = 0; i < components.Length; i++)
            components[i].enabled = false;


           // mainObject.SetActive(false);
    }
}
