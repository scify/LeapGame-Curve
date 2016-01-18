using UnityEngine;
using System.Collections;

public class CurveMovingObject : WorldObject, IUnityRenderable {

    public string prefab;

    public CurveMovingObject(string prefab, Vector3 position, bool hidden) : base(position, hidden) {
        this.prefab = prefab;
	}

    public string getPrefab() {
        return prefab;
    }

}