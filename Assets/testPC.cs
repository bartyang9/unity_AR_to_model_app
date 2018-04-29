using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using Meta;
//using pcl;

public class testPC : MetaBehaviour
{
    
    [DllImport("wrapper", EntryPoint = "detectplane")]
    public unsafe static extern int detectplane(float* points, int size, float* transInfo);
    [DllImport("wrapper", EntryPoint = "dataConverter")]
    public unsafe static extern int dataConverter(float* points, int size, float* vir_pose, float* initial_guess, float* output_pose, bool isFirst);

    bool isFirst = true;
    bool pause = true;
    bool pause2 = true;
    int cnt = 0;
    float[] real_obj = new float [16];

    void Update()
    {
        GameObject myObject = GameObject.Find("object");

        if (pause == false)
        {
            Debug.Log("start the app");
            Time.timeScale = 1;

            //Debug.Log("start the main function");
            //Time.timeScale = 1;
            
            cnt = cnt + 1;
            
            if (cnt % 200 == 0)
            {
                Debug.Log("enter the pointclouddata");
                PointCloudMetaData metadata = new PointCloudMetaData();
                metaContext.Get<InteractionEngine>().GetCloudMetaData(ref metadata);
                PointCloudData<PointXYZConfidence> pointCloudData = new PointCloudData<PointXYZConfidence>(metadata.maxSize);
                metaContext.Get<InteractionEngine>().GetCloudData(ref pointCloudData);
                //Debug.Log(pointCloudData.size);
                //Use point cloud data here

                /*
                for (int i = 0; i < pointCloudData.size; i++)
                {
                    string pointinfo = pointCloudData.points[i].ToString();
                    Debug.Log(i);
                    System.IO.File.WriteAllText(path: @"D:\test1.txt", contents: pointinfo);
                }*/
                /*
                var type = pointCloudData.points[0].vertex.GetType();
                Debug.Log(type);
                var sizetype = pointCloudData.points[0].vertex[0].GetType();
                Debug.Log("xxxxxxx"+sizetype);
                */
                int length = pointCloudData.points.Length;
                int size = pointCloudData.size;
                Debug.Log("point_size=" + size);
                Debug.Log("point_length=" + length);

                float[] psource = new float[size * 3];
                float[] transInfo = new float[7];
                unsafe
                {
                    fixed (float* temp_p = psource)
                    {
                        fixed (float* transinfo = transInfo)
                        {
                            float* pt = temp_p;
                            int num = 0;
                            foreach (PointXYZConfidence point in pointCloudData.points)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    if (i == 0)
                                    {
                                        *pt = point.vertex[i];
                                    }
                                    else if (i == 1)
                                    {
                                        *pt = -point.vertex[i];
                                    }
                                    else
                                    {
                                        *pt = point.vertex[i];
                                    }
                                    pt++;
                                    num++;
                                }
                                if (num >= size * 3) break;
                            }
                            Debug.Log("num:" + num);
                            pt = null;
                            //var len = (pt - psource);
                            //Debug.Log("length between pt and psource="+len);
                            //Debug.Log(num);
                            //float[] pF = null;
                            //int siz = 0;
                            if (pause2 == false)
                            {
                                Debug.Log("Am I here?");
                                float[] vir_pose = new float[7];
                                
                                fixed (float* vir_pose_ = vir_pose)
                                {
                                    fixed (float* initial_guess_= real_obj)
                                    {
                                        
                                        vir_pose[0] = myObject.transform.position.x;
                                        vir_pose[1] = myObject.transform.position.y;
                                        vir_pose[2] = myObject.transform.position.z;
                                        vir_pose[3] = myObject.transform.rotation.w;
                                        vir_pose[4] = myObject.transform.rotation.x;
                                        vir_pose[5] = myObject.transform.rotation.y;
                                        vir_pose[6] = myObject.transform.rotation.z;

                                        float[] output_pose = new float[16];
                                        fixed (float* output_pose_ = output_pose)
                                        {
                                            dataConverter(temp_p, num, vir_pose_, initial_guess_, output_pose_, isFirst);
                                        }
                                        for (int i = 0; i < 16; i++)
                                        {
                                            real_obj[i] = output_pose[i];
                                        }

                                        if (isFirst) isFirst = false;
                                        
                                    }
                                    
                                }
                                
                            }
                            else
                            { 
                                detectplane(temp_p, num, transinfo);
                                Vector3 trans_cam2obj = new Vector3(transinfo[0], transinfo[1], transinfo[2] + 0.05f);
                                Quaternion rot_cam2obj = new Quaternion(transinfo[3], transinfo[4], transinfo[5], transinfo[6]);

                                GameObject myCam = GameObject.Find("myCamera");
                                //Vector3 trans_world2cam = myCam.transform.position;
                                //Quaternion rot_world2cam = myCam.transform.rotation;// * tempQ;
                                myObject.transform.rotation = myCam.transform.rotation;
                                myObject.transform.position = myCam.transform.position;
                                

                                myObject.transform.Translate(trans_cam2obj, Space.Self);
                                myObject.transform.Rotate(rot_cam2obj.eulerAngles, Space.Self);
                                
                                

                                //-------------------------------------------
                                //myObject.transform.rotation = rot_world2cam;
                                //myObject.transform.position = trans_world2cam;


                                //Quaternion new_rot_cam2obj = Quaternion.Inverse(rot_cam2obj);

                                //myObject.transform.localPosition = trans_cam2obj;
                                //myObject.transform.localRotation = rot_cam2obj;
                                //myObject.transform.rotation = rot_world2cam*rot_cam2obj;
                                //myObject.transform.position = myCam.transform.TransformPoint(trans_cam2obj);
                                //Debug.Log("xyz"+myObject.transform.position.x + myObject.transform.position.y + myObject.transform.position.z);

                            }
                        }
                    }

                }
            }
        }

        else
        {
            Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pause == true)
            {
                pause = false;
            }
            else
            {
                pause = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (pause == true)
            {
                pause = true;
            }
            else
            {
                pause = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (pause2 == true)
            {
                pause2 = false;
            }
            else
            {
                pause2 = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (pause2 == true)
            {
                pause2 = true;
            }
            else
            {
                pause2 = true;
            }
        }
    }
}