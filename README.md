# Octopus Camouflage Shader

A biologically-inspired camouflage shader for Unity's Built-in Render Pipeline that models how real octopus skin works.

## How It Works

Real octopus skin has three layers of specialized cells that work together to produce rapid color change. This shader replicates each layer:

**Leucophores (Bottom Layer)** — White scattering cells that reflect ambient light wavelengths back, acting as a neutral backdrop. Controls: base color, ambient blend, intensity.

**Iridophores (Middle Layer)** — Stacks of reflective plates that produce iridescent blues, greens, and golds depending on viewing angle. Driven by fresnel (view-angle) calculations with adjustable hue range, shimmer speed, and scale.

**Chromatophores (Top Layer)** — Thousands of tiny elastic pigment sacs that expand and contract under neural control. Four independent pigment channels (red, yellow, brown, black) are each generated on their own offset Voronoi cell grid, giving each channel a unique organic pattern. Each channel has independent expansion, wave influence, and pigment color.

**Wave Propagation** — Octopus neurons fire in cascading sequences, creating visible ripples of color change across the skin. The shader supports radial and directional waves, dual-wave sources, per-channel wave sensitivity, and adjustable speed/frequency/decay.

**Papillae** — Muscle bundles that raise or smooth the skin to match surrounding textures. Implemented as vertex displacement plus normal perturbation using fractal noise.

## Installation

1. Copy `OctopusCamouflage.shader` anywhere into your Unity project's `Assets` folder.
2. Copy `OctopusCamouflageInspector.cs` into an `Editor` folder (e.g. `Assets/Editor/`).
3. Optionally copy `OctopusCamouflageController.cs` anywhere into `Assets` if you want runtime scripting control.

## Quick Start

1. Create a new Material in Unity.
2. Set the shader to **Custom > OctopusCamouflage**.
3. Assign your character's base texture to the **Base Texture** slot.
4. Click one of the **Presets** buttons in the inspector to see it in action.
5. Tweak from there.

## Inspector Sections

### Presets
One-click configurations to use as starting points:

| Preset | Description |
|---|---|
| **Resting** | Calm state, minimal chromatophore activity, subtle breathing pulse |
| **Alert** | Heightened activity, fast waves, raised papillae |
| **Sandy Camo** | Warm yellows/browns blending toward a sand target color |
| **Deep Ocean** | Dark leucophore base, strong iridescence, no yellows |
| **Coral Reef** | Mixed pigments with heavy papillae for rough texture matching |
| **Threat Display** | Maximum expansion, fast dual waves, intense iridescence |
| **Passing Cloud** | The signature dark wave that octopuses send across their body |
| **Reset All** | Returns every parameter to its default value |

### Layer Blending
Controls how much each biological layer covers your base texture:

- **Base Texture Visibility** — How strongly the original texture comes through (1.0 = full).
- **Leucophore / Iridophore / Chromatophore Opacity** — Per-layer opacity. Lower chromatophore opacity to let base texture details show through the pigment cells.
- **Camo Transition Opacity** — How opaque the camouflage target color blend is.
- **Chromatophore Blend Mode**:
  - **Replace** — Pigment fully covers the base (original behavior).
  - **Multiply** — Pigment tints the base texture, preserving detail in light areas.
  - **Overlay** — Preserves base texture contrast while adding pigment color. Best for seeing details and camo simultaneously.

### Chromatophore Layer
- **Cell Scale** — Size of the Voronoi cells (higher = smaller cells).
- **Cell Edge Sharpness** — How soft/hard the edge of each pigment sac is.
- Each pigment channel (Red, Yellow, Brown, Black) has:
  - **Pigment Color** — The color of the pigment. Alpha controls max opacity of that channel.
  - **Expansion** — 0 = fully contracted (invisible), 1 = fully expanded.
  - **Wave Influence** — How much this channel responds to the neural wave signal.

### Wave Propagation
- **Wave Origin** — Center point of radial waves in UV space (0-1).
- **Speed / Frequency / Amplitude** — How fast, dense, and strong the wave is.
- **Decay** — How quickly the wave fades with distance from origin.
- **Radial vs Directional** — Toggle between expanding rings and sweeping planes.
- **Secondary Wave** — Enable a second independent wave source.

### Camouflage Transition
- **Target Color** — The color the octopus is trying to match.
- **Camo Blend** — 0 = no camouflage, 1 = fully transitioned. Animate this for the full effect.
- **Camo Pattern Noise** — Assign a grayscale noise texture for organic transition edges.
- **Pattern Scale / Contrast / Sharpness** — Shape the transition pattern.

### Papillae
- **Bump Strength** — How much the mesh vertices displace (0 = smooth skin).
- **Scale** — Size of the bump pattern.
- **Sharpness** — Higher = more peaked bumps, lower = rounder bumps.

### Animation
- **Global Speed** — Master speed multiplier for all animation.
- **Pulse Speed / Amount** — Chromatophores rhythmically pulse like real octopus cells.
- **Breathing Speed / Amount** — Slow oscillation simulating the animal breathing.

## Runtime Controller

Attach `OctopusCamouflageController` to any GameObject with a Renderer using this shader. It provides:

```csharp
// Smooth transition to a built-in state
camo.TransitionToState(OctopusCamouflageController.StateSandyCamo, 2f);

// Instant snap
camo.SetStateImmediate(OctopusCamouflageController.StateAlert);

// Signature dark wave across the body
camo.TriggerPassingCloud(Vector2.right, speed: 3f, duration: 3f);

// Brief flash when startled or hit
camo.TriggerStartleFlash(flashDuration: 0.3f, recoverDuration: 1.5f);

// Set wave origin to a hit point (UV space)
camo.SetWaveOrigin(new Vector2(0.3f, 0.7f));

// Directly control a single pigment channel
camo.SetPigmentExpansion(
    OctopusCamouflageController.PigmentChannel.Red, 0.8f);

// Blend toward a terrain/environment color
camo.BlendToCamoColor(terrainColor, blend: 0.6f, duration: 1.5f);
```

### Custom States

Create your own `CamoState` structs for game-specific scenarios:

```csharp
var hidingState = new OctopusCamouflageController.CamoState
{
    redExpansion = 0.1f,
    yellowExpansion = 0.5f,
    brownExpansion = 0.6f,
    blackExpansion = 0.05f,
    waveAmplitude = 0.1f,
    waveSpeed = 0.3f,
    iridophoreStrength = 0.05f,
    papillaeStrength = 1.0f,
    camoBlend = 0.5f,
    chromatophoreOpacity = 0.6f,
    baseTextureStrength = 1.0f,
    targetColor = someEnvironmentColor,
    leucophoreColor = new Color(0.8f, 0.75f, 0.65f)
};

camo.TransitionToState(hidingState, 2.5f);
```

The `transitionCurve` field on the controller is a public `AnimationCurve` you can edit in the inspector to shape how transitions ease in/out.

## Requirements

- Unity (Built-in Render Pipeline)
- Shader Target 3.5+

## Tips

- For characters where you want the base skin/texture visible through the camo effect, set **Chromatophore Opacity** to 0.4–0.6 and use **Overlay** blend mode.
- Assign a Perlin or Simplex noise texture to the **Camo Pattern Noise** slot for organic-looking camouflage transition edges.
- The **Passing Cloud** preset recreates the dark traveling wave real octopuses use for communication — great for signaling mood in gameplay.
- All shader properties can be animated via Unity's Animation window or Timeline for cutscenes.
- Wave Origin is in UV space (0–1), so you can convert a raycast hit's `textureCoord` directly into a wave origin point.

## License

MIT
