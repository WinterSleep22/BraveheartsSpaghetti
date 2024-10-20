using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public static class MonobehaviourExtensions
{



    public static void ChangeWidth(this MonoBehaviour mono, RectTransform target, float newWidth, float lerp)
    {
        mono.ExecuteEach(step: 0f,
            OnUpdate: () =>
            {
                float lerpedWidth = Mathf.Lerp(target.GetWidth(), newWidth, Time.deltaTime * lerp);
                target.SetWidth(lerpedWidth);
               // Debug.Log("updating width");
            },
            ExitCondition: () =>
            {

                bool exit = Mathf.Abs(target.GetWidth() - newWidth) < 1f;
                if (exit)
                {
                  //  Debug.Log("ExitCondition");
                }
                return exit;
            }
        );
    }

    /// <summary>
    /// this is a version of GetComponentInChildren not including the parent itself
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mono"></param>
    public static T GetComponentInChildrenOnly<T>(this MonoBehaviour mono) where T : Component
    {
        var childrenList = mono.GetComponentsInChildren<T>(true).Where(x => x.gameObject.transform.parent != mono.transform.parent).ToArray();
        if(childrenList.Length > 0){
            return childrenList[0];
        }
        else
        {
            return null;
        }

    }

    /// <summary>
    /// this is a version of GetComponentsInChildren not including the parent itself
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mono"></param>
    public static T[] GetComponentsInChildrenOnly<T>(this MonoBehaviour mono) where T : Component
    {
        var childrenList = mono.GetComponentsInChildren<T>(true).Where(x => x.gameObject.transform.parent != mono.transform.parent).ToArray();
        return childrenList;
    }

    public static void SetActiveChildsIn(this MonoBehaviour monoBheaviour, Transform Parent, float deltaTime=0.5f,bool on=true)
    {
        monoBheaviour.StartCoroutine(SetActivatingGroup(Parent.GetChildren(), deltaTime,on));
    }

    static IEnumerator SetActivatingGroup(Transform[] group,float deltaTime = 0.5f ,bool on=true)
    {
        for (int i = 0; i < group.Length; i++)
        {
            group[i].gameObject.SetActive(on);
            yield return new WaitForSeconds(deltaTime);
        }
    }


  


    public static void ExecuteEach(
        this MonoBehaviour monoBheaviour,
        float step =0,
        System.Action OnUpdate=null,
        System.Action OnEnter= null,
        System.Action OnExit = null,
        System.Func<bool> ExitCondition = null,
        System.Action OnExitCondition = null,
        System.Action OnExitMaxTime = null,
        float maxTime = -1
    ){
        monoBheaviour.StartCoroutine(ExecutingEach(step,
            OnEnter,
            OnUpdate,
            ExitCondition,
            OnExitCondition,
            OnExitMaxTime,
            OnExit,
            maxTime
            ));
    }

    static IEnumerator ExecutingEach(float step,
        System.Action OnEnter,
        System.Action OnUpdate,
        System.Func<bool> ExitCondition,
        System.Action OnExitCondition,
        System.Action OnExitMaxTime,
        System.Action OnExit, 
        float maxTime)
    {
        if (OnEnter != null)
            OnEnter();

        float steps = 0;
        if(maxTime==-1){
            maxTime = 100;
        }
       
        for (; ; )
        {
            if (ExitCondition != null)
                if (ExitCondition())
                {
                    if (OnExitCondition != null)
                        OnExitCondition();
                    break;
                }

            if (OnUpdate != null)
                OnUpdate();
            if (steps > maxTime / ((step == 0) ? Time.deltaTime: step) )
            {
                if (OnExitMaxTime != null)
                    OnExitMaxTime();
                break;
            }
            steps++;
            if (step == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                yield return new WaitForSeconds(step);
            }
        }
        if (OnExit != null)
            OnExit();
    }



 

    public static void ExecuteIn(this MonoBehaviour me, float time, System.Action action)
    {
        if (!me.gameObject.activeSelf || action==null)
        {
            return;
        }
        me.StartCoroutine(ExecutingIn(time, me.gameObject, action));
    }

    public static IEnumerator ExecutingIn(float time, GameObject go , System.Action action)
    {
        yield return new WaitForSeconds(time);
        action();
    }

  


    public static void SetActiveIn(this MonoBehaviour me, float time, bool on,GameObject go)
    {
        me.ExecuteIn(time, () => { go.SetActive(on); });
    }

    
    public static T GetOrAddComponent<T>(this MonoBehaviour me ) where T : Component 
    {
        T component = me.GetComponent<T>();
        if (component == null)
        {
            component = me.gameObject.AddComponent<T>();
        }
        return component;
    }


    #region Search Children Components Helpers

    public static T[] GetComponentsInChildrenBFS<T>(this Transform me, bool includeInactive = false, bool includeSelf=true ) where T: Component
    {
         Queue<Transform> toScan = new Queue<Transform>();
        List<T> result = new List<T>();
        Transform current = me.transform;

        if (includeSelf)
        {
            result.AddRange(current.GetComponents<T>());
        }
        do
        {
            foreach (Transform t in current)
            {
                if (t.gameObject.activeSelf || includeInactive)
                {
                    toScan.Enqueue(t);
                }
            }
            if (toScan.Count == 0)
            {
                break;
            }
            current = toScan.Dequeue();
            result.AddRange(current.GetComponents<T>());
        } while (toScan.Count > 0);

        return result.ToArray();
    }

    public static T[] GetComponentsInChildrenBFS<T>(this Component c, bool includeInactive = false, bool includeSelf=true) where T : Component
    {
        Queue<Transform> toScan = new Queue<Transform>();
        List<T> result = new List<T>();
        Transform current = c.transform;

        if (includeSelf)
        {
            result.AddRange(current.GetComponents<T>());
        }
        do
        {
            foreach (Transform t in current)
            {
                if (t.gameObject.activeSelf || includeInactive)
                {
                    toScan.Enqueue(t);
                }
            }
            if (toScan.Count == 0)
            {
                break;
            }
            current = toScan.Dequeue();
            result.AddRange(current.GetComponents<T>());
        } while (toScan.Count > 0);

        return result.ToArray();
    }

    #endregion

}
