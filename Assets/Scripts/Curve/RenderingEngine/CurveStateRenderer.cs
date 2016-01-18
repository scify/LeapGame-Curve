using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class CurveStateRenderer : StateRenderer {

	public CurveStateRenderer() {
	}

    public override void render(GameEngine engine) {
        throw new System.NotImplementedException("This engine can not be used for this game");
    }

    public void render(CurveMenuEngine engine) {
        CurveMenuState state = engine.state;
        List<WorldObject> currentRenderedObjects = new List<WorldObject>();
        render(state.environment, currentRenderedObjects, engine);
        List<WorldObject> toRemove = new List<WorldObject>();
        foreach (WorldObject so in rendered.Keys) {
            if (!currentRenderedObjects.Contains(so)) {
                UnityEngine.Object.Destroy(rendered[so]);
                toRemove.Add(so);
            }
        }
        foreach (WorldObject so in toRemove) {
            rendered.Remove(so);
            prefabs.Remove(so);
        }
    }

    public void render(CurveGameEngine engine) {
        CurveGameState state = engine.state;
        List<WorldObject> currentRenderedObjects = new List<WorldObject>();
        render(state.environment, currentRenderedObjects, engine);
        render(state.actors, currentRenderedObjects, engine);
        List<WorldObject> toRemove = new List<WorldObject>();
        foreach (WorldObject so in rendered.Keys) {
            if (!currentRenderedObjects.Contains(so)) {
                UnityEngine.Object.Destroy(rendered[so]);
                toRemove.Add(so);
            }
        }
        foreach (WorldObject so in toRemove) {
            rendered.Remove(so);
        }
    }

    private void render<T>(List<T> set, List<WorldObject> currentRenderedObjects, GameEngine engine) where T : WorldObject {
        foreach (T so in set) {
            if (so is IUnityRenderable) {
                if (!rendered.ContainsKey(so)) {
                    GameObject go = (GameObject) GameObject.Instantiate(Resources.Load((so as IUnityRenderable).getPrefab()));
                    go.transform.position = so.position;
                    rendered.Add(so, go);
                    prefabs.Add(so, (so as IUnityRenderable).getPrefab());
                    if (so is CanvasObject && (so as CanvasObject).coversCamera) {
                        go.GetComponent<Canvas>().worldCamera = Camera.main;
                    }
                    if (so is CurveMenuItem) {
                        go.GetComponentInChildren<TextMesh>().text = (so as CurveMenuItem).message;
                    }
                    if (so is CurveSoundObject) {
                        go.GetComponent<AudioSource>().clip = (so as CurveSoundObject).clip;
                        go.GetComponent<AudioSource>().Play();
                        go.GetComponent<SoundScript>().initialize(engine);
                    }
                    if (so is CurveMovingObject) {
                        go.GetComponent<Movement>().engine = engine;
                    }
                } else {
					if (so.hidden && so is SoundObject) {
						rendered[so].GetComponent<AudioSource>().Stop();
					}
                    if (!prefabs[so].Equals((so as IUnityRenderable).getPrefab())) {
                        UnityEngine.Object.Destroy(rendered[so]);
                        rendered.Remove(so);
                        prefabs.Remove(so);
                        GameObject go = (GameObject) GameObject.Instantiate(Resources.Load((so as IUnityRenderable).getPrefab()));
                        rendered.Add(so, go);
                        prefabs.Add(so, (so as IUnityRenderable).getPrefab());
                    }
                    if (so is CurveMenuItem) {
                        rendered[so].GetComponentInChildren<TextMesh>().text = (so as CurveMenuItem).message;
                    }
                    if (so is TextCanvasObject) {
                        rendered[so].GetComponentInChildren<Text>().text = (so as TextCanvasObject).text;
                    }
                    if (so is CurveMovingObject) {
                        so.position = rendered[so].transform.position;
                    } else {
                        rendered[so].transform.position = so.position;
                    }
                }
                currentRenderedObjects.Add(so);
            }
        }
    }
}