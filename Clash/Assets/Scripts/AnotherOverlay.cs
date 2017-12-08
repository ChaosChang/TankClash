using UnityEngine;
using System.Collections;
//using System.Drawing;

public class AnotherOverlay : MonoBehaviour {

    public GUITexture backgroundImage;//GUI图像，如果是我个人的话，不光要有背景图，还得有前景图,注意，backgroundImage在另一个Camera中渲染，而且似乎两者的距离不能差太多，至少这个测试里两者的x是一定的
    public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.HandRight;//追踪用骨骼节点
    public GameObject OverlayObject;
    public float smoothFactor = 5f;

    public GUIText debugText;

    private float distanceToCamera = 10f;


    void Start()
    {
        if (OverlayObject)
        {
            distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;//计算控制物体到相机的距离
        }
    }

    void Update()
    {
        KinectManager manager = KinectManager.Instance;//相机初始化

        if (manager && manager.IsInitialized())
        {
            //backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
            if (backgroundImage && (backgroundImage.texture == null))//有背景图且背景图没有纹理
            {
                //backgroundImage.texture = manager.GetUsersClrTex();
                //backgroundImage.texture = MedianFilterFunction(manager.GetUsersLblTex(),1);//获取深度数据流，这里有问题，深度数据流我没办法确定它是纯深度信息还是带有色彩值
                backgroundImage.texture = manager.GetUsersLblTex();
            }

            //			Vector3 vRight = BottomRight - BottomLeft;
            //			Vector3 vUp = TopLeft - BottomLeft;

            int iJointIndex = (int)TrackedJoint;//获取节点在骨骼信息中的编号

            if (manager.IsUserDetected())//用户被侦测到
            {
                uint userId = manager.GetPlayer1ID();//获取玩家1的ID

                if (manager.IsJointTracked(userId, iJointIndex))//如果骨骼节点被侦测到
                {
                    Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);//获取骨骼节点的深度信息，转化为三维坐标

                    if (posJoint != Vector3.zero)
                    {
                        // 3d position to depth
                        Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);//先将骨骼节点对应的点取出深度图转成二维坐标

                        // depth pos to color pos
                        Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);//将深度图中的二维深度信息转到彩色图中

                        float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;//计算x的位置
                        float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;//计算y的位置
                        float scaleZ = 1100 - posJoint.z * 940;//提取z轴深度信息
                        Debug.Log(scaleZ);
                        //float scaleZ = -(float)posJoint.z*10;

                        //						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
                        //						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
                        //Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));

                        if (debugText)
                        {
                            debugText.GetComponent<GUIText>().text = "Tracked user ID: " + userId;  // new Vector2(scaleX, scaleY).ToString();
                        }

                        if (OverlayObject)
                        {
                            Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, scaleZ));
                            OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);//单纯的位置移动
                        }
                    }
                }

            }

        }
    }
          /// <summary>
          /// 对矩阵M进行中值滤波
          /// </summary>
          /// <param name="m">矩阵M</param>
          /// <param name="windowRadius">过滤半径</param>
          /// <returns>结果矩阵</returns>
          
          private Texture2D MedianFilterFunction(Texture2D m, int windowRadius)//Radius一般都是奇数，3居多
         {
             int width = m.width;
             int height = m.height;
             Texture2D neww=new Texture2D(m.width,m.height);
              //byte[,] lightArray = new byte[width, height];
  
              //开始滤波
              for (int i = 0; i <= width - 1; i++)
              {
                  for (int j = 0; j <= height - 1; j++)
                  {
                      //得到过滤窗口矩形
                      int lefttop_width = i - windowRadius;//左上定点坐标宽高位置
                      int lefttop_height = j - windowRadius;
                      int rect_width = 2 * windowRadius + 1;//正方形的宽
                      int rect_height = 2 * windowRadius + 1;//正方形的高
                      //Rectangle rectWindow = new Rectangle(i - windowRadius, j - windowRadius, 2 * windowRadius + 1, 2 * windowRadius + 1);
                      if (lefttop_width < 0) lefttop_width = 0;
                      if (lefttop_height < 0) lefttop_height = 0;
                      if (lefttop_width + rect_width > width - 1) rect_width = width - 1 - lefttop_width;
                      if (lefttop_height + rect_width > height - 1) rect_height = height - 1 - lefttop_height;
                      //将窗口中的颜色取到列表中
                      Color [] windowPixelColorList=new Color [10];
                      float [] r=new float[10];
                      float [] g=new float[10];
                      float [] b=new float[10];
                      //ArrayList windowPixelColorList;
                      int count=0;
                      for (int oi = lefttop_width; oi < lefttop_width+rect_width; oi++)
                      {
                          for (int oj = lefttop_height; oj < lefttop_height+rect_height; oj++)
                         {
                             windowPixelColorList[count]=m.GetPixel(i,j);//获取当前像素
                              r[count]=windowPixelColorList[count].r;
                              g[count]=windowPixelColorList[count].g;
                              b[count]=windowPixelColorList[count].b;
                              count++;
                         }
                     }
                     //排序
                      int length = count;
                      count=0;
                      float exchange;
                    for(;count<r.Length;count++)//r排序
                    {
                        for(int counter=count;counter<length;counter++)
                        if(r[count]>r[counter])
                        {
                            exchange=r[count];
                            r[count]=r[counter];
                            r[counter]=exchange;
                        }
                    }
                      for(;count<length;count++)//g排序
                    {
                        for(int counter=count;counter<g.Length;counter++)
                        if(g[count]>g[counter])
                        {
                            exchange=g[count];
                            g[count]=g[counter];
                            g[counter]=exchange;
                        }
                    }
                      for(;count<length;count++)//b排序
                    {
                        for(int counter=count;counter<b.Length;counter++)
                        if(b[count]>b[counter])
                        {
                            exchange=b[count];
                            b[count]=b[counter];
                            b[counter]=exchange;
                        }
                    }
                     //windowPixelColorList.Sort();
                     //取中值
                     Color middleValue=new Color();
                     if ((windowRadius * windowRadius) % 2 == 0)
                     {
                         //如果是偶数
                         middleValue.r = (r[windowPixelColorList.Length / 2] + r[windowPixelColorList.Length / 2 - 1]) / 2;
                         middleValue.g = (g[windowPixelColorList.Length / 2] + g[windowPixelColorList.Length / 2 - 1]) / 2;
                         middleValue.b = (b[windowPixelColorList.Length / 2] + b[windowPixelColorList.Length / 2 - 1]) / 2;
                     }
                     else
                     {
                         //如果是奇数
                         middleValue.r = r[(windowPixelColorList.Length - 1) / 2];
                         middleValue.g = g[(windowPixelColorList.Length - 1) / 2];
                         middleValue.b = b[(windowPixelColorList.Length - 1) / 2];
                     }
                     //设置为中值
                     neww.SetPixel(i,j,middleValue);
                 }
             }
             return neww;
         }
}
