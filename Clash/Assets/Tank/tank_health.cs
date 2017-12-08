using UnityEngine;
using System.Collections;

public class tank_health : MonoBehaviour {

    public float myhealth = 40.0f;
    public float damage = 50.0f;
    public GameObject attack_aim;
    public bool alive=true;//生死状态
    public bool aim_state = false;//角度状态
	public bool distance_state=false;//距离状态
	//public float rotate_speed=1.0f;//旋转速度
	Vector3 location;//目标点位置
	Vector3 direction;//向量
	public float angle;//水平偏差角
	public float attack_distance=120.0f;//攻击距离
    public float distance;//坦克距离目标点的水平距离，distance from tank to location
    public float speed = 10.0f;//坦克移动速度，等我把刚体研究明白我就用force

    public GameObject boom;//攻击粒子

    RaycastHit hit;//射线打击点

	// Use this for initialization
	void Start () {
        attack_aim = GameObject.FindWithTag("tower");
        location = attack_aim.transform.position;//获取目标地坐标点位置
        direction = location - this.transform.position;//获取向量
        angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;//计算角度,无误
        Debug.Log("偏差角为" + angle + location.x + location.y + location.z + "   " + this.transform.position.y);
        this.InvokeRepeating("attack",2.0f,2.0f);//重复执行的指令，每两秒一次，两秒后开始,invoke只执行一次
	}
	
	// Update is called once per frame
	void Update () {
        direction = location - this.transform.position;//获取向量
        distance = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);//水平相对距离
        if(!aim_state)//未对准目标先旋转位置对准目标，这地方弄麻烦了，可以直接lookat，不过我这么弄对以后有帮助
        {
			//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,angle,0), Time.deltaTime*5);//三个参数，第一个初始角度，第二个最终角度，第三个为旋转持续时间
        }
        transform.LookAt(attack_aim.transform);

        //if (Mathf.Abs(transform.rotation.y) >= Mathf.Abs(angle))//这东西似乎没发挥作用
            //aim_state = true;
		if (!distance_state) {
            //Debug.Log("w zenmejinbuqu");
            if (distance <= attack_distance)
            {
                distance_state = true;
            }
            else
            {
                //Debug.Log("我也想动啊");
                this.transform.Translate(Vector3.forward * speed * Time.deltaTime);//没办法，先用最笨的办法
                //Debug.Log("距离为" + distance);
            }
		}
        if (distance > attack_distance)
            distance_state = false;
        Vector3 realposition = transform.position;//坦克射线发射点,应该是无视自己的collider
        realposition.y += 3;
        //realposition.z += 9.3f;//坦克射线发射点最好在collider外面
        Vector3 fwd = transform.TransformDirection(Vector3.forward);//获取坦克指向方向

		if (Physics.Raycast(realposition, fwd, out hit, Mathf.Infinity))//射线击中了有效的collider
        {
            //Debug.DrawLine(transform.position, hit.point, Color.red);
           //Debug.Log("I found you!"+hit.collider.gameObject.name);
        }
        if(!alive)
        {
            Debug.Log("坦克已被击杀");
            Instantiate(boom, transform.position, Quaternion.Euler(0,0,0));//生成粒子
            Destroy(this.gameObject);
        }
	}
    bool attack()
    {
        if (distance <= attack_distance)
        {
            Debug.Log("开炮");
            if (hit.collider.gameObject.name == "tower")
            {
                Quaternion sRotation = Quaternion.Euler(Random.Range(0, 0), 0, Random.Range(0, 0));//设置生成物体的随机角度
                Instantiate(boom, hit.point, sRotation);//生成物体
            }
            hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);//不接收返回的报错消息;
            return true;
        }
        else
        {
            if (hit.collider.gameObject.name == "tower")
                Debug.Log("我扫到了就是不打");
            Debug.Log("我还没到");
            return false;
        }
    }
    void ApplyDamage(float damage)
    {
        if (myhealth <= 0.0)
            return;
        myhealth -= damage;
        Debug.Log("我的血量还剩" + myhealth);
        if (myhealth <= 0.0)
        {
            alive = false;
            Debug.Log("生命值为0，可以破坏");
            //Destroy(this.GetComponent<Collider>());
        }
    }
}
