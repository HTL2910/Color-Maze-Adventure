using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Whisper.Samples
{
    /// <summary>
    /// Voice-controlled ball movement using Whisper VAD.
    /// Automatically records when voice is detected, transcribes, then controls ball movement.
    /// </summary>
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
        [Tooltip("Minimum recording duration in seconds (prevents very short recordings)")]
        public float minRecordingDuration = 1.0f;
        [Tooltip("Delay before starting to listen again after transcription")]
        public float delayBeforeNextRecord = 0.5f;

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

        private void Awake()
        {
            whisper.OnNewSegment += OnNewSegment;
            microphoneRecord.OnRecordStop += OnRecordStop;
            microphoneRecord.OnVadChanged += OnVadChanged;

            // Enable VAD features and disable manual stop
            microphoneRecord.useVad = true;
            microphoneRecord.vadStop = false;
            
            // Adjust VAD settings for better sensitivity
            microphoneRecord.vadThd = 0.5f; // Lower threshold for more sensitivity
            microphoneRecord.vadFreqThd = 50.0f; // Lower frequency threshold
            microphoneRecord.vadUpdateRateSec = 0.05f; // Update more frequently
            
            // Check microphone devices
            var devices = Microphone.devices;
            UnityEngine.Debug.Log($"Available microphones: {string.Join(", ", devices)}");
            if (devices.Length == 0)
            {
                UnityEngine.Debug.LogError("No microphone devices found!");
               
                return;
            }
            
            // Check if BallMove component is assigned
            if (ballMove == null)
            {
                ballMove = FindObjectOfType<BallMove>();
                if (ballMove == null)
                {
                    UnityEngine.Debug.LogError("BallMove component not found! Please assign it in the inspector.");
                   
                    return;
                }
            }
            
            // Start listening immediately
            _isReadyForNextRecord = true;
           
            
            // Debug: Start microphone immediately to enable VAD
            microphoneRecord.StartRecord();
            UnityEngine.Debug.Log("Microphone started for VAD detection");
        }

        private void Update()
        {
           
        }

        private void UpdatePerformanceDisplay()
        {
            var memoryUsage = System.GC.GetTotalMemory(false) / (1024f * 1024f); // MB
            var avgProcessingTime = _totalRecordings > 0 ? _totalProcessingTime / _totalRecordings : 0f;
            var avgAudioLength = _totalRecordings > 0 ? _totalAudioLength / _totalRecordings : 0f;
            var processingRate = _totalAudioLength > 0 ? _totalAudioLength / _totalProcessingTime : 0f;
        }

        private void OnVadChanged(bool isVoiceDetected)
        {
            UnityEngine.Debug.Log($"VAD Changed: {isVoiceDetected}, IsRecording: {microphoneRecord.IsRecording}, IsProcessing: {_isProcessing}, IsReady: {_isReadyForNextRecord}");
            
            // Don't process VAD if app is paused or not focused
            if (_isAppPaused || !_isAppFocused)
            {
                UnityEngine.Debug.Log($"VAD ignored - App paused: {_isAppPaused}, App focused: {_isAppFocused}");
                return;
            }
            
            // Don't process if we're currently processing a previous recording
            if (_isProcessing) return;

            if (isVoiceDetected && !microphoneRecord.IsRecording && _isReadyForNextRecord)
            {
                // Voice detected - start recording
                UnityEngine.Debug.Log("Voice detected - starting recording");
                StartRecording();
            }
            else if (!isVoiceDetected && microphoneRecord.IsRecording)
            {
                // Silence detected - check if we should stop recording
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

    
        
        private void ProcessWhisperResult(string text)
        {
            // Convert to lowercase for case-insensitive matching
            string lowerText = text.ToLower();
            
            // Check for movement commands and control ball
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
        
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            // Only process if we have meaningful audio
            if (recordedAudio.Data.Length == 0)
            {
                _isProcessing = false;
                _isReadyForNextRecord = true;
               
                // Restart microphone for VAD detection
                microphoneRecord.StartRecord();
                UnityEngine.Debug.Log("Restarted microphone after empty recording");
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
                
                // Process the result for ball movement
                ProcessWhisperResult(res.Result);
            }

            // Wait a bit before listening again
            await System.Threading.Tasks.Task.Delay((int)(delayBeforeNextRecord * 1000));
            
            _isProcessing = false;
            _isReadyForNextRecord = true;
           
            
            // Restart microphone for VAD detection
            microphoneRecord.StartRecord();
            UnityEngine.Debug.Log("Restarted microphone after processing");
        }

        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            _buffer += segment.Text;
            outputText.text = _buffer + "...";
        }
        
        // App lifecycle management
        private void OnApplicationPause(bool pauseStatus)
        {
            _isAppPaused = pauseStatus;
            UnityEngine.Debug.Log($"App paused: {pauseStatus}");
            
            if (pauseStatus)
            {
                // App is being paused - stop VAD and recording
                if (microphoneRecord.IsRecording)
                {
                    UnityEngine.Debug.Log("Stopping recording due to app pause");
                    microphoneRecord.StopRecord();
                }
               
            }
            else
            {
                // App is being resumed - restart VAD if conditions are met
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
                // App lost focus - stop VAD and recording
                if (microphoneRecord.IsRecording)
                {
                    UnityEngine.Debug.Log("Stopping recording due to app focus loss");
                    microphoneRecord.StopRecord();
                }
            }
            else
            {
                // App gained focus - restart VAD if conditions are met
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
            
            // Unsubscribe from events to prevent memory leaks
            if (whisper != null)
            {
                whisper.OnNewSegment -= OnNewSegment;
            }
            
            if (microphoneRecord != null)
            {
                microphoneRecord.OnRecordStop -= OnRecordStop;
                microphoneRecord.OnVadChanged -= OnVadChanged;
                
                // Stop recording if active
                if (microphoneRecord.IsRecording)
                {
                    microphoneRecord.StopRecord();
                }
            }
        }
    }
}
