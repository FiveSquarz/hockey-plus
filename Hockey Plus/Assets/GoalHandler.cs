using UnityEngine;

public sealed class GoalHandler : MonoBehaviour {
    [SerializeField, Range(0, 1)] int pointForPlayer;
    void OnTriggerEnter2D(Collider2D col) {
        if (col.GetComponent<Ball>()) {
            GameManager.Instance.AddRoundPoint(pointForPlayer);
            Destroy(col.gameObject, 1f);
        }
    }
}
