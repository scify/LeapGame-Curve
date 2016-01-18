using UnityEngine;
using System.Collections.Generic;

public class CurveMainMenuInitiator : MonoBehaviour {

    public float offset_x;
    public float offset_y;

    void Start() {
        CurveStateRenderer renderer = new CurveStateRenderer();
        AudioEngine auEngine = new AudioEngine(0, Settings.game_name, Settings.menu_sounds, Settings.game_sounds);

        List<WorldObject> environment = new List<WorldObject>();
        environment.Add(new CurveStaticObject("Prefabs/Curve/Camera_Default", new Vector3(0, 10, 0), false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/Light_Default", new Vector3(0, 10, 0), false));
        environment.Add(new CanvasObject("Prefabs/Curve/Logos", true, new Vector3(10000, 0, 0), false));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonSelected", "Οδηγίες", "tutorial", "tutorials", auEngine.getSoundForMenu("tutorials"), new Vector3(0, 0, -offset_y), false, true));
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "Νέο Παιχνίδι", "curveSelectionMenu", "new_game", auEngine.getSoundForMenu("new_game"), new Vector3(0, 0, 0), false));        
        environment.Add(new CurveMenuItem("Prefabs/Curve/ButtonDefault", "Έξοδος", "exitScene", "exit", auEngine.getSoundForMenu("exit"), new Vector3(0, 0, offset_y), false));

        CurveRuleset rules = new CurveRuleset();
        rules.Add(new CurveRule("initialization", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            AudioClip audioClip;
            if (Settings.just_started) {
                Settings.just_started = false;
                audioClip = auEngine.getSoundForMenu("game_intro");
                state.timestamp = 0;
            } else {
                audioClip = auEngine.getSoundForMenu("tutorials");
                state.timestamp = 1;
            }
            CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", audioClip, Vector3.zero);
            state.environment.Add(tso);
            state.stoppableSounds.Add(tso);
            Settings.previousMenu = "mainMenu";
            return false;
        }));

        rules.Add(new CurveRule("soundSettings", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            Settings.menu_sounds = eve.payload;
            auEngine = new AudioEngine(0, "curve", Settings.menu_sounds, Settings.game_sounds);
            foreach (WorldObject wo in state.environment) {
                if (wo is CurveMenuItem) {
                    (wo as CurveMenuItem).audioMessage = auEngine.getSoundForMenu((wo as CurveMenuItem).audioMessageCode);
                }
            }
            return false;
        }));

        rules.Add(new CurveRule("soundOver", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            if (state.timestamp == 0) {
                AudioClip audioClip = auEngine.getSoundForMenu("tutorials");
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
            return true;
        }));

        gameObject.AddComponent<CurveMenuEngine>();
        gameObject.AddComponent<CurveMenuUserInterface>();
        gameObject.GetComponent<CurveMenuEngine>().initialize(rules, environment, renderer);
        gameObject.GetComponent<CurveMenuUserInterface>().initialize(gameObject.GetComponent<CurveMenuEngine>());
        gameObject.GetComponent<CurveMenuEngine>().postEvent(new GameEvent("", "initialization", "unity"));
    }
}
