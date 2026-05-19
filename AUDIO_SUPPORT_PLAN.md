# JT1078 Audio Support Implementation Plan

This document outlines the strategy for adding comprehensive audio support to the JT1078 project, covering the core protocol, FLV, fMP4, and HLS (TS) outputs.

## 1. Core Transcoding Layer Refinement

The current `AudioCodecFactory` is incomplete and doesn't provide a unified way to encode to streaming-friendly formats like AAC.

- **Task 1.1: Standardize Audio Transcoding Interface**
  - Create a unified `IAudioEncoder` interface.
  - Implement `AACEncoder` which wraps `FaacEncoder` or a managed alternative.
  - Ensure support for common sample rates (8000Hz, 16000Hz) and bit depths.

- **Task 1.2: Complete `AudioCodecFactory`**
  - Integrate `AACEncoder` into `AudioCodecFactory.Encode`.
  - Ensure the pipeline: `JT1078 (G711/ADPCM) -> PCM -> AAC` works seamlessly.
  - Add support for direct AAC pass-through if the source is already AAC.

## 2. FLV Audio Support

FLV audio support is partially implemented but disabled.

- **Task 2.1: Re-enable `FlvEncoder.EncoderAudioTag`**
  - Remove the `[Obsolete]` attribute.
  - Fix the logic to correctly use the updated `AudioCodecFactory`.
  - Handle timestamp synchronization between audio and video tags.

- **Task 2.2: AAC Sequence Header (AudioSpecificConfig)**
  - Ensure `EncoderFirstAudioTag` correctly generates the `AudioSpecificConfig` required for AAC in FLV.
  - Use `AudioSpecificConfig` class in `JT1078.Flv.Metadata`.

## 3. fMP4 Audio Support

fMP4 requires adding a second track for audio.

- **Task 3.1: Implement Audio Boxes**
  - Ensure `mp4a` (AudioSampleEntry) is fully implemented.
  - Implement `esds` (Elementary Stream Descriptor) box for AAC configuration.

- **Task 3.2: Update `FMp4Encoder`**
  - Modify `FMp4Encoder` to support multiple tracks (Trak).
  - Add an `AddAudioTrack` method.
  - Handle `moof` and `mdat` for audio fragments.

## 4. HLS (MPEG-TS) Audio Support

HLS currently only supports video.

- **Task 4.1: Update PMT (Program Map Table)**
  - Update `TSEncoder.CreatePMT` to include an audio component.
  - Use `StreamType.aac` (0x0F) for AAC audio.

- **Task 4.2: Implement Audio PES Packaging**
  - Create `CreateAudioPES` in `TSEncoder`.
  - Wrap AAC frames in ADTS headers if necessary for TS.
  - Ensure proper PID (e.g., 257) and continuity counters for the audio stream.

## 5. Testing and Validation

- **Task 5.1: Unit Tests**
  - Add tests in `JT1078.Flv.Test/Audio` for G711 to FLV AAC conversion.
  - Create `JT1078.FMp4.Test` cases for audio tracks.
  - Create `JT1078.Hls.Test` cases for TS audio.

- **Task 5.2: Integration Tests**
  - Use tools like `FFmpeg` to verify the generated FLV, fMP4, and HLS streams have playable audio.
  - Verify AV sync in long-running streams.

## 6. Implementation Order Recommendation

1.  **Transcoding Layer**: Foundation for all outputs.
2.  **FLV Support**: Easiest to verify and has existing partial implementation.
3.  **HLS Support**: High priority for web streaming.
4.  **fMP4 Support**: Complementary to HLS/DASH.
