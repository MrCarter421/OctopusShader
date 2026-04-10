using UnityEngine;
using UnityEditor;

public class OctopusCamouflageInspector : ShaderGUI
{
    // Foldout states
    bool showBase = true;
    bool showLayerBlending = true;
    bool showLeucophore = true;
    bool showIridophore = true;
    bool showChromatophore = true;
    bool showRedChannel = true;
    bool showYellowChannel = true;
    bool showBrownChannel = true;
    bool showBlackChannel = true;
    bool showWave = true;
    bool showTransition = false;
    bool showPapillae = false;
    bool showAnimation = true;
    bool showRendering = false;
    bool showPresets = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        Material material = materialEditor.target as Material;

        EditorGUILayout.LabelField("OCTOPUS CAMOUFLAGE SHADER", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Biologically-inspired chromatophore system", EditorStyles.miniLabel);
        EditorGUILayout.Space(8);

        // ── PRESETS ──
        showPresets = EditorGUILayout.Foldout(showPresets, "PRESETS", true, EditorStyles.foldoutHeader);
        if (showPresets)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Resting")) ApplyPresetResting(material);
            if (GUILayout.Button("Alert")) ApplyPresetAlert(material);
            if (GUILayout.Button("Sandy Camo")) ApplyPresetSandyCamo(material);
            if (GUILayout.Button("Deep Ocean")) ApplyPresetDeepOcean(material);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Coral Reef")) ApplyPresetCoralReef(material);
            if (GUILayout.Button("Threat Display")) ApplyPresetThreat(material);
            if (GUILayout.Button("Passing Cloud")) ApplyPresetPassingCloud(material);
            if (GUILayout.Button("Reset All")) ApplyPresetReset(material);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
        }

        // ── BASE MESH ──
        showBase = EditorGUILayout.Foldout(showBase, "BASE MESH", true, EditorStyles.foldoutHeader);
        if (showBase)
        {
            EditorGUI.indentLevel++;
            DrawProperty(materialEditor, properties, "_MainTex");
            DrawProperty(materialEditor, properties, "_NormalMap");
            DrawProperty(materialEditor, properties, "_NormalStrength");
            EditorGUI.indentLevel--;
        }

        // ── LAYER BLENDING ──
        showLayerBlending = EditorGUILayout.Foldout(showLayerBlending,
            "LAYER BLENDING (Base Texture Visibility)", true, EditorStyles.foldoutHeader);
        if (showLayerBlending)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "Controls how much each biological layer covers the base texture. " +
                "Lower Chromatophore Opacity to let your base texture details show through. " +
                "Multiply mode tints the base; Overlay preserves contrast.", MessageType.None);
            DrawProperty(materialEditor, properties, "_BaseTextureStrength");
            DrawProperty(materialEditor, properties, "_LeucophoreOpacity");
            DrawProperty(materialEditor, properties, "_IridophoreOpacity");
            DrawProperty(materialEditor, properties, "_ChromatophoreOpacity");
            DrawProperty(materialEditor, properties, "_CamoTransitionOpacity");
            DrawProperty(materialEditor, properties, "_BlendMode");
            EditorGUI.indentLevel--;
        }

        // ── LEUCOPHORE ──
        showLeucophore = EditorGUILayout.Foldout(showLeucophore,
            "LEUCOPHORE LAYER (Bottom - Ambient Reflection)", true, EditorStyles.foldoutHeader);
        if (showLeucophore)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "White scattering cells that reflect ambient light wavelengths. " +
                "Acts as the neutral backdrop for all layers above.", MessageType.None);
            DrawProperty(materialEditor, properties, "_LeucophoreColor");
            DrawProperty(materialEditor, properties, "_LeucophoreAmbientBlend");
            DrawProperty(materialEditor, properties, "_LeucophoreIntensity");
            EditorGUI.indentLevel--;
        }

        // ── IRIDOPHORE ──
        showIridophore = EditorGUILayout.Foldout(showIridophore,
            "IRIDOPHORE LAYER (Middle - Iridescence)", true, EditorStyles.foldoutHeader);
        if (showIridophore)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "Reflective plates creating iridescent greens, blues, golds. " +
                "View-angle dependent, like thin-film interference.", MessageType.None);
            DrawProperty(materialEditor, properties, "_IridophoreStrength");
            DrawProperty(materialEditor, properties, "_IridophoreScale");
            DrawProperty(materialEditor, properties, "_IridophoreSpeed");
            DrawProperty(materialEditor, properties, "_IridophoreHueShift");
            DrawProperty(materialEditor, properties, "_IridophoreHueRange");
            DrawProperty(materialEditor, properties, "_IridophoreFresnelPower");
            EditorGUI.indentLevel--;
        }

        // ── CHROMATOPHORE ──
        showChromatophore = EditorGUILayout.Foldout(showChromatophore,
            "CHROMATOPHORE LAYER (Top - Pigment Cells)", true, EditorStyles.foldoutHeader);
        if (showChromatophore)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "Thousands of elastic pigment sacs controlled by muscles. " +
                "Each channel has independent expansion and wave response.", MessageType.None);
            DrawProperty(materialEditor, properties, "_ChromatophoreScale");
            DrawProperty(materialEditor, properties, "_ChromatophoreEdge");

            EditorGUILayout.Space(5);
            showRedChannel = EditorGUILayout.Foldout(showRedChannel, "Red Pigment Channel", true);
            if (showRedChannel)
            {
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, properties, "_RedPigment");
                DrawProperty(materialEditor, properties, "_RedExpansion");
                DrawProperty(materialEditor, properties, "_RedWaveInfluence");
                EditorGUI.indentLevel--;
            }

            showYellowChannel = EditorGUILayout.Foldout(showYellowChannel, "Yellow Pigment Channel", true);
            if (showYellowChannel)
            {
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, properties, "_YellowPigment");
                DrawProperty(materialEditor, properties, "_YellowExpansion");
                DrawProperty(materialEditor, properties, "_YellowWaveInfluence");
                EditorGUI.indentLevel--;
            }

            showBrownChannel = EditorGUILayout.Foldout(showBrownChannel, "Brown Pigment Channel", true);
            if (showBrownChannel)
            {
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, properties, "_BrownPigment");
                DrawProperty(materialEditor, properties, "_BrownExpansion");
                DrawProperty(materialEditor, properties, "_BrownWaveInfluence");
                EditorGUI.indentLevel--;
            }

            showBlackChannel = EditorGUILayout.Foldout(showBlackChannel, "Black Pigment Channel", true);
            if (showBlackChannel)
            {
                EditorGUI.indentLevel++;
                DrawProperty(materialEditor, properties, "_BlackPigment");
                DrawProperty(materialEditor, properties, "_BlackExpansion");
                DrawProperty(materialEditor, properties, "_BlackWaveInfluence");
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        // ── WAVE PROPAGATION ──
        showWave = EditorGUILayout.Foldout(showWave,
            "WAVE PROPAGATION (Neural Cascade)", true, EditorStyles.foldoutHeader);
        if (showWave)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "Simulates neural signal cascading across chromatophore arrays, " +
                "creating the signature rippling color waves.", MessageType.None);
            DrawProperty(materialEditor, properties, "_WaveOrigin");
            DrawProperty(materialEditor, properties, "_WaveSpeed");
            DrawProperty(materialEditor, properties, "_WaveFrequency");
            DrawProperty(materialEditor, properties, "_WaveAmplitude");
            DrawProperty(materialEditor, properties, "_WaveDecay");
            DrawProperty(materialEditor, properties, "_WaveRadial");
            DrawProperty(materialEditor, properties, "_WaveDirection");
            EditorGUILayout.Space(3);
            DrawProperty(materialEditor, properties, "_MultiWave");
            DrawProperty(materialEditor, properties, "_WaveOrigin2");
            DrawProperty(materialEditor, properties, "_WaveSpeed2");
            DrawProperty(materialEditor, properties, "_WaveFrequency2");
            EditorGUI.indentLevel--;
        }

        // ── CAMOUFLAGE TRANSITION ──
        showTransition = EditorGUILayout.Foldout(showTransition,
            "CAMOUFLAGE TRANSITION", true, EditorStyles.foldoutHeader);
        if (showTransition)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "Blend toward a target camouflage color with organic noise-driven edges. " +
                "Animate CamoBlend from 0 to 1 for full transition.", MessageType.None);
            DrawProperty(materialEditor, properties, "_TargetColor");
            DrawProperty(materialEditor, properties, "_CamoBlend");
            DrawProperty(materialEditor, properties, "_CamoNoiseTex");
            DrawProperty(materialEditor, properties, "_CamoNoiseScale");
            DrawProperty(materialEditor, properties, "_CamoNoiseContrast");
            DrawProperty(materialEditor, properties, "_TransitionSharpness");
            EditorGUI.indentLevel--;
        }

        // ── PAPILLAE ──
        showPapillae = EditorGUILayout.Foldout(showPapillae,
            "PAPILLAE (Texture Bumps)", true, EditorStyles.foldoutHeader);
        if (showPapillae)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.HelpBox(
                "Muscle bundles that raise or smooth the skin surface, " +
                "mimicking rocky or coral textures.", MessageType.None);
            DrawProperty(materialEditor, properties, "_PapillaeStrength");
            DrawProperty(materialEditor, properties, "_PapillaeScale");
            DrawProperty(materialEditor, properties, "_PapillaeSharpness");
            EditorGUI.indentLevel--;
        }

        // ── ANIMATION ──
        showAnimation = EditorGUILayout.Foldout(showAnimation,
            "ANIMATION", true, EditorStyles.foldoutHeader);
        if (showAnimation)
        {
            EditorGUI.indentLevel++;
            DrawProperty(materialEditor, properties, "_GlobalSpeed");
            DrawProperty(materialEditor, properties, "_PulseSpeed");
            DrawProperty(materialEditor, properties, "_PulseAmplitude");
            DrawProperty(materialEditor, properties, "_BreathingSpeed");
            DrawProperty(materialEditor, properties, "_BreathingAmount");
            EditorGUI.indentLevel--;
        }

        // ── RENDERING ──
        showRendering = EditorGUILayout.Foldout(showRendering,
            "RENDERING", true, EditorStyles.foldoutHeader);
        if (showRendering)
        {
            EditorGUI.indentLevel++;
            DrawProperty(materialEditor, properties, "_Glossiness");
            DrawProperty(materialEditor, properties, "_Metallic");
            DrawProperty(materialEditor, properties, "_OcclusionStrength");
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space(10);
        materialEditor.RenderQueueField();
    }

    void DrawProperty(MaterialEditor editor, MaterialProperty[] props, string name)
    {
        MaterialProperty prop = FindProperty(name, props, false);
        if (prop != null)
            editor.ShaderProperty(prop, prop.displayName);
    }

    // ─────────────────────────────────────────────────
    // PRESETS
    // ─────────────────────────────────────────────────

    void ApplyPresetResting(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Resting");
        m.SetFloat("_RedExpansion", 0.2f);
        m.SetFloat("_YellowExpansion", 0.15f);
        m.SetFloat("_BrownExpansion", 0.3f);
        m.SetFloat("_BlackExpansion", 0.1f);
        m.SetFloat("_WaveAmplitude", 0.1f);
        m.SetFloat("_WaveSpeed", 0.5f);
        m.SetFloat("_IridophoreStrength", 0.15f);
        m.SetFloat("_PulseAmplitude", 0.02f);
        m.SetFloat("_BreathingAmount", 0.02f);
        m.SetFloat("_PapillaeStrength", 0.0f);
        m.SetFloat("_CamoBlend", 0.0f);
    }

    void ApplyPresetAlert(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Alert");
        m.SetFloat("_RedExpansion", 0.85f);
        m.SetFloat("_YellowExpansion", 0.4f);
        m.SetFloat("_BrownExpansion", 0.6f);
        m.SetFloat("_BlackExpansion", 0.7f);
        m.SetFloat("_WaveAmplitude", 0.8f);
        m.SetFloat("_WaveSpeed", 5.0f);
        m.SetFloat("_WaveFrequency", 4.0f);
        m.SetFloat("_IridophoreStrength", 0.5f);
        m.SetFloat("_PulseSpeed", 4.0f);
        m.SetFloat("_PulseAmplitude", 0.1f);
        m.SetFloat("_BreathingAmount", 0.0f);
        m.SetFloat("_PapillaeStrength", 0.5f);
        m.SetFloat("_CamoBlend", 0.0f);
    }

    void ApplyPresetSandyCamo(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Sandy Camo");
        m.SetColor("_LeucophoreColor", new Color(0.9f, 0.85f, 0.7f));
        m.SetColor("_TargetColor", new Color(0.76f, 0.7f, 0.55f));
        m.SetFloat("_RedExpansion", 0.1f);
        m.SetFloat("_YellowExpansion", 0.6f);
        m.SetFloat("_BrownExpansion", 0.4f);
        m.SetFloat("_BlackExpansion", 0.05f);
        m.SetFloat("_WaveAmplitude", 0.15f);
        m.SetFloat("_WaveSpeed", 0.3f);
        m.SetFloat("_IridophoreStrength", 0.05f);
        m.SetFloat("_PapillaeStrength", 0.8f);
        m.SetFloat("_PapillaeScale", 20.0f);
        m.SetFloat("_CamoBlend", 0.4f);
    }

    void ApplyPresetDeepOcean(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Deep Ocean");
        m.SetColor("_LeucophoreColor", new Color(0.15f, 0.2f, 0.35f));
        m.SetFloat("_RedExpansion", 0.7f);
        m.SetFloat("_YellowExpansion", 0.0f);
        m.SetFloat("_BrownExpansion", 0.8f);
        m.SetFloat("_BlackExpansion", 0.6f);
        m.SetFloat("_WaveAmplitude", 0.3f);
        m.SetFloat("_WaveSpeed", 1.0f);
        m.SetFloat("_IridophoreStrength", 0.6f);
        m.SetFloat("_IridophoreHueShift", 0.55f);
        m.SetFloat("_IridophoreHueRange", 0.15f);
        m.SetFloat("_PapillaeStrength", 0.0f);
        m.SetFloat("_CamoBlend", 0.0f);
    }

    void ApplyPresetCoralReef(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Coral Reef");
        m.SetColor("_LeucophoreColor", new Color(0.8f, 0.75f, 0.7f));
        m.SetColor("_TargetColor", new Color(0.6f, 0.45f, 0.35f));
        m.SetFloat("_RedExpansion", 0.5f);
        m.SetFloat("_YellowExpansion", 0.4f);
        m.SetFloat("_BrownExpansion", 0.5f);
        m.SetFloat("_BlackExpansion", 0.3f);
        m.SetFloat("_WaveAmplitude", 0.25f);
        m.SetFloat("_WaveSpeed", 1.5f);
        m.SetFloat("_IridophoreStrength", 0.35f);
        m.SetFloat("_IridophoreHueShift", 0.3f);
        m.SetFloat("_PapillaeStrength", 1.2f);
        m.SetFloat("_PapillaeScale", 12.0f);
        m.SetFloat("_PapillaeSharpness", 3.0f);
        m.SetFloat("_CamoBlend", 0.3f);
    }

    void ApplyPresetThreat(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Threat Display");
        m.SetFloat("_RedExpansion", 1.0f);
        m.SetFloat("_YellowExpansion", 0.7f);
        m.SetFloat("_BrownExpansion", 0.0f);
        m.SetFloat("_BlackExpansion", 0.9f);
        m.SetFloat("_WaveAmplitude", 1.0f);
        m.SetFloat("_WaveSpeed", 8.0f);
        m.SetFloat("_WaveFrequency", 6.0f);
        m.SetFloat("_MultiWave", 1.0f);
        m.SetFloat("_IridophoreStrength", 0.7f);
        m.SetFloat("_IridophoreSpeed", 3.0f);
        m.SetFloat("_PulseSpeed", 6.0f);
        m.SetFloat("_PulseAmplitude", 0.15f);
        m.SetFloat("_PapillaeStrength", 1.5f);
        m.SetFloat("_PapillaeSharpness", 4.0f);
        m.SetFloat("_CamoBlend", 0.0f);
    }

    void ApplyPresetPassingCloud(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Passing Cloud");
        // "Passing cloud" is the signature dark wave that octopuses send across their body
        m.SetFloat("_RedExpansion", 0.15f);
        m.SetFloat("_YellowExpansion", 0.1f);
        m.SetFloat("_BrownExpansion", 0.2f);
        m.SetFloat("_BlackExpansion", 0.15f);
        m.SetFloat("_BlackWaveInfluence", 1.0f);
        m.SetFloat("_BrownWaveInfluence", 0.9f);
        m.SetFloat("_WaveAmplitude", 0.9f);
        m.SetFloat("_WaveSpeed", 3.0f);
        m.SetFloat("_WaveFrequency", 1.5f);
        m.SetFloat("_WaveDecay", 0.3f);
        m.SetFloat("_WaveRadial", 0.0f);
        m.SetVector("_WaveDirection", new Vector4(1, 0.3f, 0, 0));
        m.SetFloat("_IridophoreStrength", 0.1f);
        m.SetFloat("_PulseAmplitude", 0.02f);
        m.SetFloat("_PapillaeStrength", 0.0f);
        m.SetFloat("_CamoBlend", 0.0f);
    }

    void ApplyPresetReset(Material m)
    {
        Undo.RecordObject(m, "Octopus Camo Preset: Reset");
        m.SetColor("_LeucophoreColor", new Color(0.85f, 0.82f, 0.78f));
        m.SetFloat("_LeucophoreAmbientBlend", 0.5f);
        m.SetFloat("_LeucophoreIntensity", 1.0f);
        m.SetFloat("_IridophoreStrength", 0.3f);
        m.SetFloat("_IridophoreScale", 5.0f);
        m.SetFloat("_IridophoreSpeed", 1.0f);
        m.SetFloat("_IridophoreHueShift", 0.0f);
        m.SetFloat("_IridophoreHueRange", 0.3f);
        m.SetFloat("_IridophoreFresnelPower", 3.0f);
        m.SetFloat("_ChromatophoreScale", 30.0f);
        m.SetFloat("_ChromatophoreEdge", 0.1f);
        m.SetColor("_RedPigment", new Color(0.7f, 0.12f, 0.08f));
        m.SetFloat("_RedExpansion", 0.5f);
        m.SetFloat("_RedWaveInfluence", 0.8f);
        m.SetColor("_YellowPigment", new Color(0.85f, 0.75f, 0.15f));
        m.SetFloat("_YellowExpansion", 0.3f);
        m.SetFloat("_YellowWaveInfluence", 0.6f);
        m.SetColor("_BrownPigment", new Color(0.4f, 0.25f, 0.1f));
        m.SetFloat("_BrownExpansion", 0.6f);
        m.SetFloat("_BrownWaveInfluence", 0.7f);
        m.SetColor("_BlackPigment", new Color(0.05f, 0.03f, 0.02f));
        m.SetFloat("_BlackExpansion", 0.2f);
        m.SetFloat("_BlackWaveInfluence", 0.9f);
        m.SetVector("_WaveOrigin", new Vector4(0.5f, 0.5f, 0, 0));
        m.SetFloat("_WaveSpeed", 2.0f);
        m.SetFloat("_WaveFrequency", 3.0f);
        m.SetFloat("_WaveAmplitude", 0.5f);
        m.SetFloat("_WaveDecay", 1.0f);
        m.SetFloat("_WaveRadial", 1.0f);
        m.SetFloat("_MultiWave", 0.0f);
        m.SetColor("_TargetColor", new Color(0.4f, 0.45f, 0.35f));
        m.SetFloat("_CamoBlend", 0.0f);
        m.SetFloat("_TransitionSharpness", 0.3f);
        m.SetFloat("_PapillaeStrength", 0.0f);
        m.SetFloat("_PapillaeScale", 15.0f);
        m.SetFloat("_PapillaeSharpness", 2.0f);
        m.SetFloat("_GlobalSpeed", 1.0f);
        m.SetFloat("_PulseSpeed", 1.5f);
        m.SetFloat("_PulseAmplitude", 0.05f);
        m.SetFloat("_BreathingSpeed", 0.5f);
        m.SetFloat("_BreathingAmount", 0.03f);
        m.SetFloat("_BaseTextureStrength", 1.0f);
        m.SetFloat("_LeucophoreOpacity", 0.5f);
        m.SetFloat("_IridophoreOpacity", 1.0f);
        m.SetFloat("_ChromatophoreOpacity", 0.7f);
        m.SetFloat("_CamoTransitionOpacity", 1.0f);
        m.SetFloat("_BlendMode", 0f);
        m.SetFloat("_Glossiness", 0.6f);
        m.SetFloat("_Metallic", 0.0f);
        m.SetFloat("_OcclusionStrength", 0.5f);
    }
}