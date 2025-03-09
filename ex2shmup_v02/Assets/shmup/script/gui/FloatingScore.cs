using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace shmup
{
    public class FloatingScore : MonoBehaviour {

    public Text myText;
    Animation anim;

        void Awake()
        {
            anim = GetComponent<Animation>();

        }

        void OnEnable()
    {

            //get animation duration
            // AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            anim.Play();
            //Invoke("DestroyMe", clipInfo[0].clip.length);
            Invoke("DestroyMe", anim.clip.length);

        }

    void DestroyMe()
    {
            this.gameObject.SetActive(false);
            //this.transform.SetParent(ShumpSceneManager.sceneManager.scoreGarbage,false);
            
    }

    public void SetText(string text)
    {
        myText.text = text;
    }
}
}
