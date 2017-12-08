using UnityEngine;
using System.Collections;

public class Controler : MonoBehaviour {

    public float damage = 80;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    void OnTriggerEnter(Collider collider)
    {
        Collider[] aim = Physics.OverlapSphere(transform.position, 30);//返回一个数组
        for(int i=0;i<aim.Length;i++)
        {
            if(aim[i].gameObject.name=="tank"||aim[i].gameObject.name=="real_tank(Clone)")
            {
                Debug.Log("我看到你了");
                aim[i].SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
