using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CurveMenuEngine : GameEngine {

    public new CurveStateRenderer renderer;
    public CurveRuleset rules;
    public CurveMenuState state;
    public List<WorldObject> environment;
    public Queue<GameEvent> events;

    private bool initialized = false;

    public void initialize(CurveRuleset rules, List<WorldObject> environment, CurveStateRenderer renderer) {
        this.rules = rules;
        this.environment = environment;
        this.renderer = renderer;
        state = new CurveMenuState(environment);
        events = new Queue<GameEvent>();
        state.curPlayer = -1;
        initialized = true;
    }

	public void Start() {
        run();
	}

    public override void run() {
    }

    public void Update() {
        loop();
    }

    public override void loop() {
        if (!initialized) {
            return;
        }
        while (events.Count != 0) {
            if (state.result.gameOver()) {
                cleanUp();
                return;
            }
            GameEvent curEvent = events.Dequeue();
            rules.applyTo(state, curEvent, this);
        }
        renderer.render(this);
        if (state.result.gameOver()) {
            cleanUp();
        }
	}

    public override void postEvent(GameEvent eve) {
        events.Enqueue(eve);
    }

    public override GameState getState() {
        return state;
    }

    public override StateRenderer getRenderer() {
        return renderer;
    }

    public override void cleanUp() {
        Application.LoadLevel(Settings.previousMenu);
	}

}