using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text score;
    public Image jumpCooldown;
    public PlayerController player;
    public GameObject gameOverCanvas;
	
	// Update is called once per frame
	void Update () {
        score.text = player.maxHeight.ToString();

        float width;

        if (player.currentPushRecovery <= player.pushRecovery)
        {
            width =  PixelRounder.Remap(player.currentPushRecovery, 0, player.pushRecovery, 0, 100);
        }
        else
        {
            width = 100;
        }

        jumpCooldown.rectTransform.sizeDelta = new Vector2(width, jumpCooldown.rectTransform.sizeDelta.y);

        if (player.isDead)
        {
            Time.timeScale = 0;
            gameOverCanvas.SetActive(true);
        }
	}
}
