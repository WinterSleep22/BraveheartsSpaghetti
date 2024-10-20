using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{

    public bool allowDrop = true;

    private RectTransform _myRectTransform;

    public RectTransform myRectTransform
    {
        get
        {
            if(_myRectTransform==null){
                _myRectTransform = GetComponent<RectTransform>();
            }
            return _myRectTransform;
        }
    }


    public Transform GetMyLastChild
    {
        get
        {
            return transform.GetChild(transform.childCount-1);
        }
    }

    public OnDropDraggable onEnterDropCallback;

    public delegate void OnDropDraggable(Draggable draggable);

    public OnExitDropZoneDraggable onExitDropCallback;

    public delegate void OnExitDropZoneDraggable(Draggable draggable);

    public OnCardComeBackDrag onCardComeBackDragCallback;
    public delegate void OnCardComeBackDrag(Draggable draggable);

    public OnAllowCardEnter onAllowCardEnter;
    public delegate bool OnAllowCardEnter(Draggable draggable);

    public OnCardBeginComeBack onCardBeginComeBack;
    public delegate void OnCardBeginComeBack(Draggable draggable);

    public delegate void OnDropped(Draggable draggable);
    public OnDropped onBeingDropped;

    public delegate bool OnAllowDragToDrop(Draggable draggable);
    public OnAllowDragToDrop onAllowDragToDrop;


    public delegate Vector3 GetDragPosition(Draggable drag);
    public GetDragPosition getDragPosition;

    public delegate Vector3 GetDragRotation(Draggable drag);
    public GetDragRotation getDragRotation;

    public List<Draggable> draggables;

    public int max=41;

    /// <summary>
    /// used for comebackToDropzone, this is a localPosirion
    /// </summary>
    /// <param name="drag"></param>
    /// <returns></returns>
    public Vector3 GetMyPosition(Draggable drag)
    {
        if (getDragPosition != null)
        {
            return getDragPosition(drag);
        }
        else
        {
            return Vector3.zero;
        }
    }


    public Vector3 GetMyRotation(Draggable drag)
    {
        if (getDragRotation != null)
        {
            return getDragRotation(drag);
        }
        else
        {
            return Vector3.zero;
        }
    }
    
    public void DetectDragChilds()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Draggable drag = transform.GetChild(i).GetComponent<Draggable>();
            if(drag!=null && !draggables.Contains(drag))
            {
                AddDraggable(drag);
            }
        }
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        if (!allowDrop)
        {
            Debug.Log("Drop not allowed");
            return;
        }
        Draggable draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if(draggable == null){
            Debug.Log("draggable is null OnDrop");
            return;
        }
        if (onBeingDropped!=null)
        {
            onBeingDropped(draggable);
        }
        if( draggable.allowDrag==false)
        {
            Debug.Log("cant drop this drag, his allow drag property is set to false");
            return;
        }
        if(draggable.allowDrop==false){
            Debug.Log("cant drop this drag, his allow drop property is set to false");
            return;
        }
        if (onAllowDragToDrop!=null)
        {
            if (!onAllowDragToDrop(draggable))
            {
                Debug.Log("Dropzone.OnDrop onAllowDragToDrop returned false draaglable=" + draggable.gameObject.name);
                return;
            }
        }

        if ( !draggables.Contains(draggable) )
        {
            Debug.Log("Drop: adding Draggable");
            AddDraggable(draggable);
        }
        if (draggables.Contains(draggable))
        {
            Debug.Log("Drop:just moving back");
            draggable.ComeBackToDropZone(2f);
        }
       

    }

    public void OnCardBeginComeBackDropZone(Draggable drag)
    {
        if (onCardBeginComeBack != null)
            onCardBeginComeBack(drag);
    }
    public void OnCardComeBackDropZone(Draggable drag)
    {
        drag.transform.SetParent(transform);
        if (onCardComeBackDragCallback != null)
            onCardComeBackDragCallback(drag);
    }
    
    public void AddDraggable(Draggable drag)
    {
        if(draggables.Count >= max){
           Debug.Log("Dropzone=" + gameObject.name + " got max");
            return;
        }
        if(onAllowCardEnter!=null)
        if(!onAllowCardEnter(drag)){
            Debug.Log("Dropzone=" + gameObject.name + " card="+drag.gameObject.name+" not allowed");
            return;
        }
     
        //set drag
        drag.SetDropZone(this);
        drag.onEndBackToDropZone += OnCardComeBackDropZone;
        drag.onChangeDropZone += RemoveDraggable;
        drag.onBeginBackToDropZone += OnCardBeginComeBackDropZone;
        
        drag.transform.SetParent(transform);
        
       // drag.myRectTransform.localPosition = Vector3.zero;
      
       
        if (onEnterDropCallback != null)
        {
            onEnterDropCallback(drag);
        }
        if (drag != null && !draggables.Contains(drag))
            draggables.Add(drag);
        
    }

    public void RemoveDraggable(Draggable drag)
    {
  
        drag.onEndBackToDropZone -= OnCardComeBackDropZone;
        drag.onChangeDropZone -= RemoveDraggable;
        drag.onBeginBackToDropZone -= OnCardBeginComeBackDropZone;

        drag.transform.SetParent(drag.myCanvas.transform);
        if (onExitDropCallback != null)
            onExitDropCallback(drag);
        if (drag != null && draggables.Contains(drag))
            draggables.Remove(drag);
    }



}
