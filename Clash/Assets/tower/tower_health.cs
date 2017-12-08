using UnityEngine;
using System.Collections;

public class tower_health : MonoBehaviour {

	// Use this for initialization
    public float myhealth = 5000.0f;//生命值
    public bool state = false;

    void Start()
    {
        //fps_cam = GameObject.FindWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {

        if (state)
        {
            // Physics.IgnoreCollision(this.GetComponent<Collider>(),fps_cam.GetComponent<Collider>());
            if (this.GetComponent<Collider>() != null)
            {
                Debug.Log("aaaaaaa");
                Destroy(this.GetComponent<Collider>());
            }
        }
    }

    void OnGUI()
    {
        GUI.skin.label.normal.textColor = Color.red;
        // 后面的color为 RGBA的格式，支持alpha，取值范围为浮点数： 0 - 1.0.
        GUI.skin.label.fontSize = 30;
        GUI.skin.label.alignment = TextAnchor.UpperCenter;
        GUI.Label(new Rect(0, 0, 200, 60), "Health:"+myhealth.ToString());
    }

    void ApplyDamage(float damage)
    {
        if (myhealth <= 0.0)
            return;
        myhealth -= damage;
        Debug.Log("我的血量还剩" + myhealth);
        if (myhealth <= 0.0)
        {
            state = true;
            Debug.Log("生命值为0，可以破坏");
            //Destroy(this.GetComponent<Collider>());
        }
    }
}
