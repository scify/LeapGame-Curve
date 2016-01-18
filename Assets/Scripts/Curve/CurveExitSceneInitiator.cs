using UnityEngine;
using System.Collections.Generic;

public class CurveExitSceneInitiator : MonoBehaviour {

    public float offset_x;
    public float offset_y;

    void Start() {
        CurveStateRenderer renderer = new CurveStateRenderer();
        AudioEngine auEngine = new AudioEngine(0, Settings.game_name, Settings.menu_sounds, Settings.game_sounds);

        List<WorldObject> environment = new List<WorldObject>();
        environment.Add(new CurveStaticObject("Prefabs/Curve/Camera_Default", new Vector3(0, 10, 0), false));
        environment.Add(new CurveStaticObject("Prefabs/Curve/Light_Default", new Vector3(0, 10, 0), false));
        environment.Add(new CanvasObject("Prefabs/Curve/OutroLogo", true, new Vector3(0, 0, 0), false));

        CurveRuleset rules = new CurveRuleset();
        rules.Add(new CurveRule("initialization", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            CurveSoundObject tso = new CurveSoundObject("Prefabs/Curve/AudioSource", auEngine.getSoundForMenu("outro"), Vector3.zero);
            state.environment.Add(tso);
            state.stoppableSounds.Add(tso);
            return false;
        }));

        rules.Add(new CurveRule("soundOver", (CurveMenuState state, GameEvent eve, CurveMenuEngine engine) => {
            Application.Quit();
            return false;
        }));

        gameObject.AddComponent<CurveMenuEngine>();
        gameObject.AddComponent<CurveMenuUserInterface>();
        gameObject.GetComponent<CurveMenuEngine>().initialize(rules, environment, renderer);
        gameObject.GetComponent<CurveMenuUserInterface>().initialize(gameObject.GetComponent<CurveMenuEngine>());
        gameObject.GetComponent<CurveMenuEngine>().postEvent(new GameEvent("", "initialization", "unity"));
    }
}
