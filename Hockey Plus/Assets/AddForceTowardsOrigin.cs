using UnityEngine;

public sealed class AddForceTowardsOrigin : MonoBehaviour {

    [SerializeField] float power = 1f;

    void OnTriggerStay2D(Collider2D col) {
        if (col.attachedRigidbody) {
            col.attachedRigidbody.AddForce((Vector2.zero - col.attachedRigidbody.position).normalized * power, ForceMode2D.Impulse);
        }
    }
}
