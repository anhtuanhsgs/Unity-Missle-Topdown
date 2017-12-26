using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PooledObject : MonoBehaviour {

    private Pooler pooler;
    private LinkedListNode<GameObject> linkedNode;

    public void SetLinkedNode (LinkedListNode<GameObject> newNode) {
        linkedNode = newNode;
    }

    public void SetPooler (Pooler newPooler) {
        pooler = newPooler;
    }

    protected virtual void update () {
        //TODO: Removable when finish project
        if (linkedNode == null) {
            Debug.LogError ("Mising linkedNode");
        }

        if (pooler == null) {
            Debug.LogError ("Missing pooler");
        }
    }
    protected virtual void Destroy () {
        gameObject.SetActive (false);
        pooler.RecyleLinkedNode (linkedNode);
    }

    protected virtual void OnEnable () {
    }

    protected virtual void OnDisable () {
        
    }
}
