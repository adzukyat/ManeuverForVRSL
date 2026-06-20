# Maneuver For VRSL

Maneuver For VRSL includes a VRChat-compatible Stage Light Maneuver (SLM) authoring layer and plays baked lighting back through VR Stage Lighting (VRSL) in VRChat worlds.

The intended workflow is:

1. Use SLM Timeline tracks for authoring and Unity Editor preview.
2. Keep Unity Timeline `ActivationTrack` and `AnimationTrack` for non-lighting show direction.
3. Bake only the SLM lighting tracks into compact VRSL/Udon runtime data before upload.
4. Let the baked runtime player follow `PlayableDirector.time` so lighting stays synced with the rest of the Timeline.

## Setup

### Installation Prerequisites

This repository is a UPM package at the repository root. Add it to an existing Unity project with the normal Git URL workflow; do not add a `?path=` suffix.

Install the external runtime dependencies before adding this package, or use a VCC world project that already resolves them:

- VR Stage Lighting `com.acchosen.vr-stage-lighting` `2.8.4`
- VRChat SDK Base `com.vrchat.base` `3.10.2`
- VRChat SDK Worlds `com.vrchat.worlds` `3.10.2`

Do not install the external `jp.iridescent.stagelightmaneuver` package alongside Maneuver For VRSL. This package already includes its SLM authoring layer under `StageLightManeuver/`.

The committed `TestProject~` harness bootstraps the VRC/VRSL/AudioLink packages locally for tests, but a normal user project should resolve those dependencies through VCC or the registries/package sources used by that project.

### 1. Place VRSL Fixtures

Add VRSL DMX Static fixtures to the scene. The initial implementation targets `VRStageLighting_DMX_Static` fixtures, such as:

- `VRSL-DMX-Mover-Spotlight-H-13CH`
- `VRSL-DMX-Mover-WashLight-H-13CH`
- other DMX Static fixtures using `VRStageLighting_DMX_Static`

AudioLink fixtures are treated as VRSL-owned behavior and are not driven by Maneuver For VRSL in the initial version.

### 2. Add the SLM Adapter Components

On each VRSL-controlled fixture object, or on a nearby control object, add:

- `StageLightFixture`
- `MfvVRSLFixtureChannel`

`MfvVRSLFixtureChannel` appears in Unity's Add Component menu as:

`Maneuver For VRSL > MFV VRSL Fixture Channel`

Assign the target `VRStageLighting_DMX_Static` component to `MfvVRSLFixtureChannel.vrslFixture`.

If the field is left empty, the channel tries to find a `VRStageLighting_DMX_Static` in children, but explicit assignment is recommended.

### 3. Group Multiple Fixtures

For multiple fixtures, create an empty GameObject and add:

- `StageLightUniverse`

Then add each `StageLightFixture` to `StageLightUniverse.stageLightFixtures`.

For a single fixture, binding the Timeline track directly to `StageLightFixture` is enough.

## Timeline Authoring

Create or select a GameObject with a `PlayableDirector`, then assign a Timeline asset.

Add an SLM `StageLightTimelineTrack` and bind it to:

- `StageLightFixture` for one fixture
- `StageLightUniverse` for multiple fixtures

You can also use Unity Timeline tracks alongside the SLM lighting track:

- `ActivationTrack`
- `AnimationTrack`

These standard Timeline tracks are kept as-is for upload.

## Supported SLM Properties

The initial version supports these lighting controls:

- `Clock`
- `StageLight Order`
- `Dimmer`
- `Light Color`
- `Light`
- `Flicker`
- `Pan`
- `Tilt`
- `Manual Pan Tilt`
- `Manual Light Array`
- `Manual Color Array`
- `VRSL Gobo`

`VRSL Gobo` is provided by this package as `MfvVRSLGoboProperty`. It controls VRSL's built-in gobo index from `1` to `8`.

## Editor Preview

Scrub or play the Timeline in Unity.

During preview, `MfvVRSLFixtureChannel` evaluates the SLM cue data and writes directly to the assigned VRSL fixture:

- `enableDMXChannels = false`
- `enableStrobe = false`
- `panOffsetBlueGreen`
- `tiltOffsetBlue`
- `globalIntensity`
- `lightColorTint`
- `coneWidth`
- `coneLength`
- `selectGOBO`

The channel calls `_UpdateInstancedProperties()` once per fixture update.

## Baking For Upload

Before uploading the world, select the GameObject with the target `PlayableDirector` and run:

`ManeuverForVRSL > Bake Selected Director`

The baker will:

- sample SLM lighting at 120Hz internally
- simplify continuous curves with tolerance-based key reduction
- save a `MfvBakedShowAsset`
- create an upload Timeline variant with SLM tracks removed
- keep `ActivationTrack` and `AnimationTrack`
- create or update a child `MfvVRSLTimelinePlayer`
- copy flattened runtime arrays into the player

The original Timeline and scene authoring data are not intentionally modified by the bake process.

## Runtime Playback

In VRChat, `MfvVRSLTimelinePlayer` reads `PlayableDirector.time` and drives the baked VRSL fixture values.

This keeps baked VRSL lighting synced with the Timeline's remaining `ActivationTrack` and `AnimationTrack` content.

Continuous values are interpolated at runtime. Discrete values such as gobo changes are stored as events.

## Bake Settings

Default bake settings:

- internal sample rate: `120Hz`
- pan/tilt tolerance: `0.5 deg`
- intensity tolerance: `0.005`
- color tolerance: `2/255` per channel
- cone width/length tolerance: `0.01`

The initial implementation prioritizes variable key reduction over fixed-frame storage to avoid large world sizes.

## Current Limitations

The initial version intentionally does not support:

- `ControlTrack`
- `SignalTrack`
- `ReflectionProbe`
- `Decal`
- arbitrary Material channels
- `VLB`
- `LensFlare`
- `Environment`
- AudioLink fixture control

If unsupported Timeline tracks or unsupported SLM channels are found during bake, the bake fails with an explicit error in the Unity Console.

## Validation

This repository stays as a UPM package at the root. `TestProject~` is the committed Unity test harness and references this package with `file:../..`.

### Local CLI

Run the committed test harness with the local Unity Editor:

```sh
scripts/bootstrap-test-project.sh
scripts/test-editmode.sh
```

### Regenerate Stage Light Maneuver

The integrated SLM authoring layer is generated from the upstream repository plus the patch in `patches/stage-light-maneuver-vrchat.patch`.
This rewrites `StageLightManeuver/`:

```sh
scripts/vendor-stage-light-maneuver.sh
```

To try another upstream ref, run for example:

```sh
SLM_UPSTREAM_REF=v1.0.3 scripts/vendor-stage-light-maneuver.sh
```

The test script defaults to `TestProject~` and Unity `2022.3.22f1` installed by Unity Hub. If Unity is installed elsewhere, set `UNITY_EXECUTABLE`:

```sh
UNITY_EXECUTABLE="/Applications/Unity/Hub/Editor/2022.3.22f1/Unity.app/Contents/MacOS/Unity" scripts/test-editmode.sh
```

Results are written to:

- `TestProject~/TestResults~/editmode-results.xml`
- `TestProject~/TestResults~/editor.log`

### Unity Hub

For manual inspection:

1. Run `scripts/bootstrap-test-project.sh` once to download the VRC/VRSL/AudioLink packages used by the harness.
2. Open `TestProject~` in Unity `2022.3.22f1`.
3. Open `Assets/MfvTestFixtures/PreviewSmoke.unity`.
4. Scrub or play `Assets/MfvTestFixtures/PreviewSmoke.playable` and confirm the VRSL fixture fields change.

If the fixture assets need to be rebuilt, use `ManeuverForVRSL > Tests > Regenerate Preview Smoke Fixture`.

### Test Levels

- Level 1 pure unit: key reduction, frame evaluator conversion, deterministic flicker, runtime interpolation, seek-back, and gobo events.
- Level 2 component integration: `MfvVRSLFixtureChannel.EvaluateQue`, `MfvVRSLFixtureApplier.Apply`, and missing `vrslFixture` detection.
- Level 3 real Timeline preview: opens `PreviewSmoke.unity`, evaluates a real SLM `StageLightTimelineTrack`, and verifies channel plus VRSL fixture fields.
- Level 4 bake consistency: bakes the same PreviewSmoke Timeline, validates baked arrays/upload Timeline, and compares baked runtime playback against real preview values.

The same EditMode assembly is used by the local CLI script and Unity Test Runner: `ManeuverForVRSL.EditorTests`.
