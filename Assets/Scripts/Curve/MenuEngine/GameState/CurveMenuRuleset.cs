using UnityEngine;
using System.Collections.Generic;
using System;

public class CurveMenuRuleset : Ruleset<CurveMenuRule> {

    public override void applyTo(GameState state, GameEvent eve, GameEngine engine) {
        throw new ArgumentException("Invalid game state type! Expected a CurveGameState, got " + state.GetType().ToString(), "state");
    }

    public void applyTo(CurveMenuState state, GameEvent eve, CurveMenuEngine engine) {
        List<CurveMenuRule> rules = this.FindAll(delegate(CurveMenuRule rule) {
            return rule.category.Equals(eve.type) || rule.category.Equals("ALL");
        });
        foreach (CurveMenuRule rule in rules) {
            if (!rule.applyTo(state, eve, engine)) {
                return;
            }
        };
    }

}