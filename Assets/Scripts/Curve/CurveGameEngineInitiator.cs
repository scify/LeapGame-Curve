using UnityEngine;
using System.Collections.Generic;

public class CurveGameEngineInitiator : MonoBehaviour {

    protected float offset_x = 0.5f;
    protected float offset_y = 0.5f;
    protected float initial_x = -7.5f;
    protected float initial_y = -4f;
    protected float initial_rx = 1f;

	void Start () {
        CurveStateRenderer renderer = new CurveStateRenderer();
        AudioEngine auEngine = new AudioEngine(0, Settings.game_name, Settings.menu_sounds, Settings.game_sounds);

        List<Actor> actors = new List<Actor>();
        actors.Add(new CurveActor("pointer", "Prefabs/Curve/Ball", new Vector3(initial_x, initial_y, 90f), false, null));

        List<WorldObject> environment = new List<WorldObject>();
        environment.Add(new CurveStaticObject("Prefabs/Curve/Ball", Vector3.zero, false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/MainCamera",    Vector3.zero, false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/Light", Vector3.zero, false));
        environment.Add(new CanvasObject("Prefabs/Curve/LeapLogo", true, Vector3.zero, false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/Divider", new Vector3(0, 0, 100f), false));
        TextCanvasObject gui = new TextCanvasObject("Prefabs/Curve/GUI_Element", true, "Time Left: " + Settings.time * 60, new Vector3(0, 0, 0), false);
        environment.Add(gui);

        List<Player> players = new List<Player>();
        players.Add(new Player("player0", "player0"));

        CurveRuleset rules = new CurveRuleset();
        rules.Add(new CurveRule("initialization", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            AudioClip auClip = auEngine.getSoundForMenu("new_game_intro");
            CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", auClip);
            state.environment.Add(tso);
            state.blockingSound = tso;
            state.timestamp = 0;
            state.level = Settings.level;
            state.time_left = Settings.time * 60;
            for (int j = 0; j < 14; j++) {
                switch (state.level) {
                    case 0:
                        state.expectedValues[j] = 6;
                        break;
                    case 1:
                        state.expectedValues[j] = j;
                        break;
                    case 2:
                        state.expectedValues[j] = (int)(13f - j);
                        break;
                    case 3:
                        state.expectedValues[j] = (int)(j * j / 13f);
                        break;
                    case 4:
                        state.expectedValues[j] = (int)(Mathf.Sqrt(14.5f * j));
                        break;
                    case 5:
                        state.expectedValues[j] = (int) Mathf.Round((Mathf.Sin(j * 3.14159f / 4f) + 1f) * 6f);
                        break;
                    case 6:
                        state.expectedValues[j] = (int)(13f / (j + 1));
                        break;
                }
                state.values[j] = -1;
            }
            return false;
        }));

        rules.Add(new CurveRule("frame", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            if (state.timestamp == 2 && !state.repeating && !state.replaying) {
                state.time_left -= Time.deltaTime;
                gui.text = "Time Left: " + (int)state.time_left;
                if (state.time_left <= 0) {
                    state.timestamp = 9;
                    AudioClip auClip = auEngine.getSoundForMenu("new_game_loss");
                    CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", auClip);
                    state.environment.Add(tso);
                    state.blockingSound = tso;
                }
            } else if (state.timestamp == 2) {
                if (state.delay <= 0) {
                    state.delay = 0.6f;
                    if (state.replaying) {
                        float x = initial_rx + state.index * offset_x;
                        float y = initial_y + state.expectedValues[state.index] * offset_y;
                        engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/VisibleGameAudio", auEngine.getSoundForPlayer("note", new Vector3(state.index, state.expectedValues[state.index], 0)), new Vector3(x, y, 90)));
                    } else {
                        if (state.values[state.index] != -1) {
                            float x = initial_x + state.index * offset_x;
                            float y = initial_y + state.values[state.index] * offset_y;
                            engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/VisibleGameAudio", auEngine.getSoundForPlayer("note", new Vector3(state.index, state.values[state.index], 0)), new Vector3(x, y, 90)));
                        } else {
                            engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/VisibleGameAudio", auEngine.getSoundForPlayer("air")));
                        }
                    }
                    state.index++;
                    if (state.index == 14) {
                        state.index = 0;
                        state.repeating = false;
                        state.replaying = false;
                        state.delay = 0;
                    }
                } else {
                    state.delay -= Time.deltaTime;
                }
            }
            return false;
        }));

        rules.Add(new CurveRule("action", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            if (eve.payload.Equals("escape")) {
                Application.LoadLevel(Settings.previousMenu);
                return false;
            }
            return true;
        }));

        rules.Add(new CurveRule("soundOver", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            int id = int.Parse(eve.payload);
            if (state.blockingSound != null && id == state.blockingSound.clip.GetInstanceID()) {
                state.environment.Remove(state.blockingSound);
                state.blockingSound = null;
                if (state.timestamp == 0) {
                    state.timestamp = 2;
                    state.replaying = true;
                }
                if (state.timestamp == 9) {
                    state.timestamp = 10;
                }
            } else {
                WorldObject toRemove = null;
                foreach (WorldObject go in state.environment) {
                    if (go is CurveSoundObject && (go as CurveSoundObject).clip.GetInstanceID() == id) {
                        toRemove = go;
                        break;
                    }
                }
                if (toRemove != null) {
                    state.environment.Remove(toRemove);
                }
            }
            return false;
        }));

        rules.Add(new CurveRule("ALL", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            return !eve.initiator.StartsWith("player") || (eve.initiator.Equals("player" + state.curPlayer) && state.blockingSound == null);
        }));

        rules.Add(new CurveRule("action", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            if (state.repeating || state.replaying || state.blockingSound != null) {
                return true;
            }
            switch (eve.payload) {
                case "any":
                    if (state.timestamp == 10) {
                        (state.result as CurveGameResult).status = CurveGameResult.GameStatus.Over;
                        return false;
                    }
                    return true;
                case "replay":
                    state.replaying = true;
                    state.index = 0;
                    return false;
                case "repeat":
                    state.repeating = true;
                    state.index = 0;
                    return false;
                case "enter":
                    if (state.timestamp == 1 || state.timestamp >= 9) {
                        return false;
                    }
                    foreach (Actor actor in state.actors) {
                        Vector3 position = actor.position - new Vector3(initial_x, initial_y, 0f);
                        int x = (int)(position.x / offset_x);
                        int y = (int)(position.y / offset_y);
                        if (state.values[x] == state.expectedValues[x]) {
                            engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("boundary", new Vector3(0, 1, 0)), Vector3.zero));
                        } else {
                            state.values[x] = y;
                            if (state.values[x] == state.expectedValues[x]) {
                                engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("correct"), Vector3.zero));
                            } else {
                                engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(x, y, 0)), Vector3.zero));
                            }
                        }
                    }
                    int i;
                    for (i = 0; i < 14; i++) {
                        if (state.values[i] != state.expectedValues[i]) {
                            break;
                        }
                    }
                    if (i == 14) {
                        state.timestamp = 9;
                        engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("claps_1"), Vector3.zero));
                        AudioClip auClip = auEngine.getSoundForMenu("new_game_win");
                        CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", auClip);
                        state.environment.Add(tso);
                        state.blockingSound = tso;
                    }
                    return false;
            }
            return true;
        }));

        rules.Add(new CurveRule("move", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            if (state.repeating || state.replaying) {
                return false;
            }
            int dx = 0;
            int dy = 0;
            switch (eve.payload) {
                case "left":
                    dx = -1;
                    break;
                case "right":
                    dx = 1;
                    break;
                case "up":
                    dy = 1;
                    break;
                case "down":
                    dy = -1;
                    break;
                default:
                    return false;
            }
            foreach (Actor actor in state.actors) {
                Vector3 position = actor.position - new Vector3(initial_x, initial_y, 0f);
                int x = (int)(position.x / offset_x) + dx;
                int y = (int)(position.y / offset_y) + dy;
                if (x <= -1 || x >= 14 || y <= -1 || y >= 14) {
                    return false;
                }
                actor.position = new Vector3(offset_x * x + initial_x, offset_y * y + initial_y, actor.position.z);
                engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(x, y, 0))));
            }
            return false;
        }));

        gameObject.AddComponent<CurveGameEngine>();
        gameObject.AddComponent<CurveUserInterface>();
        gameObject.GetComponent<CurveGameEngine>().initialize(rules, actors, environment, players, renderer);
        gameObject.GetComponent<CurveUserInterface>().initialize(gameObject.GetComponent<CurveGameEngine>());
        gameObject.GetComponent<CurveGameEngine>().postEvent(new GameEvent("", "initialization", "unity"));
	}
}