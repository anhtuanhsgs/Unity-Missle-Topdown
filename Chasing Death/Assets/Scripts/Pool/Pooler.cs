using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Pooler : MonoBehaviour {

    public GameObject pooledObj;
    public int pooledNum;
    public bool growable = true;

    protected LinkedList<GameObject> pooledObjArrList;

	// Use this for initialization
	protected virtual void Start () {
        pooledObjArrList = new LinkedList<GameObject> ();
        for (int i = 0; i < pooledNum; i++) {
            CreateNewPooledObj ();
        }
    }
	
    private void CreateNewPooledObj () {
        GameObject gameObj = Instantiate (pooledObj);
        gameObj.SetActive (false);
        pooledObjArrList.AddFirst (gameObj);
        gameObj.transform.parent = this.transform;

        //Setting up pooled object component
        PooledObject pooledObjCpnt = gameObj.GetComponent<PooledObject> ();
        if (pooledObjCpnt == null) {
            Debug.LogError ("Missing pooledObjCpnt");
        }

        pooledObjCpnt.SetLinkedNode (pooledObjArrList.First);
        pooledObjCpnt.SetPooler (this);
       
    }

	public GameObject GetPooledObj () {
        GameObject gameObj = pooledObjArrList.First.Value;

        //If currently usable
        if (!gameObj.activeInHierarchy) {
            LinkedListNode<GameObject> tmp = pooledObjArrList.First;
            pooledObjArrList.RemoveFirst ();
            pooledObjArrList.AddLast (tmp);
            gameObj.SetActive (true);
            return gameObj;
        }

        //Not available usable object, then create new if GROWABLE
        if (growable) {
            pooledNum++;
            CreateNewPooledObj ();
            return GetPooledObj ();
        }
       
        return null;
    }

    public void RecyleLinkedNode (LinkedListNode<GameObject> node) {
        pooledObjArrList.Remove (node);
        pooledObjArrList.AddFirst (node);
    }
}
