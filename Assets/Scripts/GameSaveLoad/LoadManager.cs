using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this for TextMeshPro
using Supabase;
using Supabase.Gotrue;
using Postgrest.Models;
using Postgrest.Attributes;
using Newtonsoft.Json;
using com.example;  // Add this line to reference the SupabaseManager namespace

namespace GameSaveLoad
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
        public string IsAvailable { get; set; }

        // [Column("file")] // Remove or comment out this line if not needed
        // public string File { get; set; }
    }

    public class LoadManager : MonoBehaviour
    {
        public Transform contentTransform;  // Assign the Content GameObject of the Scroll View in the Inspector
        public GameObject rowPrefab;  // Assign the Row prefab in the Inspector

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

            FetchAndDisplayFilenames();
        }

        public async void FetchAndDisplayFilenames()
        {
            try
            {
                var response = await SupabaseManager.Instance.Supabase().From<InventoryDataModel>().Get();
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

        private void OnMapSelected(InventoryDataModel map)
        {
            Debug.Log($"Selected map: {map.Filename}");
            // Add your selection logic here
        }
    }
}
