using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Player_MovementController))]
public class Player_MovementControllerEditor : Editor
{
    // Visual tree asset
    [SerializeField] private VisualTreeAsset visualTreeAsset;

    // Foldout states
    private static readonly Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    //== Create Inspector GUI
    public override VisualElement CreateInspectorGUI()
    {
        // Get root
        var root = visualTreeAsset.CloneTree();

        // Get all foldouts
        string[] foldoutNames = { "Movement", "Jumping", "Physics", "CollisionDetection" };
        // For each foldout, check active state
        foreach (var foldoutName in foldoutNames)
        {
            // Get foldout
            var foldout = root.Q<Foldout>(foldoutName);
            // If the foldout does not exist, skip checl
            if (foldout == null) continue;

            // Create a unique key for foldout
            string key = $"{target.GetInstanceID()}_{foldoutName}";

            // Load previous state
            foldout.value = foldoutStates.TryGetValue(key, out var state) ? state : true;

            // Register callback to save foldout state
            foldout.RegisterValueChangedCallback(evt =>
            {
                foldoutStates[key] = evt.newValue;

                // Store in editor preferences to persist across sessions
                EditorPrefs.SetBool(key, evt.newValue);
            });

            // Load from editor preferences to persist across sessions
            if (EditorPrefs.HasKey(key))
                foldout.value = EditorPrefs.GetBool(key);
        }

        // Return root
        return root;
    }
}
