using UnityEngine;
using System.Collections;

public class CurveStaticObject : StaticObject, IUnityRenderable {

    public string prefab;

    public CurveStaticObject(string prefab, Vector3 position, bool hidden) : base(position, hidden) {
        this.prefab = prefab;
	}

    public string getPrefab() {
        return prefab;
    }

}