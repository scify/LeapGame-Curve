using UnityEngine;
using System.Collections;

public class curveDrawing : MonoBehaviour {

    int index = 0;
    float delay = 0f;
    float startDelay = 0f;
    float x = 0.5f;
    float y = 0.5f;
    public static int algo = 0;
    int reminding = 0;

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            algo = (algo + 1) % 9;
            Application.LoadLevel(Application.loadedLevel);
        } else if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)) {
            reminding = 1;
        }
        if (reminding == 1) {
            index = 0;
            delay = 0f;
            reminding = 0;
        }
        if (index == 14) {
            return;
        }
        startDelay += Time.deltaTime;
        delay += Time.deltaTime;
        if (startDelay < 1) {
            return;
        }
        if (delay < 0.4) {
            return;
        }
        delay = 0f;
        float value = 0;
        switch (algo) {
            case 0:
                value = index;
                break;
            case 1:
                value = 0.5f * index + 3.25f;
                break;
            case 2:
                value = 13f - index;
                break;
            case 3:
                value = (index + 1) * (index + 1) / 16f;
                break;
            case 4:
                value = Mathf.Sqrt(2f * index) * Mathf.Sqrt(6.5f);
                break;
            case 5:
                value = (Mathf.Sin(index * Mathf.PI / 4f) + 1f) * 6.5f;
                break;
            case 6:
                value = (index - 6.5f) * (index - 6.5f) / 6.5f;
                break;
            case 7:
                value = 13f / index;
                break;
            default:
                value = index * index * index / 169f;
                break;
        }
        index++;
        gameObject.transform.position = new Vector3(x * index, y * value - 4f, 100f);
        gameObject.GetComponent<AudioSource>().clip = Resources.Load("sounds/" + (int) value + "_" + index) as AudioClip;
        gameObject.GetComponent<AudioSource>().Play();
	}
}