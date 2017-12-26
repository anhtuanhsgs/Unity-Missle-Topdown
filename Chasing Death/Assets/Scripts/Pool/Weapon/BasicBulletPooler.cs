using UnityEngine;
using System.Collections;

public class BasicBulletPooler : Pooler {

    public static BasicBulletPooler instance;

	void Awake () {
        instance = this;
    }
}
