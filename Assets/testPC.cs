using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
using Meta;
[RequireComponent(typeof(LineRenderer))]

//using pcl;

public class testPC : MetaBehaviour
{
    
    [DllImport("wrapper", EntryPoint = "detectplane")]
    public unsafe static extern int detectplane(float* points, int size, float* transInfo);
    [DllImport("wrapper", EntryPoint = "dataConverter")]
    public unsafe static extern float[] dataConverter(float* points, int size, float* initial_guess, bool isFirst);

    bool isFirst = true;
    bool pause = true;
    bool pause2 = true;
    int cnt = 0;
    float[] real_obj = new float[7];
    //LineRenderer line;
    float pre_score = 0;
    Vector3 prev_frame = new Vector3(10,10,10);
    void Update()
    {
        GameObject myObject = GameObject.Find("object");
        GameObject myCam = GameObject.Find("myCamera");
        GameObject my_real_object = GameObject.Find("realObject");
        GameObject Line = GameObject.Find("Line");
        GameObject rotLine = GameObject.Find("rotLine");

        //var line = my_real_object.AddComponent<LineRenderer>();
        //Transform my_real_object;

        if (pause == false)
        {
            //Debug.Log("start the app");
            Time.timeScale = 1;

            //Debug.Log("start the main function");
            //Time.timeScale = 1;
            
            cnt = cnt + 1;
            
            if (cnt % 25 == 0)
            {
                //Debug.Log("enter the pointclouddata");
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
                //Debug.Log("point_size=" + size);
                //Debug.Log("point_length=" + length);

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
                            //Debug.Log("num:" + num);
                            pt = null;
                            //var len = (pt - psource);
                            //Debug.Log("length between pt and psource="+len);
                            //Debug.Log(num);
                            //float[] pF = null;
                            //int siz = 0;
                            if (pause2 == false)
                            {
                                Debug.Log("Am I here?");
                                
                                fixed (float* initial_guess_= real_obj)
                                {
                                    real_obj = dataConverter(temp_p, num, initial_guess_, isFirst);
                                    //var str = "pose_output:";
                                    //for (var i = 0; i < real_obj.Length; i++)
                                    //{
                                    //    str = str + real_obj[i];
                                    //}
                                    //Debug.Log("output_pose0:" + real_obj[0]);// + "," + real_obj[2] + "," + real_obj[3] + "," + real_obj[4]);// + "," + real_obj[5] + "," + real_obj[6] + "," + real_obj[7] + "," + real_obj[8] + "," + real_obj[9] + "," + real_obj[10] + "," + real_obj[11] + "," + real_obj[12] + "," + real_obj[13] + "," + real_obj[14] + "," + real_obj[15] + ".");
                                    //Debug.Log("output_pose1:" + real_obj[1]);
                                    //Debug.Log("output_pose2:" + real_obj[2]);
                                    //Debug.Log("output_pose3:" + real_obj[3]);
                                    //Debug.Log("output_pose4:" + real_obj[4]);
                                    //Debug.Log("output_pose5:" + real_obj[5]);
                                    //Debug.Log("output_pose6:" + real_obj[6]);
                                    //Debug.Log("output_pose7:" + real_obj[7]);
                                    //Debug.Log("output_pose8:" + real_obj[8]);
                                    //Debug.Log("output_pose9:" + real_obj[9]);
                                    //Debug.Log("output_pose10:" + real_obj[10]);
                                    //Debug.Log("output_pose11:" + real_obj[11]);
                                    //Debug.Log("output_pose:" + real_obj[12]);
                                    //Debug.Log("output_pose:" + real_obj[13]);
                                    //Debug.Log("output_pose:" + real_obj[14]);
                                    //Debug.Log("output_pose:" + real_obj[15]);
                                    if (isFirst) isFirst = false;
                                    
                                }


                                //===================== visualization ==============================
                                //float[] vir_pose = new float[7];
                                //vir_pose[0] = myObject.transform.position.x;
                                //vir_pose[1] = myObject.transform.position.y;
                                //vir_pose[2] = myObject.transform.position.z;
                                //vir_pose[3] = myObject.transform.rotation.w;
                                //vir_pose[4] = myObject.transform.rotation.x;
                                //vir_pose[5] = myObject.transform.rotation.y;
                                //vir_pose[6] = myObject.transform.rotation.z;
                                if (real_obj[0] == -1) return;

                                Quaternion rot_real = new Quaternion(real_obj[1], real_obj[2], real_obj[3], real_obj[0]);
                                Vector3 trans_real = new Vector3(real_obj[4], real_obj[5], real_obj[6]);
                                float score = real_obj[7];
                                float distance_real_to_model = (prev_frame - trans_real).magnitude;
                                bool inFOV = trans_real.z > 0.12 && trans_real.x <  trans_real.z && trans_real.x > - trans_real.z && trans_real.y <  0.6*trans_real.z && trans_real.y > -0.6 * trans_real.z;

                                if (score < 0.001 && distance_real_to_model< 0.1 && inFOV)
                                {
                                    my_real_object.transform.rotation = myCam.transform.rotation;
                                    my_real_object.transform.position = myCam.transform.position;
                                
                                    my_real_object.transform.Translate(trans_real, Space.Self);
                                    my_real_object.transform.Rotate(rot_real.eulerAngles,Space.Self);

                                    //line.sortingLayerName = "OnTop";
                                    //line.sortingOrder = 5;
                                    var line = Line.GetComponent<LineRenderer>();
                                    line.sortingOrder = 1;
                                    var rotline = rotLine.GetComponent<LineRenderer>();
                                    rotline.sortingOrder = 1;
                                    //line.material = new Material(Shader.Find("Sprites/Default"));
                                    //line.sortingLayerName = "Foreground";
                                    if (distance_real_to_model > 0.15)
                                    {
                                        line.material.color = Color.red;
                                    }
                                    else if (distance_real_to_model > 0.05)
                                    {
                                        line.material.color = Color.yellow;
                                    }
                                    else
                                    {
                                        line.material.color = Color.green;
                                    }

                                    Quaternion rot_diff = Quaternion.Inverse(my_real_object.transform.rotation) * myObject.transform.rotation;
                                    Vector3 rot_axis = new Vector3(0,0,0);
                                    float rot_angle =0;
                                    rot_diff.ToAngleAxis(out rot_angle, out rot_axis);


                                    var point1 = my_real_object.transform.position;
                                    var point2 = myObject.transform.position;
                                    point1.y += 0.05f;
                                    point2.y += 0.05f;
                                    line.positionCount = 2;
                                    line.SetPosition(0, point1);
                                    line.SetPosition(1, point2);
                                    line.SetWidth(0.025f, 0.005f);
                                    line.useWorldSpace = true;

                                    var point2r = point1 + 0.05f * rot_angle * rot_axis;
                                    rotline.positionCount = 2;
                                    rotline.SetPosition(0, point1);
                                    rotline.SetPosition(1, point2r);
                                    rotline.SetWidth(0.025f, 0.005f);
                                    rotline.useWorldSpace = true;

                                    Debug.Log("the pre score is :" + pre_score);
                                    Debug.Log("The curr score is:" + score);
                                    
                                }
                                prev_frame = trans_real;
                                pre_score = score;
                                
                            }
                            else
                            { 
                                detectplane(temp_p, num, transinfo);
                                Vector3 trans_cam2obj = new Vector3(transinfo[0], transinfo[1], transinfo[2]);
                                Quaternion rot_cam2obj = new Quaternion(transinfo[3], transinfo[4], transinfo[5], transinfo[6]);
                                
                               
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
        if (Input.GetKeyDown(KeyCode.U))
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
        if (Input.GetKeyDown(KeyCode.I))
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