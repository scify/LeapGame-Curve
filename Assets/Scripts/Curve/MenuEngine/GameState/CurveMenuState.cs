
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurveMenuState : GameState {

    public List<CurveSoundObject> stoppableSounds = new List<CurveSoundObject>();

    public CurveMenuState(List<WorldObject> environment) {
        timestamp = 0;
        this.actors = new List<Actor>();
        this.environment = environment;
        this.players = new List<Player>();
        curPlayer = 0;
        result = new CurveMenuResult(CurveMenuResult.GameStatus.Choosing);
    }

    public CurveMenuState(CurveMenuState previousState) {
        timestamp = previousState.timestamp;
        actors = new List<Actor>();
        players = new List<Player>();
        environment = new List<WorldObject>(previousState.environment);
        curPlayer = previousState.curPlayer;
    }
}