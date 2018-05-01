using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class visualization : MonoBehaviour
{
    [Tooltip("The percent of the line that is consumed by the arrowhead")]
    [Range(0, 1)]
    public float PercentHead = 0.4f;
    public Vector3 ArrowOrigin;
    public Vector3 ArrowTarget;
    
    bool pause = true;
    bool pause2 = true;
    private LineRenderer cachedLineRenderer;
    void Start()
    {
        UpdateArrow();
    }
    void Update()
    {
        
        if (pause == false && pause2 == false)
        {
            Time.timeScale = 1;
            UpdateArrow();
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
    [ContextMenu("UpdateArrow")]
    void UpdateArrow()
    {
        GameObject myObject = GameObject.Find("object");
        GameObject my_real_object = GameObject.Find("realObject");
        ArrowOrigin = my_real_object.transform.position;
        ArrowTarget = myObject.transform.position;
        if (cachedLineRenderer == null)
            cachedLineRenderer = this.GetComponent<LineRenderer>();
        cachedLineRenderer.widthCurve = new AnimationCurve(new Keyframe(0, 0.4f), new Keyframe(0.999f - PercentHead, 0.4f)  // neck of arrow
                                                            , new Keyframe(1 - PercentHead, 1f)  // max width of arrow head
                                                            , new Keyframe(1, 0f));  // tip of arrow
        cachedLineRenderer.SetPositions(new Vector3[] {
              ArrowOrigin
              , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.999f - PercentHead)
              , Vector3.Lerp(ArrowOrigin, ArrowTarget, 1 - PercentHead)
              , ArrowTarget });
    }
}
