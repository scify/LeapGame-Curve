using UnityEngine;
using System.Collections.Generic;
using System;

public class CurveRuleset : Ruleset<CurveRule> {

    public override void applyTo(GameState state, GameEvent eve, GameEngine engine) {
        throw new ArgumentException("Invalid game state type! Expected a CurveGameState, got " + state.GetType().ToString(), "state");
    }

    public void applyTo(CurveMenuState state, GameEvent eve, CurveMenuEngine engine) {
        List<CurveRule> rules = this.FindAll(delegate(CurveRule rule) {
            return rule.category.Equals(eve.type) || rule.category.Equals("ALL");
        });
        foreach (CurveRule rule in rules) {
            if (!rule.applyTo(state, eve, engine)) {
                return;
            }
        };
    }
    public void applyTo(CurveGameState state, GameEvent eve, CurveGameEngine engine) {
        List<CurveRule> rules = this.FindAll(delegate(CurveRule rule) {
            return rule.category.Equals(eve.type) || rule.category.Equals("ALL");
        });
        foreach (CurveRule rule in rules) {
            if (!rule.applyTo(state, eve, engine)) {
                return;
            }
        };
    }

}