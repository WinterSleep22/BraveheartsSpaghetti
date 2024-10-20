using UnityEngine;
using System.Collections;

public class CameraRatioSizeAdjust : MonoBehaviour {
    public float aspectRatio;
    public float multiplierAdjust=1;
    public float offset = 448.575f;
    public float correctionMultiplier = 100;
    public float correctionOffset = -100;
	// Use this for initialization
	void Start () {
      
	}

	// Update is called once per frame
	public void Update () {
        UpdateRatio();
       
	}
    public void UpdateRatio()
    {
        
        #if UNITY_EDITOR
            aspectRatio = GetMainGameViewSizeAspectRatio();
        #else
            aspectRatio = (float)Screen.width / (float)Screen.height;
        #endif
     
        if (this.enabled)
        {

            Camera.main.orthographicSize = aspectRatio * multiplierAdjust + offset + ((aspectRatio > 1.56f) ? 0 : -1 * correctionMultiplier * aspectRatio + correctionOffset);
        }
    }
    #if UNITY_EDITOR
    public static float GetMainGameViewSizeAspectRatio()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        System.Object Res = GetSizeOfMainGameView.Invoke(null, null);
        Vector2 viewSize = (Vector2)Res;
        return viewSize.x / viewSize.y;
    }
    #endif
}





#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(CameraRatioSizeAdjust))]
[UnityEditor.CanEditMultipleObjects]
public class EditorCameraRatioSizeAdjust : UnityEditor.Editor
{
    CameraRatioSizeAdjust targetScript;
    void OnEnable()
    {
        targetScript = serializedObject.targetObject as CameraRatioSizeAdjust;

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (targetScript != null)
        {

            targetScript.UpdateRatio();
        }
    }
}

#endif