using UnityEngine;
using System.Collections;
using System;

public class CurveMenuRule : Rule {

    public delegate bool Applicator(CurveMenuState state, GameEvent eve, CurveMenuEngine engine);
    public Applicator apllicator;

    public CurveMenuRule(string category, Applicator applier) : base(category) {
        apllicator = applier;
    }

    public override bool applyTo(GameState state, GameEvent eve, GameEngine engine) {
        throw new ArgumentException("Invalid game state type! Expected a CurveMenuGameState, got " + state.GetType().ToString());
    }

    public bool applyTo(CurveMenuState state, GameEvent eve, CurveMenuEngine engine) {
        return apllicator(state, eve, engine);
    }
	
}