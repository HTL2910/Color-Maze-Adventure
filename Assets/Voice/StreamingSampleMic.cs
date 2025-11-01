using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;
using UnityEngine.SceneManagement;

namespace Whisper.Samples
{
    public class StreamingSampleMic : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("Ball Movement Control")]
        [Tooltip("Reference to the BallMove component")]
        public BallMove ballMove;

        [Header("Continuous Recording Settings")]
        [Tooltip("Minimum words to trigger immediate processing")]
        public int minWordsToProcess = 2;
        [Tooltip("Maximum recording duration before restart (seconds)")]
        public float maxRecordingDuration = 30f;
        [Tooltip("Auto restart recording interval (seconds)")]
        public float autoRestartInterval = 10f;
        
        [Header("VAD Settings")]
        public float minRecordingDuration = 1.0f;
        public float delayBeforeNextRecord = 0.05f;
       
        [Header("Immediate command settings")]
        [Tooltip("Minimum time (s) between handling immediate movement commands from segments")]
        public float commandCooldown = 0.25f;

        [Header("UI")] 
        public TextMeshProUGUI outputText;
        
        [Header("Command Display Text")]
        [Tooltip("Text to display UP command")]
        public TextMeshProUGUI upText;
        [Tooltip("Text to display DOWN command")]
        public TextMeshProUGUI downText;
        [Tooltip("Text to display LEFT command")]
        public TextMeshProUGUI leftText;
        [Tooltip("Text to display RIGHT command")]
        public TextMeshProUGUI rightText;
        
        [Header("Command Colors")]
        [Tooltip("Color when command is detected")]
        public Color commandDetectedColor = Color.green;
        [Tooltip("Default color for commands")]
        public Color commandDefaultColor = Color.white;
        [Tooltip("Duration to show detected color")]
        public float commandHighlightDuration = 0.5f;
        
        private WhisperStream _stream;
        private string _buffer;
        private bool _isProcessing;
        private bool _isReadyForNextRecord;
        private bool _isInitialized = false;

        // Performance tracking
        private int _totalRecordings = 0;
        private float _totalProcessingTime = 0f;
        private float _totalAudioLength = 0f;

        // App lifecycle tracking
        private bool _isAppPaused = false;
        private bool _isAppFocused = true;
        private bool _isSceneActive = true;

        // command debounce
        private float _lastCommandTime = -10f;
        
        // Command highlight timers
        private float _upHighlightTimer = 0f;
        private float _downHighlightTimer = 0f;
        private float _leftHighlightTimer = 0f;
        private float _rightHighlightTimer = 0f;
        
        // Continuous recording timers
        private float _recordingStartTime = 0f;
        private float _lastRestartTime = 0f;
        private float _currentRecordingDuration = 0f;

        private async void Start()
        {
            // Subscribe to scene events
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            
            // Configure VAD settings
            ConfigureVAD();
            
            // Create streaming session
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;
            _stream.OnSegmentUpdated += OnSegmentUpdated;
            _stream.OnSegmentFinished += OnSegmentFinished;
            _stream.OnStreamFinished += OnFinished;

            microphoneRecord.OnRecordStop += OnRecordStop;
            microphoneRecord.OnVadChanged += OnVadChanged;

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
            _isInitialized = true;

            // Initialize command texts
            InitializeCommandTexts();

            // Start continuous recording immediately
            StartContinuousRecording();
            UnityEngine.Debug.Log("Continuous streaming started for real-time voice control");
        }

        private void InitializeCommandTexts()
        {
            // Set default texts and colors
            if (upText != null)
            {
                upText.text = "UP";
                upText.color = commandDefaultColor;
            }
            
            if (downText != null)
            {
                downText.text = "DOWN";
                downText.color = commandDefaultColor;
            }
            
            if (leftText != null)
            {
                leftText.text = "LEFT";
                leftText.color = commandDefaultColor;
            }
            
            if (rightText != null)
            {
                rightText.text = "RIGHT";
                rightText.color = commandDefaultColor;
            }
        }

        private void ConfigureVAD()
        {
            if (microphoneRecord == null) return;
            
            // Enable VAD for continuous recording
            microphoneRecord.useVad = true;
            microphoneRecord.vadStop = false;

            // Configure VAD parameters for continuous listening
            microphoneRecord.vadThd = 0.3f; // More sensitive
            microphoneRecord.vadFreqThd = 30.0f; // Lower frequency threshold
            microphoneRecord.vadUpdateRateSec = 0.02f; // Faster updates
            
            // Disable echo to avoid interference
            microphoneRecord.echo = false;
        }

        private void RestartRecording()
        {
            if (_isProcessing)
            {
                StopContinuousRecording();
                
                // Small delay before restart
                Invoke(nameof(StartContinuousRecording), 0.1f);
            }
            else
            {
                // If not processing, start immediately
                StartContinuousRecording();
            }
        }

        private void StartContinuousRecording()
        {
            if (_stream != null && !_isProcessing && _isInitialized)
            {
                try
                {
                    _stream.StartStream();
                    microphoneRecord.StartRecord();
                    _isProcessing = true;
                    _isReadyForNextRecord = false;
                    _recordingStartTime = Time.time;
                    _lastRestartTime = Time.time;
                    _currentRecordingDuration = 0f;
                    UnityEngine.Debug.Log("Continuous recording session started");
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError($"Error starting continuous recording: {e.Message}");
                    _isProcessing = false;
                    _isReadyForNextRecord = true;
                    
                    // Try to restart after a longer delay
                    Invoke(nameof(StartContinuousRecording), 1f);
                }
            }
            else if (_stream == null && _isInitialized)
            {
                UnityEngine.Debug.LogWarning("Stream is null, attempting to recreate...");
                RecreateStream();
            }
        }

        private void StopContinuousRecording()
        {
            if (_stream != null && _isProcessing)
            {
                try
                {
                    microphoneRecord.StopRecord();
                    _isProcessing = false;
                    _isReadyForNextRecord = true;
                    _currentRecordingDuration = 0f;
                    UnityEngine.Debug.Log("Continuous recording session stopped");
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.LogError($"Error stopping continuous recording: {e.Message}");
                    _isProcessing = false;
                    _isReadyForNextRecord = true;
                }
            }
        }

        private async void RecreateStream()
        {
            try
            {
                UnityEngine.Debug.Log("Recreating WhisperStream...");
                
                // Cleanup old stream
                if (_stream != null)
                {
                    _stream.OnResultUpdated -= OnResult;
                    _stream.OnSegmentUpdated -= OnSegmentUpdated;
                    _stream.OnSegmentFinished -= OnSegmentFinished;
                    _stream.OnStreamFinished -= OnFinished;
                }
                
                // Create new stream
                _stream = await whisper.CreateStream(microphoneRecord);
                _stream.OnResultUpdated += OnResult;
                _stream.OnSegmentUpdated += OnSegmentUpdated;
                _stream.OnSegmentFinished += OnSegmentFinished;
                _stream.OnStreamFinished += OnFinished;
                
                UnityEngine.Debug.Log("WhisperStream recreated successfully");
                
                // Start recording with new stream
                StartContinuousRecording();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError($"Error recreating stream: {e.Message}");
                _isReadyForNextRecord = true;
                
                // Try again after delay
                Invoke(nameof(RecreateStream), 2f);
            }
        }

        private void Update()
        {
            if (!_isInitialized) return;
            
            // Update command highlight timers
            UpdateCommandHighlights();
            
            // Update continuous recording timers
            UpdateContinuousRecording();
        }

        private void UpdateContinuousRecording()
        {
            if (_isProcessing)
            {
                _currentRecordingDuration = Time.time - _recordingStartTime;
                
                // Auto restart recording every interval to prevent memory issues
                if (Time.time - _lastRestartTime >= autoRestartInterval)
                {
                    UnityEngine.Debug.Log($"Auto restarting recording after {autoRestartInterval}s");
                    RestartRecording();
                }
                
                // Emergency stop if recording too long
                if (_currentRecordingDuration >= maxRecordingDuration)
                {
                    UnityEngine.Debug.LogWarning($"Recording duration exceeded {maxRecordingDuration}s, restarting");
                    RestartRecording();
                }
            }
        }

        private void UpdateCommandHighlights()
        {
            // Update UP highlight
            if (_upHighlightTimer > 0)
            {
                _upHighlightTimer -= Time.deltaTime;
                if (_upHighlightTimer <= 0 && upText != null)
                {
                    upText.color = commandDefaultColor;
                }
            }
            
            // Update DOWN highlight
            if (_downHighlightTimer > 0)
            {
                _downHighlightTimer -= Time.deltaTime;
                if (_downHighlightTimer <= 0 && downText != null)
                {
                    downText.color = commandDefaultColor;
                }
            }
            
            // Update LEFT highlight
            if (_leftHighlightTimer > 0)
            {
                _leftHighlightTimer -= Time.deltaTime;
                if (_leftHighlightTimer <= 0 && leftText != null)
                {
                    leftText.color = commandDefaultColor;
                }
            }
            
            // Update RIGHT highlight
            if (_rightHighlightTimer > 0)
            {
                _rightHighlightTimer -= Time.deltaTime;
                if (_rightHighlightTimer <= 0 && rightText != null)
                {
                    rightText.color = commandDefaultColor;
                }
            }
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
            if (!_isInitialized) return;
            
            UnityEngine.Debug.Log($"VAD Changed: {isVoiceDetected}, IsRecording: {microphoneRecord.IsRecording}, IsProcessing: {_isProcessing}");

            if (_isAppPaused || !_isAppFocused || !_isSceneActive)
            {
                UnityEngine.Debug.Log($"VAD ignored - App paused: {_isAppPaused}, App focused: {_isAppFocused}, Scene active: {_isSceneActive}");
                return;
            }

            if (isVoiceDetected)
            {
                UnityEngine.Debug.Log("Voice detected - continuing continuous recording");
            }
            else
            {
                UnityEngine.Debug.Log("Silence detected - continuing to listen");
            }
        }

        // Called on stream segments (real-time partial results)
        private void OnSegmentUpdated(WhisperResult segment)
        {
            if (!streamSegments || !outputText || !_isInitialized)
                return;

            // Update UI with current segment
            if (outputText != null)
            {
                outputText.text = segment.Result + "...";
            }

            // Check if we have enough words for immediate processing
            if (HasEnoughWords(segment.Result))
            {
                // Handle immediate commands from each segment (debounced)
                TryHandleImmediateCommand(segment.Result);
            }
        }

        // Check if text has enough words to process
        private bool HasEnoughWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            
            string[] words = text.Trim().Split(new char[] { ' ', '\t', '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
            return words.Length >= minWordsToProcess;
        }

        // Called when a segment is finished
        private void OnSegmentFinished(WhisperResult segment)
        {
            if (!string.IsNullOrWhiteSpace(segment.Result) && _isInitialized)
            {
                UnityEngine.Debug.Log($"Segment finished: {segment.Result}");
                
                // Process the finished segment for ball movement
                ProcessWhisperResult(segment.Result);
            }
        }

        // Called when stream result is updated
        private void OnResult(string result)
        {
            if (!_isInitialized) return;
            
            if (outputText != null)
            {
                outputText.text = result;
            }
            
            if (!string.IsNullOrWhiteSpace(result))
            {
                UnityEngine.Debug.Log($"Stream result updated: {result}");
            }
        }

        // Called when stream is finished
        private void OnFinished(string finalResult)
        {
            if (!_isInitialized) return;
            
            UnityEngine.Debug.Log($"Stream finished with result: {finalResult}");
            
            // Process final result for ball movement
            if (!string.IsNullOrWhiteSpace(finalResult))
            {
                ProcessWhisperResult(finalResult);
            }
            
            // Always restart continuous recording for continuous listening
            _isProcessing = false;
            _isReadyForNextRecord = true;
            
            if (_isSceneActive && !_isAppPaused && _isAppFocused)
            {
                UnityEngine.Debug.Log("Restarting continuous recording after stream finished");
                StartContinuousRecording();
            }
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
                // Highlight UP text
                if (upText != null)
                {
                    upText.color = commandDetectedColor;
                    _upHighlightTimer = commandHighlightDuration;
                }
            }
            else if (lowerText.Contains("down") || lowerText.Contains("xuống") || lowerText.Contains("dưới"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveDown();
                    UnityEngine.Debug.Log("DOWN command detected - ball moving down");
                }
                // Highlight DOWN text
                if (downText != null)
                {
                    downText.color = commandDetectedColor;
                    _downHighlightTimer = commandHighlightDuration;
                }
            }
            else if (lowerText.Contains("left") || lowerText.Contains("trái"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveLeft();
                    UnityEngine.Debug.Log("LEFT command detected - ball moving left");
                }
                // Highlight LEFT text
                if (leftText != null)
                {
                    leftText.color = commandDetectedColor;
                    _leftHighlightTimer = commandHighlightDuration;
                }
            }
            else if (lowerText.Contains("right") || lowerText.Contains("phải"))
            {
                if (ballMove != null)
                {
                    ballMove.MoveRight();
                    UnityEngine.Debug.Log("RIGHT command detected - ball moving right");
                }
                // Highlight RIGHT text
                if (rightText != null)
                {
                    rightText.color = commandDetectedColor;
                    _rightHighlightTimer = commandHighlightDuration;
                }
            }
            else
            {
                UnityEngine.Debug.Log($"No movement command detected in: '{text}'");
            }
        }

        // Called when recording stops
        private void OnRecordStop(AudioChunk recordedAudio)
        {
            if (!_isInitialized) return;
            
            _isProcessing = false;
            _isReadyForNextRecord = true;

            UnityEngine.Debug.Log($"Recording stopped - Length: {recordedAudio.Length:F2}s, Samples: {recordedAudio.Data.Length}");

            // Always restart continuous recording for continuous listening
            if (!_isAppPaused && _isAppFocused && _isSceneActive)
            {
                UnityEngine.Debug.Log("Restarting continuous recording after record stop");
                StartContinuousRecording();
            }
        }

        // Scene management
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _isSceneActive = true;
            UnityEngine.Debug.Log($"Scene loaded: {scene.name}");
            
            // Restart recording when scene is loaded
            if (_isInitialized && !_isAppPaused && _isAppFocused)
            {
                StartContinuousRecording();
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            _isSceneActive = false;
            UnityEngine.Debug.Log($"Scene unloaded: {scene.name}");
            
            // Stop recording when scene is unloaded
            StopContinuousRecording();
        }

        // App lifecycle management
        private void OnApplicationPause(bool pauseStatus)
        {
            _isAppPaused = pauseStatus;
            UnityEngine.Debug.Log($"App paused: {pauseStatus}");

            if (pauseStatus)
            {
                StopContinuousRecording();
            }
            else
            {
                if (_isAppFocused && _isReadyForNextRecord && _isSceneActive)
                {
                    UnityEngine.Debug.Log("Restarting continuous recording after app resume");
                    StartContinuousRecording();
                }
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _isAppFocused = hasFocus;
            UnityEngine.Debug.Log($"App focused: {hasFocus}");

            if (!hasFocus)
            {
                StopContinuousRecording();
            }
            else
            {
                if (!_isAppPaused && _isReadyForNextRecord && _isSceneActive)
                {
                    UnityEngine.Debug.Log("Restarting continuous recording after app focus gain");
                    StartContinuousRecording();
                }
            }
        }

        private void OnDestroy()
        {
            UnityEngine.Debug.Log("StreamingSampleMic being destroyed - cleaning up resources");

            // Unsubscribe from scene events
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            // Stop recording
            StopContinuousRecording();

            // Cleanup stream
            if (_stream != null)
            {
                _stream.OnResultUpdated -= OnResult;
                _stream.OnSegmentUpdated -= OnSegmentUpdated;
                _stream.OnSegmentFinished -= OnSegmentFinished;
                _stream.OnStreamFinished -= OnFinished;
            }

            // Cleanup microphone
            if (microphoneRecord != null)
            {
                microphoneRecord.OnRecordStop -= OnRecordStop;
                microphoneRecord.OnVadChanged -= OnVadChanged;

                if (microphoneRecord.IsRecording)
                {
                    microphoneRecord.StopRecord();
                }
            }
            
            // Force garbage collection for performance
            System.GC.Collect();
        }
    }
}
