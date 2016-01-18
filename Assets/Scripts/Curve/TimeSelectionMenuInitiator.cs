using UnityEngine;
using System.Collections.Generic;
using System;

public class TimeSelectionMenuInitiator : MonoBehaviour {

    public float offset_y;
    private CurveStaticObject movingCamera = new CurveStaticObject("Prefabs/Curve/Camera_Default", new Vector3(0, 10, 0), false);

    void Start() {
        CurveStateRenderer renderer = new CurveStateRenderer();
        AudioEngine auEngine = new AudioEngine(0, Settings.game_name, Settings.menu_sounds, Settings.game_sounds);

        List<WorldObject> environment = new List<WorldObject>();
        environment.Add(movingCamera);
        environment.Add(new CurveStaticObject("Prefabs/Curve/Light_Default", new Vector3(0, 10, 0), false));
        environment.Add(new CanvasObject("Prefabs/Curve/Logos", true, new Vector3(10000, 0, 0), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonSelected", "5 mins", "newGame", "5", auEngine.getSoundForMenu("new_game_remaining_time_5"), new Vector3(0, 0, -offset_y), false, true));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "4 mins", "newGame", "4", auEngine.getSoundForMenu("new_game_remaining_time_4"), new Vector3(0, 0, 0), false, false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "3 mins", "newGame", "3", auEngine.getSoundForMenu("new_game_remaining_time_3"), new Vector3(0, 0, offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "2 mins", "newGame", "2", auEngine.getSoundForMenu("new_game_remaining_time_2"), new Vector3(0, 0, 2 * offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "1 mins", "newGame", "1", auEngine.getSoundForMenu("new_game_remaining_time_1"), new Vector3(0, 0, 3 * offset_y), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "Back", "curveSelectionMenu", "back", auEngine.getSoundForMenu("back"), new Vector3(0, 0, 4 * offset_y), false));

        CurveRuleset rules = new CurveRuleset();
        rules.Add(new CurveRule("initialization", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            AudioClip audioClip;
            state.timestamp = 0;
            audioClip = auEngine.getSoundForMenu("new_game_select");
            CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
            state.environment.Add(tso);
            state.stoppableSounds.Add(tso);
            Settings.previousMenu = "curveSelectionMenu";
            return false;
        }));

        rules.Add(new CurveRule("soundOver", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (state.timestamp == 0) {
                AudioClip audioClip = auEngine.getSoundForMenu("new_game_remaining_time_5");
                CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
                state.environment.Add(tso);
                state.stoppableSounds.Add(tso);
                state.timestamp = 1;
            }
            return false;
        }));

        rules.Add(new CurveRule("action", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (eve.payload.Equals("escape")) {
                Application.LoadLevel("curveSelectionMenu");
                return false;
            }
            return true;
        }));

        rules.Add(new CurveRule("action", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (eve.payload.Equals("enter")) {
                foreach (WorldObject obj in state.environment) {
                    if (obj is CurveMenuItem) {
                        if ((obj as CurveMenuItem).selected) {
                            int mins;
                            if (Int32.TryParse((obj as CurveMenuItem).audioMessageCode, out mins)) {
                                Settings.time = mins;
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
