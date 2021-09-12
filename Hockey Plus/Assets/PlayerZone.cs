using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PlayerZone : MonoBehaviour, IPointerDownHandler, IDragHandler {

    [SerializeField] float maxSpeed;
    [SerializeField] Transform player;
    [SerializeField] bool overflowForward = true;

    Vector2 target;

    BoxCollider2D zone;
    Rigidbody2D playerRb;
    CircleCollider2D playerCol;

    void Awake() {
        zone = GetComponent<BoxCollider2D>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerCol = player.GetComponent<CircleCollider2D>();

        target = player.position;
    }

    public void OnPointerDown(PointerEventData eventData) {
        UpdatePosition(eventData.position);
    }

    public void OnDrag(PointerEventData eventData) {
        UpdatePosition(eventData.position);
    }

    void UpdatePosition(Vector2 screenPosition) {
        Vector2 areaCenter = transform.InverseTransformDirection(transform.position);
        Vector2 areaExtents = zone.size * transform.lossyScale / 2f;
        float playerRadius = playerCol.radius;
        Vector2 position = transform.InverseTransformDirection((Camera.main.ScreenToWorldPoint(screenPosition)));
        position.x = Mathf.Clamp(position.x, areaCenter.x - areaExtents.x + playerRadius, areaCenter.x + areaExtents.x - playerRadius);
        position.y = Mathf.Clamp(position.y, areaCenter.y - areaExtents.y + playerRadius, areaCenter.y + areaExtents.y - (overflowForward ? 0f : playerRadius));

        target = transform.TransformDirection(position);
    }

    void FixedUpdate() {
        if (GameManager.Instance.Status == GameManager.GameStatus.Playing) {
            playerRb.MovePosition(playerRb.position + Vector2.ClampMagnitude(target - playerRb.position, maxSpeed * Time.fixedDeltaTime));
        } else {
            target = player.position;
            playerRb.velocity = Vector3.zero;
        }
    }
}