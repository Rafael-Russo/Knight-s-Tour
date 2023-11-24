using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public BoardController boardController;
    public KnightsTour ktController;
    public GameObject wKnight;
    public GameObject bKnight;
    public Camera mainCamera;
    public Slider speedSlider;
    public TMP_InputField txtBoardSize;
    protected internal bool isRunning;

    public void OnSliderValueChanged()
    {
        ktController.mvSpeed = speedSlider.value;
    }
    public void setBoardSize()
    {
        if(txtBoardSize.text != "")
        {
            int.TryParse(txtBoardSize.text, out boardController.boardSize);
            boardController.Generate();
        }
        else
        {
            boardController.boardSize = 8;
            boardController.Generate();
        }
    }
    public void setWhiteKnight()
    {
        boardController.knightPrefab = wKnight;
    }

    public void setBlackKnight()
    {
        boardController.knightPrefab = bKnight;
    }

    public void ValidateIntegerInput()
    {
        string input = txtBoardSize.text;
        if (!int.TryParse(input, out _))
        {
            txtBoardSize.text = "";
            // Você também pode adicionar aqui uma lógica para mostrar uma mensagem de erro
        }
    }

    public void doKT()
    {
        if (ktController.tourComplete)
        {
            SceneManager.LoadScene("SampleScene");
            return;
        }

        if(ktController.path == null)
        {
            boardController.PlaceOrMoveKnight(new Vector3(0, 0.5f, 0));
            ktController.PrepareKnightsTour(new Vector2(0, 0), boardController, this);
        }
        isRunning = true;
        Debug.Log(boardController.currentKnight.transform.position);
        ktController.StartKnightsTour();
    }
}
