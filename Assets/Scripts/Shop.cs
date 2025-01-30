using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour {

    public CanvasGroup canvasGroupShopSoulsPerSecond;
    public CanvasGroup canvasGroupShopSoulsPerCollection;
    public CanvasGroup canvasGroupShopSoulsPercentage;

    public void AlternateCanvasGroupShopSoulsPerSecond() {
        canvasGroupShopSoulsPerSecond.alpha = canvasGroupShopSoulsPerSecond.alpha == 0 ? 1 : 0;
        canvasGroupShopSoulsPerSecond.interactable = !canvasGroupShopSoulsPerSecond.interactable;
        canvasGroupShopSoulsPerSecond.blocksRaycasts = !canvasGroupShopSoulsPerSecond.blocksRaycasts;

        ActiveCanvasGroupShopSoulsPerCollection(false);
        ActiveCanvasGroupShopSoulsPercentage(false);
    }

    public void AlternateCanvasGroupShopSoulsPerCollection() {
        canvasGroupShopSoulsPerCollection.alpha = canvasGroupShopSoulsPerCollection.alpha == 0 ? 1 : 0;
        canvasGroupShopSoulsPerCollection.interactable = !canvasGroupShopSoulsPerCollection.interactable;
        canvasGroupShopSoulsPerCollection.blocksRaycasts = !canvasGroupShopSoulsPerCollection.blocksRaycasts;

        ActiveCanvasGroupShopSoulsPerSecond(false);
        ActiveCanvasGroupShopSoulsPercentage(false);
    }

    public void AlternateCanvasGroupShopSoulsPercentage() {
        canvasGroupShopSoulsPercentage.alpha = canvasGroupShopSoulsPercentage.alpha == 0 ? 1 : 0;
        canvasGroupShopSoulsPercentage.interactable = !canvasGroupShopSoulsPercentage.interactable;
        canvasGroupShopSoulsPercentage.blocksRaycasts = !canvasGroupShopSoulsPercentage.blocksRaycasts;

        ActiveCanvasGroupShopSoulsPerSecond(false);
        ActiveCanvasGroupShopSoulsPerCollection(false);
    }

    private void ActiveCanvasGroupShopSoulsPerSecond(bool active) {
        canvasGroupShopSoulsPerSecond.alpha = active ? 1 : 0;
        canvasGroupShopSoulsPerSecond.interactable = active;
        canvasGroupShopSoulsPerSecond.blocksRaycasts = active;
    }

    private void ActiveCanvasGroupShopSoulsPerCollection(bool active) {
        canvasGroupShopSoulsPerCollection.alpha = active ? 1 : 0;
        canvasGroupShopSoulsPerCollection.interactable = active;
        canvasGroupShopSoulsPerCollection.blocksRaycasts = active;
    }

    private void ActiveCanvasGroupShopSoulsPercentage(bool active) {
        canvasGroupShopSoulsPercentage.alpha = active ? 1 : 0;
        canvasGroupShopSoulsPercentage.interactable = active;
        canvasGroupShopSoulsPercentage.blocksRaycasts = active;
    }
}
