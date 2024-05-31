using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InputTrigger))]
public class InputTriggerEditor : Editor
{
    private bool showError = false;

    public override void OnInspectorGUI()
    {
        // Get the InputTrigger script
        InputTrigger inputTrigger = (InputTrigger)target;

        // Serialized properties
        SerializedProperty eventTriggerProp = serializedObject.FindProperty("EventTrigger");

        // Begin property field check
        EditorGUI.BeginChangeCheck();

        // Draw the EventTrigger property field
        EditorGUILayout.PropertyField(eventTriggerProp);

        // If the field has changed
        if (EditorGUI.EndChangeCheck())
        {
            // Apply changes to serialized properties
            serializedObject.ApplyModifiedProperties();

            // Check if EventTrigger is assigned
            if (inputTrigger.EventTrigger != null)
            {
                // Check if the assigned object has the correct tag
                if (!inputTrigger.EventTrigger.CompareTag("EventTriggerPerformAction"))
                {
                    // If the tag is incorrect, reset the EventTrigger field
                    Debug.LogWarning("Assigned object does not have the correct tag! Resetting EventTrigger.");
                    eventTriggerProp.objectReferenceValue = null;
                    serializedObject.ApplyModifiedProperties();
                    showError = true;
                }
                else
                {
                    showError = false;
                }
            }
            else
            {
                showError = false;
            }
        }

        // Display the error message if needed
        if (showError)
        {
            EditorGUILayout.HelpBox("The assigned object does not have the correct tag!", MessageType.Error);
        }

        // Draw other properties excluding EventTrigger
        DrawPropertiesExcluding(serializedObject, "EventTrigger");

        // Apply the modified properties
        serializedObject.ApplyModifiedProperties();
    }
}
