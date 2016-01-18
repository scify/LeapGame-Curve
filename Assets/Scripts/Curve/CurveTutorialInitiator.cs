using UnityEngine;
using System.Collections.Generic;

public class CurveTutorialInitiator : MonoBehaviour {

    protected float offset_x = 0.5f;
    protected float offset_y = 0.5f;
    protected float initial_x = -7.5f;
    protected float initial_y = -4f;
    protected float initial_rx = 1f;
    protected int max = 1;
	
	void Start () {
		CurveStateRenderer renderer = new CurveStateRenderer();
        AudioEngine auEngine = new AudioEngine(0, Settings.game_name, Settings.menu_sounds, Settings.game_sounds);

        List<Actor> actors = new List<Actor>();
        actors.Add(new CurveActor("pointer", "Prefabs/Curve/Ball", new Vector3(initial_x, initial_y, 90f), false, null));

        List<WorldObject> environment = new List<WorldObject>();
        environment.Add(new CurveStaticObject("Prefabs/Curve/Ball", Vector3.zero, false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/MainCamera", Vector3.zero, false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/Light", Vector3.zero, false));
        environment.Add(new CanvasObject("Prefabs/Curve/LeapLogo", true, Vector3.zero, false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/Divider", new Vector3(0, 0, 100f), false));
		
		List<Player> players = new List<Player>();
		players.Add(new Player("player0", "player0"));

        CurveRuleset rules = new CurveRuleset();
		rules.Add(new CurveRule("initialization", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
			AudioClip auClip = auEngine.getSoundForMenu("tutorial_1");
			CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", auClip);
			state.environment.Add(tso);
			state.blockingSound = tso;
            state.timestamp = 11;
            for (int j = 0; j < 14; j++) {
                state.expectedValues[j] = 6;
            }
			return false;
		}));

        rules.Add(new CurveRule("frame", (CurveGameState state, GameEvent eve, CurveGameEngine engine) => {
            if (state.replaying || state.repeating) {
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
                    if (state.index == max) {
                        if ((state.timestamp == 17 && state.replaying)
                            || (state.timestamp == 19 && state.replaying)
                            || (state.timestamp == 20 && state.repeating)
                            || (state.timestamp == 22 && state.replaying)
                            || (state.timestamp == 23 && state.replaying)) {
                            state.timestamp += 41;
                            if (state.timestamp == 60) {
                                state.values[0] = 12;
                            }
                        }
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
                CurveSoundObject tso;
                switch (state.timestamp) {
                    case 11:
                        tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(7, 0, 0)));
                        state.blockingSound = tso;
                        state.environment.Add(tso);
                        state.timestamp = 52;
                        return false;
                    case 12:
                        tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(7, 1, 0)));
                        state.blockingSound = tso;
                        state.environment.Add(tso);
                        state.timestamp = 53;
                        return false;
                    case 13:
                        tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(7, 13, 0)));
                        state.blockingSound = tso;
                        state.environment.Add(tso);
                        state.timestamp = 54;
                        return false;
                    case 14:
                        state.replaying = true;
                        state.timestamp = 55;
                        return false;
                    case 115:
                        tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForMenu("tutorial_5b"));
                        state.blockingSound = tso;
                        state.environment.Add(tso);
                        state.timestamp = 15;
                        return false;
                    case 116:
                        tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForMenu("tutorial_6"));
                        state.blockingSound = tso;
                        state.environment.Add(tso);
                        state.timestamp = 57;
                        max = 3;
                        return false;
                    case 118:
                        tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForMenu("tutorial_8b"));
                        state.blockingSound = tso;
                        state.environment.Add(tso);
                        state.timestamp = 18;
                        return false;
                    case 22:
                        max = 14;
                        for (int i = 0; i < 14; i++) {
                            state.expectedValues[i] = i / 3;
                        }
                        state.replaying = true;
                        return false;
                    case 23:
                        state.replaying = true;
                        for (int i = 0; i < 14; i++) {
                            state.expectedValues[i] = 13 - i;
                        }
                        return false;
                    case 24:
                        state.timestamp = 10;
                        return false;
                    default:
                        if (state.timestamp > 50) {
                            state.timestamp = state.timestamp - 50;
                            tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForMenu("tutorial_" + state.timestamp));
                            state.blockingSound = tso;
                            state.environment.Add(tso);
                            state.timestamp += 10;
                        }
                        return false;
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
                if (state.timestamp == 55
                    || state.timestamp == 58
                    || state.timestamp == 60
                    || state.timestamp == 61
                    || state.timestamp == 63
                    || state.timestamp == 64
                    || state.timestamp == 65) {
                    state.timestamp -= 50;
                    CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForMenu("tutorial_" + state.timestamp));
                    state.blockingSound = tso;
                    state.environment.Add(tso);
                    state.timestamp += 10;
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
                    foreach (Actor actor in state.actors) {
                        Vector3 position = actor.position - new Vector3(initial_x, initial_y, 0f);
                        int x = (int)(position.x / offset_x);
                        int y = (int)(position.y / offset_y);
                        if (state.values[x] == state.expectedValues[x]) {
                            if (state.timestamp == 18) {
                                CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("boundary", new Vector3(0, 1, 0)));
                                state.environment.Add(tso);
                                state.blockingSound = tso;
                                state.timestamp = 118;
                            } else {
                                engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("boundary", new Vector3(0, 1, 0)), Vector3.zero));
                            }
                        } else {
                            if (state.timestamp == 15) {
                                CurveSoundObject tso;
                                state.values[x] = y;
                                if (state.values[x] == state.expectedValues[x]) {
                                    tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("correct"));
                                    state.environment.Add(tso);
                                    state.blockingSound = tso;
                                    state.timestamp = 116;
                                } else {
                                    tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(x, y, 0)));
                                    state.environment.Add(tso);
                                    state.blockingSound = tso;
                                    state.timestamp = 115;
                                }
                            } else {
                                state.values[x] = y;
                                if (state.values[x] == state.expectedValues[x]) {
                                    if (state.timestamp == 18 && state.values[1] == state.expectedValues[1] && state.values[2] == state.expectedValues[2]) {
                                        CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("correct"));
                                        state.environment.Add(tso);
                                        state.blockingSound = tso;
                                        state.timestamp = 59;
                                    } else if (state.timestamp == 21 && state.values[0] == state.expectedValues[0]) {
                                        CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("correct"));
                                        state.environment.Add(tso);
                                        state.blockingSound = tso;
                                        state.timestamp = 62;
                                    } else {
                                        engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("correct")));
                                    }
                                } else {
                                    engine.state.environment.Add(new CurveSoundObject("Prefabs/Curve/GameAudio", auEngine.getSoundForPlayer("note", new Vector3(x, y, 0)), Vector3.zero));
                                }
                            }
                        }
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
                if (x <= -1 || x >= max || y <= -1 || y >= 14) {
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