using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ThemeItem))]
public class ThemeItemDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        SerializedProperty priceType = property.FindPropertyRelative("PriceType");

        int count = 4;

        if (priceType.enumValueIndex == (int)ThemePriceType.BuyCoin ||
            priceType.enumValueIndex == (int)ThemePriceType.BuyGem)
        {
            count++;
        }

        return count * (EditorGUIUtility.singleLineHeight + 2) + 6;
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(position, property.FindPropertyRelative("Theme"));
        position.y += position.height + 2;

    

        EditorGUI.PropertyField(position, property.FindPropertyRelative("Name_Theme"));
        position.y += position.height + 2;

        SerializedProperty priceType = property.FindPropertyRelative("PriceType");

        EditorGUI.PropertyField(position, priceType);
        position.y += position.height + 2;

        if (priceType.enumValueIndex == (int)ThemePriceType.BuyCoin)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("BuyCoin"));
        }
        else if (priceType.enumValueIndex == (int)ThemePriceType.BuyGem)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("BuyGem"));
        }

        EditorGUI.EndProperty();
    }
}