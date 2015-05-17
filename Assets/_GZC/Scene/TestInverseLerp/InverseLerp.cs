using UnityEngine;
using System.Collections;

public class InverseLerp : MonoBehaviour {

    public float walkSpeed = 5.0F;
    public float runSpeed = 10.0F;
    public float speed = 8.0F;

    void Start() {
        float parameter = Mathf.InverseLerp(walkSpeed, runSpeed, speed);
        print(parameter);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            speed++;
            float parameter = Mathf.InverseLerp(walkSpeed, runSpeed, speed);
            print(parameter);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            speed--;
            float parameter = Mathf.InverseLerp(walkSpeed, runSpeed, speed);
            print(parameter);
        }
    }

}
