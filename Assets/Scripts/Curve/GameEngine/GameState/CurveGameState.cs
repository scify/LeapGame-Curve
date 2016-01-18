using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CurveGameState : GameState {

    public const int MAX_LEVEL = 10;
    public const float TIME_PER_LEVEL = 180;

    public CurveSoundObject blockingSound;
    public float time_left;
    public int level;
    public bool replaying;
    public bool repeating;
    public int[] values = new int[14];
    public int[] expectedValues = new int[14];
    public float delay = 0f;
    public int index = 0;

    public CurveGameState(List<Actor> actors, List<WorldObject> environment, List<Player> players) {
        timestamp = 0;
        this.actors = actors;
        this.environment = environment;
        this.players = players;
        curPlayer = 0;
        values = new int[14];
        for (int i = 0; i < 14; i++) {
            values[i] = 0;
        }
        result = new CurveGameResult(CurveGameResult.GameStatus.Ongoing, -1);
        blockingSound = null;
        level = 1;
        time_left = TIME_PER_LEVEL;
        replaying = false;
        repeating = false;
    }
}