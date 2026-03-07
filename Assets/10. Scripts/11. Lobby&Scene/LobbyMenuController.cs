using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenuController : MonoBehaviour
{
    [Serializable]
    public struct MenuItem
    {
        public string itemName;
        public Button button;
        public GameObject arrow;
        public TMP_Text text;
        [HideInInspector] public bool isInteractable;
    }

    [Header("Menu Settings")] 
    [SerializeField] private List<MenuItem> menuItems;
    [SerializeField] private Color selectedColor = Color.brown;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color disabledColor = Color.gray;

    private int currentIndex = 0;

    private void Start()
    {
        InitializeMenu();
        UpdateSelection();
    }

    private void Update()
    {
        HandleNavigation();
        HandleSelection();
    }

    private void InitializeMenu()
    {
        for (int i = 0; i < menuItems.Count; ++i)
        {
            var item = menuItems[i];

            item.isInteractable = true;

            if (i == 1)
            {
                item.isInteractable = CheckSaveDataExists();
                
                if (item.button != null)
                    item.button.interactable = item.isInteractable;
            }

            menuItems[i] = item;
        }

        if (!menuItems[currentIndex].isInteractable)
            MoveIndex(1);
    }

    private bool CheckSaveDataExists() => Managers.Instance.Save.HasSaveFile();

    private void HandleNavigation()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveIndex(-1);
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            MoveIndex(1);
    }

    private void MoveIndex(int direction)
    {
        int nextIndex = currentIndex;
        int safetyNet = 0;

        while (safetyNet < menuItems.Count)
        {
            nextIndex = (nextIndex + direction + menuItems.Count) % menuItems.Count;
            if (menuItems[nextIndex].isInteractable)
            {
                currentIndex = nextIndex;
                UpdateSelection();
                break;
            }

            safetyNet++;
        }
    }

    private void HandleSelection()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (menuItems[currentIndex].isInteractable)
                ExecuteMenuAction(currentIndex);
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < menuItems.Count; ++i)
        {
            var item = menuItems[i];
            bool isSelected = (i == currentIndex);
            
            // 화살표 투명도 조절
            if (item.arrow != null)
                item.arrow.SetActive(isSelected);
            
            // 텍스트 색상 변경
            if (item.text != null)
            {
                if (item.isInteractable == false)
                    item.text.color = disabledColor;
                else
                    item.text.color = isSelected ? selectedColor : defaultColor;
            }

            
        }
    }

    private void ExecuteMenuAction(int index)
    {
        switch (index)
        {
            case 0:
                StartNewGame();
                break;
            case 1:
                LoadExistingGame();
                break;
            case 2:
                ExitGame();
                break;
        }
    }

    private void StartNewGame()
    {
        Debug.Log("Starting New Game ...");
        
        if (Managers.Instance.Flow != null)
            Managers.Instance.Flow.StartTutorial();
    }

    private void LoadExistingGame()
    {
        Debug.Log("Loading Game ...");
        SaveData loadedData = Managers.Instance.Save.LoadGame();
        if (loadedData != null)
        {
            SceneManager.LoadScene(loadedData.lastSceneName);
        }
    }

    private void ExitGame()
    {
        Debug.Log("Exiting Process ...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}