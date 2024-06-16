using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro
using Supabase;
using Supabase.Gotrue;
using Postgrest.Models;
using Postgrest.Attributes;
using Newtonsoft.Json;
using com.example;  // Add this line to reference the SupabaseManager namespace
using UnityEngine.SceneManagement;

namespace SaveLoad
{
    [Table("save_files")]
    public class InventoryDataModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("filename")]
        public string Filename { get; set; }

        [Column("is_available")]
        public bool IsAvailable { get; set; }

        [Column("file")]
        public Dictionary<string, object> File { get; set; }
    }

    public class LoadManager : MonoBehaviour
    {
        public Transform contentTransform; 
        public GameObject rowPrefab; 
        public Button fetchButton; 
        public string nextScene;

        private void Start()
        {
            if (contentTransform == null)
            {
                Debug.LogError("Content Transform is not assigned!");
                return;
            }

            if (rowPrefab == null)
            {
                Debug.LogError("Row Prefab is not assigned!");
                return;
            }

            if (fetchButton == null)
            {
                Debug.LogError("Fetch Button is not assigned!");
                return;
            }

            fetchButton.onClick.AddListener(FetchAndDisplayFilenames);

            // Auto-fetch the filenames when the scene loads
            FetchAndDisplayFilenames();
        }

        public async void FetchAndDisplayFilenames()
        {
            try
            {
                var response = await SupabaseManager.Instance.Supabase()
                     .From<InventoryDataModel>()
                     .Where(x => x.IsAvailable == true)
                     .Select("id, filename, created_at")
                     .Get();

                List<InventoryDataModel> maps = response.Models;

                // Clear any existing children in the Content GameObject
                foreach (Transform child in contentTransform)
                {
                    Destroy(child.gameObject);
                }

                // Instantiate a Row prefab for each map
                foreach (var map in maps)
                {
                    GameObject newRow = Instantiate(rowPrefab, contentTransform);

                    // Set the Button text
                    Transform buttonTransform = newRow.transform.Find("Button");
                    if (buttonTransform != null)
                    {
                        TMP_Text buttonText = buttonTransform.GetComponentInChildren<TMP_Text>();
                        if (buttonText != null)
                        {
                            buttonText.text = map.Filename;
                        }

                        Button buttonComponent = buttonTransform.GetComponent<Button>();
                        if (buttonComponent != null)
                        {
                            buttonComponent.onClick.AddListener(() => OnMapSelected(map));
                        }
                    }

                    // Set the Created At text
                    Transform textTransform = newRow.transform.Find("Text");
                    if (textTransform != null)
                    {
                        TMP_Text textComponent = textTransform.GetComponentInChildren<TMP_Text>();
                        if (textComponent != null)
                        {
                            textComponent.text = map.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error fetching maps from Supabase: {e.Message}");
            }
        }

        private async void OnMapSelected(InventoryDataModel map)
        {
            try
            {
                var response = await SupabaseManager.Instance.Supabase()
                    .From<InventoryDataModel>()
                    .Where(x => x.Id == map.Id)
                    .Select("file")
                    .Single();

                if (response != null && response.File != null)
                {
                    string fileContent = JsonConvert.SerializeObject(response.File);
                    Debug.Log($"Fetched file content: {fileContent}");

                    // Save the file content to a .json file
                    string saveFilePath = Path.Combine(Application.persistentDataPath, $"{map.Filename}.json");
                    File.WriteAllText(saveFilePath, fileContent);
                    Debug.Log($"File saved to: {saveFilePath}");

                    SaveFileManager.Instance.SaveFilePath = saveFilePath;

                    // Load the next scene asynchronously
                    StartCoroutine(LoadSceneAndInitializeGameLoader(nextScene));
                }
                else
                {
                    Debug.LogWarning("No file content found.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error fetching file content from Supabase: {e.Message}");
            }
        }

        private IEnumerator LoadSceneAndInitializeGameLoader(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            // Scene is loaded, now initialize the GameLoader
            GameLoader gameLoader = FindObjectOfType<GameLoader>();
            if (gameLoader != null)
            {
                gameLoader.Initialize();
            }
            else
            {
                Debug.LogError("GameLoader not found in the loaded scene.");
            }
        }
    }
}
