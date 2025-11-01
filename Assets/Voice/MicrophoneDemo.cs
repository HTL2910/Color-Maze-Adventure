using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Whisper.Samples
{
    public class MicrophoneDemo : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("Ball Movement Control")]
        [Tooltip("Reference to the BallMove component")]
        public BallMove ballMove;

        [Header("VAD Settings")]
        public float minRecordingDuration = 1.0f;
        public float delayBeforeNextRecord = 0.05f; // reduced default
       
        [Header("Immediate command settings")]
        [Tooltip("Minimum time (s) between handling immediate movement commands from segments")]
        public float commandCooldown = 0.25f; // debounce to avoid duplicate triggers

        [Header("UI")] 
        public TextMeshProUGUI outputText;
        private string _buffer;
        private float _recordingStartTime;
        private bool _isProcessing;
        private bool _isReadyForNextRecord;

        // Performance tracking
        private int _totalRecordings = 0;
        private float _totalProcessingTime = 0f;
        private float _totalAudioLength = 0f;

        // App lifecycle tracking
        private bool _isAppPaused = false;
        private bool _isAppFocused = true;

        // command debounce
        private float _lastCommandTime = -10f;

        private void Awake()
        {
            whisper.OnNewSegment += OnNewSegment;
            microphoneRecord.OnRecordStop += OnRecordStop;
            microphoneRecord.OnVadChanged += OnVadChanged;

            microphoneRecord.useVad = true;
            microphoneRecord.vadStop = false;

            microphoneRecord.vadThd = 0.5f;
            microphoneRecord.vadFreqThd = 50.0f;
            microphoneRecord.vadUpdateRateSec = 0.05f;

            var devices = Microphone.devices;
            UnityEngine.Debug.Log($"Available microphones: {string.Join(", ", devices)}");
            if (devices.Length == 0)
            {
                UnityEngine.Debug.LogError("No microphone devices found!");
                return;
            }

            if (ballMove == null)
            {
                ballMove = FindObjectOfType<BallMove>();
                if (ballMove == null)
                {
                    UnityEngine.Debug.LogError("BallMove component not found! Please assign it in the inspector.");
                    return;
                }
            }

            _isReadyForNextRecord = true;

            // Start microphone immediately to enable VAD
            microphoneRecord.StartRecord();
            UnityEngine.Debug.Log("Microphone started for VAD detection");
        }

        private void Update()
        {
            // (keep empty or use for UI updates)
        }

        private void UpdatePerformanceDisplay()
        {
            var memoryUsage = System.GC.GetTotalMemory(false) / (1024f * 1024f); // MB
            var avgProcessingTime = _totalRecordings > 0 ? _totalProcessingTime / _totalRecordings : 0f;
            var avgAudioLength = _totalRecordings > 0 ? _totalAudioLength / _totalRecordings : 0f;
            var processingRate = _totalProcessingTime > 0 ? _totalAudioLength / _totalProcessingTime : 0f;
            // you can show values in UI if needed
        }

        private void OnVadChanged(bool isVoiceDetected)
        {
            UnityEngine.Debug.Log($"VAD Changed: {isVoiceDetected}, IsRecording: {microphoneRecord.IsRecording}, IsProcessing: {_isProcessing}, IsReady: {_isReadyForNextRecord}");

            if (_isAppPaused || !_isAppFocused)
            {
                UnityEngine.Debug.Log($"VAD ignored - App paused: {_isAppPaused}, App focused: {_isAppFocused}");
                return;
            }

            if (_isProcessing) return;

            if (isVoiceDetected && !microphoneRecord.IsRecording && _isReadyForNextRecord)
            {
                UnityEngine.Debug.Log("Voice detected - starting recording");
                StartRecording();
            }
            else if (!isVoiceDetected && microphoneRecord.IsRecording)
            {
                var recordingDuration = Time.time - _recordingStartTime;
                UnityEngine.Debug.Log($"Silence detected - recording duration: {recordingDuration:F2}s");
                if (recordingDuration >= minRecordingDuration)
                {
                    UnityEngine.Debug.Log("Stopping recording due to silence");
                    StopRecording();
                }
            }
        }

        private void StartRecording()
        {
            if (microphoneRecord.IsRecording) return;

            _recordingStartTime = Time.time;
            microphoneRecord.StartRecord();
        }

        private void StopRecording()
        {
            if (!microphoneRecord.IsRecording) return;

            _isProcessing = true;
            microphoneRecord.StopRecord();
        }

        // Called on stream segments (real-time partial results)
        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            // Append to the streaming buffer
            _buffer += segment.Text;
            outputText.text = _buffer + "...";

            // Handle immediate commands from each segment (debounced)
            TryHandleImmediateCommand(segment.Text);
        }

        // Try to process small segment instantly (for low-latency control)
        private void TryHandleImmediateCommand(string segmentText)
        {
            if (string.IsNullOrWhiteSpace(segmentText)) return;

            // Debounce so we don't call movement many times per second on streaming tokens
            if (Time.time - _lastCommandTime < commandCooldown) return;

            // Check for keywords and trigger action immediately
            string lower = segmentText.ToLower();
            if (ContainsMovementKeyword(lower))
            {
                _lastCommandTime = Time.time;
                ProcessWhisperResult(segmentText); // immediate action
            }
        }

        // Helper to detect if a segment contains movement keywords
        private bool ContainsMovementKeyword(string lowerText)
        {
            if (string.IsNullOrWhiteSpace(lowerText)) return false;
            return lowerText.Contains("up") || lowerText.Contains("lên") || lowerText.Contains("trên") ||
                   lowerText.Contains("down") || lowerText.Contains("xuống") || lowerText.Contains("dưới") ||
                   lowerText.Contains("left") || lowerText.Contains("trái") ||
                   lowerText.Contains("right") || lowerText.Contains("phải");
        }

        private void ProcessWhisperResult(string text)
        {
            if (string.IsNullOrEmpty(text)) return;

            string lowerText = text.ToLower();

            if (lowerText.Contains("up") || lowerText.Contains("lên") || lowerText.Contains("trên"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveUp();
                    UnityEngine.Debug.Log("UP command detected - ball moving up");
                }
            }
            else if (lowerText.Contains("down") || lowerText.Contains("xuống") || lowerText.Contains("dưới"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveDown();
                    UnityEngine.Debug.Log("DOWN command detected - ball moving down");
                }
            }
            else if (lowerText.Contains("left") || lowerText.Contains("trái"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveLeft();
                    UnityEngine.Debug.Log("LEFT command detected - ball moving left");
                }
            }
            else if (lowerText.Contains("right") || lowerText.Contains("phải"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveRight();
                    UnityEngine.Debug.Log("RIGHT command detected - ball moving right");
                }
            }
            else
            {
                UnityEngine.Debug.Log($"No movement command detected in: '{text}'");
            }
        }

        // Full recording finished — still process final transcription result
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            // Only process if we have meaningful audio
            if (recordedAudio.Data.Length == 0)
            {
                _isProcessing = false;
                _isReadyForNextRecord = true;

                // Restart microphone for VAD detection if not already recording
                if (!microphoneRecord.IsRecording)
                {
                    microphoneRecord.StartRecord();
                    UnityEngine.Debug.Log("Restarted microphone after empty recording");
                }

                return;
            }

            var sw = new Stopwatch();
            sw.Start();

            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);

            var time = sw.ElapsedMilliseconds;
            var rate = recordedAudio.Length / (time * 0.001f);

            // Update performance statistics
            _totalRecordings++;
            _totalProcessingTime += time;
            _totalAudioLength += recordedAudio.Length;

            if (res != null && outputText != null)
            {
                var text = res.Result;
                if (printLanguage)
                    text += $"\n\nLanguage: {res.Language}";

                outputText.text = text;

                // Process the result for ball movement (final)
                ProcessWhisperResult(res.Result);
            }

            // Small delay before next recording, keep very short or zero for responsiveness
            if (delayBeforeNextRecord > 0f)
                await System.Threading.Tasks.Task.Delay((int)(delayBeforeNextRecord * 1000));

            _isProcessing = false;
            _isReadyForNextRecord = true;

            // Restart microphone for VAD detection if not already running
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                UnityEngine.Debug.Log("Restarted microphone after processing");
            }
        }

        // App lifecycle management
        private void OnApplicationPause(bool pauseStatus)
        {
            _isAppPaused = pauseStatus;
            UnityEngine.Debug.Log($"App paused: {pauseStatus}");

            if (pauseStatus)
            {
                if (microphoneRecord.IsRecording)
                {
                    UnityEngine.Debug.Log("Stopping recording due to app pause");
                    microphoneRecord.StopRecord();
                }
            }
            else
            {
                if (_isAppFocused && !_isProcessing && _isReadyForNextRecord)
                {
                    UnityEngine.Debug.Log("Restarting VAD after app resume");
                    microphoneRecord.StartRecord();
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _isAppFocused = hasFocus;
            UnityEngine.Debug.Log($"App focused: {hasFocus}");

            if (!hasFocus)
            {
                if (microphoneRecord.IsRecording)
                {
                    UnityEngine.Debug.Log("Stopping recording due to app focus loss");
                    microphoneRecord.StopRecord();
                }
            }
            else
            {
                if (!_isAppPaused && !_isProcessing && _isReadyForNextRecord)
                {
                    UnityEngine.Debug.Log("Restarting VAD after app focus gain");
                    microphoneRecord.StartRecord();
                }
            }
        }

        private void OnDestroy()
        {
            UnityEngine.Debug.Log("MicrophoneDemo being destroyed - cleaning up resources");

            if (whisper != null)
            {
                whisper.OnNewSegment -= OnNewSegment;
            }

            if (microphoneRecord != null)
            {
                microphoneRecord.OnRecordStop -= OnRecordStop;
                microphoneRecord.OnVadChanged -= OnVadChanged;

                if (microphoneRecord.IsRecording)
                {
                    microphoneRecord.StopRecord();
                }
            }
        }
    }
}
