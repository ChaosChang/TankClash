using UnityEngine;
using System.Collections;

public class AutoDestory : MonoBehaviour {

    private ParticleSystem[] particleSystems;

    void Start()
    {
        Debug.Log("我已生成");
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        bool allStopped = true;

        foreach (ParticleSystem ps in particleSystems)//遍历粒子，全播放完就销毁
        {
            if (!ps.isStopped)
            {
                allStopped = false;
            }
        }

        if (allStopped)
        {
            Debug.Log("我已销毁");
            GameObject.Destroy(gameObject);
        }
    } 
}
