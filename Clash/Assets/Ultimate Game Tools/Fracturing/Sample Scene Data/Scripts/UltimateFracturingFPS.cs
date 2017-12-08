using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UltimateFracturing;

// Assign this script to a Camera to activate some simple FPS behavior, and interaction with fractured objects.

public class UltimateFracturingFPS : MonoBehaviour
{
    public enum Mode
    {
        ShootObjects,       // Will shoot physical objects
        ExplodeRaycast      // Will raycast against the scene, finding fractured chunks and explode them
    };

    public Mode              ShootMode          = Mode.ExplodeRaycast;  // The current shoot mode,当前射击模式，默认为放射式无重力武器
    public float             MouseSpeed         = 0.02f;                 // Mouse sensivity，鼠标感应
    public Texture           HUDTexture;                                // The texture to draw at the center
    public float             HUDSize            = 0.03f;                // The size of the HUD to draw
    public Color             HUDColorNormal;                            // HUD color when no raycast chunk was found
    public Color             HUDColorRaycast;                           // HUD color when raycast chunk was found
    public Transform         Weapon;                                    // The transform of the weapon, which should be child of this component assigned to a camera
    public AudioClip         AudioWeaponShot;                           // The audio clip to play when a shot is fired//音效
    public float             WeaponShotVolume   = 1.0f;                 // The volume of the weapon shot
    public float             ExplosionForce     = 1.0f;                 // The force to apply when FracturedChunk.Impact() is called.//爆炸力
    public float             ExplosionRadius    = 0.4f;                 // The radius to apply when FracturedChunk.Impact() is called.//爆炸范围
    public float             RecoilDuration     = 0.2f;                 // The length of the recoil animation in seconds，后坐力
    public float             RecoilIntensity    = 0.05f;                // The intensity of the recoil
    public GameObject        ObjectToShoot      = null;                 // In ShootObjects mode, the object to instance when shooting
    public float             InitialObjectSpeed = 1.5f;                 // In ShootObjects mode, the initial speed of the object
    public float             ObjectScale        = 1.0f;                 // In ShootObjects mode, the object's scale，大小
    public float             ObjectMass         = 1.0f;                 // In ShootObjects mode, the object's mass，重力
    public float             ObjectLife         = 10.0f;                // In ShootObjects mode, the object's life time (seconds until it deletes itself)//物体发射状态下发射的物体的生命
    public float damage = 50.0f;//伤害值

    private Vector3          m_v3MousePosition;//鼠标位置
    private bool             m_bRaycastFound;
    private float            m_fRecoilTimer;
    private Vector3          m_v3InitialWeaponPos;
    private Quaternion       m_qInitialWeaponRot;

	void Start()
    {
	    m_v3MousePosition = Input.mousePosition;
        m_bRaycastFound   = false;
        m_fRecoilTimer    = 0.0f;

        if(Weapon)
        {
            m_v3InitialWeaponPos = Weapon.localPosition;
            m_qInitialWeaponRot  = Weapon.localRotation;
        }
	}

    void OnGUI()//自动调用GUI
    {
        Color colGUI = GUI.color;

        // Draw a simple hud for aiming

        if(ShootMode == Mode.ExplodeRaycast)
        {
            int nHalfPixelSize = Mathf.RoundToInt(Screen.width * HUDSize * 0.5f);

            Rect rectPosition = new Rect((Screen.width / 2) - nHalfPixelSize, (Screen.height / 2) - nHalfPixelSize, nHalfPixelSize * 2, nHalfPixelSize * 2);
            GUI.color = m_bRaycastFound ? HUDColorRaycast : HUDColorNormal;
            GUI.DrawTexture(rectPosition, HUDTexture, ScaleMode.StretchToFill, true);
            GUI.color = colGUI;
        }

        GUI.color = colGUI;
    }

	void Update()//先执行update再执行LateUpdate，每一帧调用一次，FixedUpdate为多帧用
    {
        if(Input.GetKey(KeyCode.W))
        {
            this.transform.Translate(Vector3.forward);
        }
        if(Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(Vector3.back);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(Vector3.left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Vector3.right);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            this.transform.Translate(Vector3.up);
        }
        if(Input.GetKey(KeyCode.E))
        {
            this.transform.Translate(Vector3.down);
        }
        if(Input.GetKeyDown(KeyCode.Q))//换武器
        {
            ShootMode = ShootMode == Mode.ExplodeRaycast ? Mode.ShootObjects : Mode.ExplodeRaycast;//切换,检验为真则返回:后面的值，否则返回?后面的值
        }

        if(ObjectToShoot != null && ShootMode == Mode.ShootObjects)//投掷类武器模式
        {
            // Shoot objects

            if(Weapon) Weapon.GetComponent<Renderer>().enabled = false;

            if(Input.GetMouseButtonDown(0))
            {
                GameObject newObject = GameObject.Instantiate(ObjectToShoot) as GameObject;
                newObject.transform.position   = this.transform.position;
                newObject.transform.localScale = new Vector3(ObjectScale, ObjectScale, ObjectScale);
                newObject.GetComponent<Rigidbody>().mass       = ObjectMass;
                newObject.GetComponent<Rigidbody>().solverIterations = 255;
                newObject.GetComponent<Rigidbody>().AddForce(this.transform.forward * InitialObjectSpeed, ForceMode.VelocityChange);

                DieTimer dieTimer = newObject.AddComponent<DieTimer>() as DieTimer;//物体死亡计时器
                dieTimer.SecondsToDie = ObjectLife;
            }
        }

        if(ShootMode == Mode.ExplodeRaycast)//射线式武器
        {
            // Raycast

            if(Weapon) Weapon.GetComponent<Renderer>().enabled = true;

            bool bShot = Input.GetMouseButtonDown(0);//射击状态，按下

            if(bShot)
            {
                m_fRecoilTimer = RecoilDuration;//后坐力
                if(AudioWeaponShot) AudioSource.PlayClipAtPoint(AudioWeaponShot, transform.position, WeaponShotVolume);//播放声音
            }

            m_bRaycastFound = false;

            RaycastHit hitInfo;//光线投射碰撞，自带的

            FracturedChunk chunkRaycast = FracturedChunk.ChunkRaycast(transform.position, transform.forward, out hitInfo);//编写的类，用来获取准星中是否有已被粉碎化处理的对象

            if(chunkRaycast)
            {
                m_bRaycastFound = true;

                if(bShot)
                {
                    // Hit it!
                    //if(hitInfo.collider!=null)
                        //if(hitInfo.collider.transform.parent.gameObject)
                            chunkRaycast.Impact(hitInfo.point, ExplosionForce, ExplosionRadius, true);//设置爆炸效果
                }
            }
            else//当未发现有模型的时候就进行冲突检测
            {
                if(bShot)
                {
                    hitInfo.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);//不接收消息;
                }
            }
        }

        // Update recoil

        if(m_fRecoilTimer > 0.0f)//后坐力设定
        {
            if(Weapon)
            {
                // Some rudimentary recoil animation here
                Weapon.transform.localPosition = m_v3InitialWeaponPos + new Vector3(0.0f, 0.0f, (-m_fRecoilTimer / RecoilDuration) * RecoilIntensity);
                Weapon.transform.localRotation = m_qInitialWeaponRot * Quaternion.Euler(new Vector3((m_fRecoilTimer / RecoilDuration) * 360.0f * RecoilIntensity * 0.1f, 0.0f, 0.0f));
            }

            m_fRecoilTimer -= Time.deltaTime;
        }
        else
        {
            if(Weapon)
            {
                Weapon.transform.localPosition = m_v3InitialWeaponPos;
                Weapon.transform.localRotation = m_qInitialWeaponRot;
            }
        }

        // Mouse-aim

        //if(Input.GetMouseButton(0) && Input.GetMouseButtonDown(0) == false)//按着鼠标且未放开
        if(true)
        {
            this.transform.Rotate      (-(Input.mousePosition.y - m_v3MousePosition.y) * MouseSpeed, 0.0f, 0.0f);
            this.transform.RotateAround(this.transform.position, Vector3.up, (Input.mousePosition.x - m_v3MousePosition.x) * MouseSpeed);
        }

        m_v3MousePosition = Input.mousePosition;
	}

}
