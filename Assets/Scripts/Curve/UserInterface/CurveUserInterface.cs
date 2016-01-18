using UnityEngine;
using System.Collections;

public class CurveUserInterface : MonoBehaviour {

    public CurveGameEngine engine;

    private bool initialized;

    public void initialize(CurveGameEngine engine) {
        this.engine = engine;
        initialized = true;
    }

	public void Update() {
        if (!initialized) {
            return;
        }
        engine.postEvent(new UIEvent("frame", "frame", ""));
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            engine.postEvent(new UIEvent("up", "move", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            engine.postEvent(new UIEvent("left", "move", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            engine.postEvent(new UIEvent("right", "move", "player0"));
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			engine.postEvent(new UIEvent("down", "move", "player0"));
		}
        if (Input.GetKeyDown(KeyCode.Escape)) {
            engine.postEvent(new UIEvent("escape", "action", "player0"));
        }
		if (Input.GetKeyDown(KeyCode.Return)) {
			engine.postEvent(new UIEvent("enter", "action", "player0"));
		}
		if (Input.GetKeyDown(KeyCode.KeypadEnter)) {
			engine.postEvent(new UIEvent("enter", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            engine.postEvent(new UIEvent("replay", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Insert)) {
            engine.postEvent(new UIEvent("repeat", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            engine.postEvent(new UIEvent("repeat", "action", "player0"));
        }
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            engine.postEvent(new UIEvent("repeat", "action", "player0"));
        }
		if (Input.anyKeyDown) {
			engine.postEvent(new UIEvent("any", "action", "player0"));
		}
	}
	
}