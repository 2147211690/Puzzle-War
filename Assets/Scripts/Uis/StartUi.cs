using Controllers;
using TTSDK;
using UnityEngine;

namespace Uis
{
    public class StartUi : MonoBehaviour
    {
        public PuzzleController puzzleController;
        public Vector2Int puzzleSize;
        public Texture2D puzzleTexture;

        public GameObject startUi;
        public GameObject gammingUi;
        public void StartGame()
        {
            puzzleController.Init(puzzleSize, puzzleTexture);
            startUi.SetActive(false);
            gammingUi.SetActive(true);
        }

        public void ReplayGame()
        {
            puzzleController.Init(puzzleSize, puzzleTexture);
        }
    }
}