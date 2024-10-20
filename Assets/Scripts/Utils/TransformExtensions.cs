using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions  {

	static List<Transform> childsOfGameobject = new List<Transform>();

	public static List<Transform> GetAllChilds(this Transform transformForSearch)
	{
		List<Transform> getedChilds = new List<Transform>();

		foreach (Transform trans in transformForSearch)
		{
			//Debug.Log (trans.name);
			GetAllChilds ( trans );
			childsOfGameobject.Add ( trans );            
		}        
		return getedChilds;
	}

    public static Transform GetMostParent(this Transform transform)
    {
        Transform aux = transform;
        while(aux.parent!=null){
            aux = aux.parent;
        }
        return aux;
    }

    public static Transform[] GetChildren(this Transform transform)
    {
        Transform[] results = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount;i ++ )
        {
            results[i] = transform.GetChild(i);
        }
        return results;
        
    }

	//does not matter how badly this seems
	public static void DestroyAllChildren(this Transform transform){
		Transform[] children = transform.GetChildren ();
		for(int i=0; i < children.Length ; i++){
			Object.Destroy (children[i].gameObject);
		}
	}


	public static  void ExecuteSelectAnimation(this Transform transform , float multiplier,float transitionTime = 0.3f){

		Vector3 originalScale = transform.localScale;
		Hashtable hash = new Hashtable ();
		hash.Add ("scale", originalScale * multiplier);
		hash.Add ("time",transitionTime);
		iTween.ScaleTo (transform.gameObject, hash);


		Hashtable hash2 = new Hashtable ();
		hash2.Add ("scale",originalScale  );
		hash2.Add ("time",transitionTime);
		hash2.Add ("delay",transitionTime+0.1f);
		iTween.ScaleTo (transform.gameObject, hash2);
	}

}
