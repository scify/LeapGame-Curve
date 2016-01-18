using UnityEngine;
using System.Collections;
using System;

public class CurveRule : Rule {

    public delegate bool Applicator(CurveGameState state, GameEvent eve, CurveGameEngine engine);
    public delegate bool MenuApplicator(CurveMenuState state, GameEvent eve, CurveMenuEngine engine);
    public Applicator apllicator;
    public MenuApplicator menuApllicator;

    public CurveRule(string category, Applicator applier) : base(category) {
        apllicator = applier;
    }
    public CurveRule(string category, MenuApplicator applier)
        : base(category) {
            menuApllicator = applier;
    }

    public override bool applyTo(GameState state, GameEvent eve, GameEngine engine) {
        throw new ArgumentException("Invalid game state type! Expected a CurveGameState, got " + state.GetType().ToString());
    }

    public bool applyTo(CurveGameState state, GameEvent eve, CurveGameEngine engine) {
        return apllicator(state, eve, engine);
    }
    public bool applyTo(CurveMenuState state, GameEvent eve, CurveMenuEngine engine) {
        return menuApllicator(state, eve, engine);
    }
	
}