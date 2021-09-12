using UnityEngine.UI;
using UnityEngine;

public sealed class ScoreDisplay : MonoBehaviour {

    [SerializeField] Text[] roundPointTexts = new Text[2];
    [SerializeField] Text[] gamePointTexts = new Text[2];

    void Start() {
        GameManager.Instance.pointsChanged += OnPointsChanged;
    }

    void OnPointsChanged() {
        for (int i = 0; i < 2; i++) {
            roundPointTexts[i].text = GameManager.Instance.RoundPoints[i].ToString();
            gamePointTexts[i].text = GameManager.Instance.GamePoints[i].ToString();
        }
    }
}
