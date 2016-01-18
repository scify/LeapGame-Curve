using UnityEngine;
using System.Collections;

public class CurveMenuUserInterface : MonoBehaviour {

    public CurveMenuEngine engine;

    private bool initialized;

    public void initialize(CurveMenuEngine engine) {
        this.engine = engine;
        initialized = true;
    }

	public void Update() {
        if (!initialized) {
            return;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            engine.postEvent(new UIEvent("_up", "move", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            engine.postEvent(new UIEvent("down", "move", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            engine.postEvent(new UIEvent("left", "move", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            engine.postEvent(new UIEvent("right", "move", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            engine.postEvent(new UIEvent("select", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Return)) {
            engine.postEvent(new UIEvent("enter", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
            engine.postEvent(new UIEvent("enter", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            engine.postEvent(new UIEvent("escape", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            engine.postEvent(new UIEvent("default", "soundSettings", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            engine.postEvent(new UIEvent("electra", "soundSettings", "player0"));
        }

	}
	
}