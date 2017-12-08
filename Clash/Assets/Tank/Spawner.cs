using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public float waittime = 2;//游戏开始一段时间后加载
    //public float nexttime;//动态生成时间间隔
    public int count = 3;
    public GameObject perfabs;//预加载的物体
    int num ;
    //public float lifetime = 3;
	// Use this for initialization
	void Start () {
        //tank_0 = GameObject.FindWithTag("tank_0");
        //StartCoroutine(spawns());
        //Destroy(tank_0,lifetime);
        num = 1;
        //Debug.Log("我来了");
        //perfabs = Resources.Load<GameObject>("real_tank");
        //Debug.Log(perfabs.name);
        this.InvokeRepeating("spawns", 3.0f, 3.0f);//重复执行的指令，每两秒一次，两秒后开始
	}

    /*IEnumerator spawns()
    {
        yield return new WaitForSeconds(waittime);
        while(true)
        {
            for(int i=0;i<=count;i++)
            {        
                    Vector3 shipPosition = new Vector3(Random.Range(0, 500),30, Random.Range(0, 500));//设置生成物体的随机坐标
                    Quaternion shipRotation = Quaternion.Euler(Random.Range(0,0), 0, Random.Range(0, 0));//设置生成物体的随机角度
                    Instantiate(Resources.Load<GameObject>("real_tank"), shipPosition, shipRotation);//生成物体
                    Debug.Log("坦克"+i+"生成中");
                    yield return new WaitForSeconds(3);//限制生成时间间隔

            }
        }
    }*/
	
	// Update is called once per frame
	void Update () {
	
	}

    void spawns()
    {
        if (num<=count)//ok了，动态生成坦克，实例化perfabs
        {
                Vector3 shipPosition = new Vector3(Random.Range(0, 500), 30, Random.Range(0, 500));//设置生成物体的随机坐标
                Quaternion shipRotation = Quaternion.Euler(Random.Range(0, 0), 0, Random.Range(0, 0));//设置生成物体的随机角度
                Debug.Log("坦克" + num + "生成中");
                Instantiate(perfabs, shipPosition, shipRotation);//生成物体
                num++;
        }
    }
}
