using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : ICube
{
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text timerText;
    [SerializeField] private Text levelText;
    [SerializeField] private Text bonusText; 
    [SerializeField] private float timeLimit = 60f;
    [SerializeField] private float bonusMultiplier = 0.2f; 
    
    private bool _isVictory = false;
    private bool _isGameOver = false;
    private float _currentTime;
    private int _currentLevel = 0; 
    private int _totalLevels = 0;

    protected override void Awake()
    {
        base.Awake();

        if (victoryPanel == null)
            victoryPanel = GameObject.FindGameObjectWithTag("VictoryPanel");

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverPanel == null)
            gameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel");

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        _currentLevel = SceneManager.GetActiveScene().buildIndex;
        _totalLevels = SceneManager.sceneCountInBuildSettings;

        _currentTime = timeLimit;
        
        LoadProgress();

        UpdateTimerUI();
        UpdateLevelUI();
    }

    private void Update()
    {
        if (_isVictory || _isGameOver)
            return;

        _currentTime -= Time.deltaTime;
        
        if (_currentTime < 0)
            _currentTime = 0;
            
        UpdateTimerUI();

        if (_currentTime <= 0 && !_isGameOver)
            ShowGameOver();
    }

    protected override void FixedUpdate()
    {
        if (_isVictory || _isGameOver)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _rb.isKinematic = true;
            return;
        }

        if (_keyboard[Key.R].isPressed)
            RestartGame();

        base.FixedUpdate();
    }

    protected override Vector3 GetMovement()
    {
        if (_isVictory || _isGameOver)
            return Vector3.zero;

        return base.GetMovement();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (_isVictory || _isGameOver)
            return;

        if (this.CompareTag("Player") && other.CompareTag("Finish"))
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

            float bonusTime = CalculateBonusTime();
            _currentTime += bonusTime;
        
            ShowBonusEffect(bonusTime);

            Debug.Log($"Level completed! Time: {_currentTime}s, Bonus: +{bonusTime}s");

            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SaveProgress(nextSceneIndex);
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
                ShowVictory();
        }

        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (_isVictory || _isGameOver)
            return;

        base.OnTriggerExit(other);
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            float displayTime = Mathf.Max(0, _currentTime);
            
            int minutes = Mathf.FloorToInt(displayTime / 60);
            int seconds = Mathf.FloorToInt(displayTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (displayTime <= 10)
                timerText.color = Color.red;
            else if (displayTime <= 30)
                timerText.color = Color.yellow;
            else
                timerText.color = Color.white;
        }
    }

    private void UpdateLevelUI()
    {
        if (levelText != null)
        {
            int currentLevelNumber = _currentLevel + 1;
            levelText.text = $"Level: {currentLevelNumber}/{_totalLevels}";
        }
    }

    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey("CurrentLevel"))
        {
            int savedLevel = PlayerPrefs.GetInt("CurrentLevel");
            if (savedLevel == _currentLevel)
            {
                if (PlayerPrefs.HasKey("RemainingTime"))
                {
                    _currentTime = PlayerPrefs.GetFloat("RemainingTime");
                    Debug.Log($"Progress loaded! Level: {savedLevel}, Time: {_currentTime}");
                    
                    // Показываем бонус при загрузке уровня
                    if (PlayerPrefs.HasKey("BonusAmount"))
                    {
                        float bonus = PlayerPrefs.GetFloat("BonusAmount");
                        ShowBonusEffect(bonus);
                        PlayerPrefs.DeleteKey("BonusAmount");
                    }
                    
                    return;
                }
            }
        }
        
        _currentTime = timeLimit;
        Debug.Log($"No progress found. Using default time: {_currentTime}");
    }

    private void SaveProgress(int nextLevelIndex)
    {
        PlayerPrefs.SetInt("CurrentLevel", nextLevelIndex);
        PlayerPrefs.SetFloat("RemainingTime", _currentTime);
        
        // Сохраняем бонус для отображения на следующем уровне
        float bonusTime = CalculateBonusTime();
        PlayerPrefs.SetFloat("BonusAmount", bonusTime);
        
        PlayerPrefs.Save();
        Debug.Log($"Progress saved! Level: {nextLevelIndex}, Time: {_currentTime}, Bonus: +{bonusTime}");
    }

    private void ShowGameOver()
    {
        if (_isGameOver) 
            return;
        
        _isGameOver = true;
        _isVictory = false;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        _currentTime = 0;
        UpdateTimerUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("GAME OVER! Time's up!");
        }
        else
            Debug.LogWarning("GameOverPanel is not assigned!");
    }

    private void ShowVictory()
    {
        if (_isVictory) 
            return;
        
        _isVictory = true;
        _isGameOver = false;

        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.isKinematic = true;

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Debug.Log("VICTORY! All levels completed!");
            
            PlayerPrefs.DeleteKey("CurrentLevel");
            PlayerPrefs.DeleteKey("RemainingTime");
            PlayerPrefs.DeleteKey("BonusAmount");
        }
        else
            Debug.LogWarning("VictoryPanel is not assigned!");
    }

    private void ShowBonusEffect(float bonusAmount)
    {
        if (bonusText != null)
        {
            bonusText.text = $"+{bonusAmount:F1}s BONUS!";
            bonusText.color = Color.green;
            bonusText.gameObject.SetActive(true);
            
            Invoke(nameof(HideBonusText), 2f);
        }
    }

    private void HideBonusText()
    {
        if (bonusText != null)
            bonusText.gameObject.SetActive(false);
    }

    private float CalculateBonusTime()
    {
        float bonus = _currentTime * bonusMultiplier;
        
        float maxBonus = 30f;
        bonus = Mathf.Min(bonus, maxBonus);
        
        return bonus;
    }

    public void RestartGame()
    {
        _isVictory = false;
        _isGameOver = false;
        _rb.isKinematic = false;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.DeleteKey("RemainingTime");
        PlayerPrefs.DeleteKey("BonusAmount");

        _currentTime = timeLimit;
        UpdateTimerUI();

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}