using UnityEngine;
using System.Collections;

public class KinectOverlayer : MonoBehaviour 
{
//	public Vector3 TopLeft;
//	public Vector3 TopRight;
//	public Vector3 BottomRight;
//	public Vector3 BottomLeft;

	public GUITexture backgroundImage;//GUI图像，如果是我个人的话，不光要有背景图，还得有前景图,注意，backgroundImage在另一个Camera中渲染，而且似乎两者的距离不能差太多，至少这个测试里两者的x是一定的
	public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.HandRight;//追踪用骨骼节点
	public GameObject OverlayObject;
	public float smoothFactor = 5f;
	
	public GUIText debugText;

	private float distanceToCamera = 10f;


	void Start()
	{
		if(OverlayObject)
		{
			distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;//计算控制物体到相机的距离
		}
	}
	
	void Update() 
	{
		KinectManager manager = KinectManager.Instance;//相机初始化
		
		if(manager && manager.IsInitialized())
		{
			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			if(backgroundImage && (backgroundImage.texture == null))//有背景图且背景图没有纹理
			{
				backgroundImage.texture = manager.GetUsersClrTex();
                //backgroundImage.texture = manager.GetUsersLblTex();//获取深度数据流
			}
			
//			Vector3 vRight = BottomRight - BottomLeft;
//			Vector3 vUp = TopLeft - BottomLeft;
			
			int iJointIndex = (int)TrackedJoint;//获取节点在骨骼信息中的编号
			
			if(manager.IsUserDetected())//用户被侦测到
			{
				uint userId = manager.GetPlayer1ID();//获取玩家1的ID
				
				if(manager.IsJointTracked(userId, iJointIndex))//如果骨骼节点被侦测到
				{
					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);//获取骨骼节点的深度信息，转化为三维坐标

					if(posJoint != Vector3.zero)
					{
						// 3d position to depth
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);//先将骨骼节点对应的点取出深度图转成二维坐标
						
						// depth pos to color pos
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);//将深度图中的二维深度信息转到彩色图中
						
						float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;//计算x的位置
						float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;//计算y的位置
                        float scaleZ = 1100-posJoint.z*940;//提取z轴深度信息
                        Debug.Log(scaleZ);
                        //float scaleZ = -(float)posJoint.z*10;
						
//						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
//						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
						//Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));

						if(debugText)
						{
							debugText.GetComponent<GUIText>().text = "Tracked user ID: " + userId;  // new Vector2(scaleX, scaleY).ToString();
						}
						
						if(OverlayObject)
						{
							Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, scaleZ));
							OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);//单纯的位置移动
						}
					}
				}
				
			}
			
		}
	}
}
