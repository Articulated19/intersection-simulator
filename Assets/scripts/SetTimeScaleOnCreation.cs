using UnityEngine;

public class SetTimeScaleOnCreation : MonoBehaviour {

    public float timeScale = 1f;
    public bool destroyObject = true;

    void Start () {
      Time.timeScale = timeScale;
    }

  void FixedUpdate() {
      Time.timeScale = timeScale;
  }
}
