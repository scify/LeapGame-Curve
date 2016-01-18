using UnityEngine;
using System.Collections;
using System.Linq;

public class pointerMovement : MonoBehaviour {

    float x = 0.5f;
    float y = 0.5f;
    float delay = 0f;
    int position = -1;
    int index = 0;
    int replayIndex = 0;
    int[] values = Enumerable.Repeat(-1, 14).ToArray();
    int replaying = 0;

	void Update () {
        if (replaying == 1) {
            if (replayIndex > index - 1) {
                replaying = 0;
                replayIndex = 0;
                return;
            }
            delay += Time.deltaTime;
            if (delay < 0.4) {
                return;
            }
            delay = 0f;
            gameObject.GetComponent<AudioSource>().clip = Resources.Load("sounds/" + values[replayIndex] + "_" + replayIndex) as AudioClip;
            gameObject.GetComponent<AudioSource>().Play();
            replayIndex++;
            return;
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            replaying = 1;
            return;
        } else if (Input.GetKeyDown(KeyCode.Return)) {
            values[index] = position;
            if (index < 13) {
                index++;
            }
        } else if (Input.GetKeyDown(KeyCode.DownArrow) && position > 0) {
            position--;
        } else if (Input.GetKeyDown(KeyCode.UpArrow) && position < 13) {
            position++;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) && index > 0) {
            index--;
            position = values[index];
        } else if (Input.GetKeyDown(KeyCode.RightArrow) && index < 13) {
            index++;
            position = values[index];
        } else {
            return;
        }
        gameObject.transform.position = new Vector3(x * index - 8f, y * position - 4f, 100f);
        gameObject.GetComponent<AudioSource>().clip = Resources.Load("sounds/" + position+"_"+index) as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
	}
}
