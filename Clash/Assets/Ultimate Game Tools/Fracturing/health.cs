using UnityEngine;
using System.Collections;

public class health : MonoBehaviour {

    public float myhealth = 200.0f;
    public bool state = false;
    public bool collider = true;
    //GameObject fps_cam=new GameObject();
	// Use this for initialization
	void Start () {
        //fps_cam = GameObject.FindWithTag("MainCamera");
	}
	
	// Update is called once per frame
	void Update () {
	
        if(state)
        {
           // Physics.IgnoreCollision(this.GetComponent<Collider>(),fps_cam.GetComponent<Collider>());
            if (collider)
            {
                Destroy(GameObject.FindGameObjectWithTag("ColliderTest"));
                Debug.Log("aaaaaaa");
                collider = false;
            }
        }
	}

    void ApplyDamage(float damage)
    {
        if (myhealth <= 0.0)
            return;
        myhealth -= damage;
        Debug.Log("我的血量还剩"+ myhealth);
        if (myhealth <= 0.0)
        {
            state = true;
            Debug.Log("生命值为0，可以破坏");
            //Destroy(this.GetComponent<Collider>());
        }
    }
}
