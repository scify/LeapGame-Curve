using UnityEngine;
using System.Collections.Generic;
using System;

public class CurveSelectionMenuInitiator : MonoBehaviour {

    public float offset_y;
    private CurveStaticObject movingCamera = new CurveStaticObject("Prefabs/Curve/Camera_Default", new Vector3(0, 10, 0), false);

    void Start() {
        CurveStateRenderer renderer = new CurveStateRenderer();
        AudioEngine auEngine = new AudioEngine(0, Settings.game_name, Settings.menu_sounds, Settings.game_sounds);

        List<WorldObject> environment = new List<WorldObject>();
        environment.Add(movingCamera);
        environment.Add(new CurveStaticObject("Prefabs/Curve/Light_Default", new Vector3(0, 10, 0), false));
        environment.Add(new CanvasObject("Prefabs/Curve/Logos", true, new Vector3(10000, 0, 0), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonSelected", "y = 6", "timeSelectionMenu", "0", auEngine.getSoundForMenu("f0"), new Vector3(0, 0, -offset_y), false, true));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "y = x", "timeSelectionMenu", "1", auEngine.getSoundForMenu("f1"), new Vector3(0, 0, 0), false, false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "y = -x", "timeSelectionMenu", "2", auEngine.getSoundForMenu("f2"), new Vector3(0, 0, offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "y = x^2", "timeSelectionMenu", "3", auEngine.getSoundForMenu("f3"), new Vector3(0, 0, 2 * offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "y = sqrt(x)", "timeSelectionMenu", "4", auEngine.getSoundForMenu("f4"), new Vector3(0, 0, 3 * offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "y = ημ(x)", "timeSelectionMenu", "5", auEngine.getSoundForMenu("f5"), new Vector3(0, 0, 4 * offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "y = 1/x", "timeSelectionMenu", "6", auEngine.getSoundForMenu("f6"), new Vector3(0, 0, 5 * offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "Πίσω", "mainMenu", "back", auEngine.getSoundForMenu("back"), new Vector3(0, 0, 6 * offset_y), false));

        CurveRuleset rules = new CurveRuleset();
        rules.Add(new CurveRule("initialization", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            AudioClip audioClip;
            state.timestamp = 0;
            audioClip = auEngine.getSoundForMenu("selection_intro");
            CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
            state.environment.Add(tso);
            state.stoppableSounds.Add(tso);
            Settings.previousMenu = "curveSelectionMenu";
            return false;
        }));

        rules.Add(new CurveRule("soundOver", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (state.timestamp == 0) {
                AudioClip audioClip = auEngine.getSoundForMenu("f0");
                CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
                state.environment.Add(tso);
                state.stoppableSounds.Add(tso);
                state.timestamp = 1;
            }
            return false;
        }));

        rules.Add(new CurveRule("action", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (eve.payload.Equals("escape")) {
                Application.LoadLevel("mainMenu");
                return false;
            }
            return true;
        }));

        rules.Add(new CurveRule("action", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (eve.payload.Equals("enter")) {
                foreach (WorldObject obj in state.environment) {
                    if (obj is CurveMenuItem) {
                        if ((obj as CurveMenuItem).selected) {
                            int lvl;
                            if (Int32.TryParse((obj as CurveMenuItem).audioMessageCode, out lvl)) {
                                Settings.level = lvl;
                            }
                            Application.LoadLevel((obj as CurveMenuItem).target);
                            return false;
                        }
                    }
                }
            }
            return true;
        }));

        rules.Add(new CurveRule("move", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            state.timestamp++;
            foreach (CurveSoundObject Curveso in state.stoppableSounds) {
                state.environment.Remove(Curveso);
            }
            state.stoppableSounds.Clear();
            CurveMenuItem previous = null;
            bool change = false;
            AudioClip audioClip;
            CurveSoundObject tso;
            foreach (WorldObject obj in state.environment) {
                if (obj is CurveMenuItem) {
                    CurveMenuItem temp = obj as CurveMenuItem;
                    if (temp.selected) {
                        if (eve.payload == "_up" || eve.payload == "left") {
                            if (previous == null) {
                                audioClip = auEngine.getSoundForPlayer("boundary", Vector3.up);
                                tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
                                state.environment.Add(tso);
                                state.stoppableSounds.Add(tso);
                                break;
                            }
                            temp.selected = false;
                            temp.prefab = temp.prefab.Replace("Selected", "Default");
                            previous.selected = true;
                            previous.prefab = previous.prefab.Replace("Default", "Selected");
                            tso = new CurveSoundObject("Prefabs/Curve/AudioSource", previous.audioMessage, Vector3.zero);
                            state.environment.Add(tso);
                            state.stoppableSounds.Add(tso);
                            break;
                        } else {
                            change = true;
                        }
                    } else if (change) {
                        temp.selected = true;
                        temp.prefab = temp.prefab.Replace("Default", "Selected");
                        previous.prefab = previous.prefab.Replace("Selected", "Default");
                        previous.selected = false;
                        change = false;
                        tso = new CurveSoundObject("Prefabs/Curve/AudioSource", temp.audioMessage, Vector3.zero);
                        state.environment.Add(tso);
                        state.stoppableSounds.Add(tso);
                        break;
                    }
                    previous = temp;
                }
            }
            if (change) {
                audioClip = auEngine.getSoundForPlayer("boundary", Vector3.down);
                tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
                state.environment.Add(tso);
                state.stoppableSounds.Add(tso);
            }
            foreach (WorldObject obj in state.environment) {
                if (obj is CurveMenuItem) {
                    CurveMenuItem temp = obj as CurveMenuItem;
                    if (temp.selected) {
                        movingCamera.position = new Vector3(0, 10, Mathf.Clamp(temp.position.z, 6 * offset_y, 0));
                        break;
                    }
                }
            }
            return true;
        }));

        gameObject.AddComponent<CurveMenuEngine>();
        gameObject.AddComponent<CurveMenuUserInterface>();
        gameObject.GetComponent<CurveMenuEngine>().initialize(rules, environment, renderer);
        gameObject.GetComponent<CurveMenuUserInterface>().initialize(gameObject.GetComponent<CurveMenuEngine>());
        gameObject.GetComponent<CurveMenuEngine>().postEvent(new GameEvent("", "initialization", "unity"));
    }
}
