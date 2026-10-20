using System;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace kacpertwl_1c_poprawka;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly ObservableCollection<string> _cycleHistory = new();
    private int _remainingSeconds;
    private bool _isRunning = false;
    private bool _isWorkMode = true;
    private int _completedCycles = 0;

    public MainWindow()
    {
        InitializeComponent();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;

        StartPauseButton.Click += StartPauseButton_Click;
        ResetButton.Click += ResetButton_Click;
        WorkDurationInput.ValueChanged += WorkDurationInput_ValueChanged;
        BreakDurationInput.ValueChanged += BreakDurationInput_ValueChanged;

        CycleHistoryListBox.ItemsSource = _cycleHistory;

        _remainingSeconds = GetWorkDurationSeconds();
        UpdateDisplay();
    }

    private int GetWorkDurationSeconds()
    {
        int minutes = (int)Math.Round(WorkDurationInput?.Value ?? 25m);
        if (minutes < 1) minutes = 1;
        return minutes * 60;
    }

    private int GetBreakDurationSeconds()
    {
        int minutes = (int)Math.Round(BreakDurationInput?.Value ?? 5m);
        if (minutes < 1) minutes = 1;
        return minutes * 60;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_remainingSeconds > 0)
        {
            _remainingSeconds--;
        }
        else
        {
            if (_isWorkMode)
            {
                // Po skończonej pracy przerwa rusza sama
                _isWorkMode = false;
                _remainingSeconds = GetBreakDurationSeconds();
            }
            else
            {
                // Zakończono przerwę - pełen cykl ukończony
                _completedCycles++;
                int workMinutes = (int)Math.Round(WorkDurationInput?.Value ?? 25m);
                int breakMinutes = (int)Math.Round(BreakDurationInput?.Value ?? 5m);
                string entry = $"Cykl {_completedCycles}: Praca {workMinutes} min + Przerwa {breakMinutes} min (ukończono: {DateTime.Now:HH:mm:ss})";
                _cycleHistory.Add(entry);

                _isWorkMode = true;
                _remainingSeconds = GetWorkDurationSeconds();
            }
        }

        UpdateDisplay();
    }

    private void StartPauseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_isRunning)
        {
            _timer.Stop();
            _isRunning = false;
            StartPauseButton.Content = "Start";
        }
        else
        {
            _timer.Start();
            _isRunning = true;
            StartPauseButton.Content = "Pauza";
        }

        UpdateDisplay();
    }

    private void ResetButton_Click(object? sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _isRunning = false;
        _isWorkMode = true;
        _remainingSeconds = GetWorkDurationSeconds();

        StartPauseButton.Content = "Start";
        UpdateDisplay();
    }

    private void WorkDurationInput_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_isRunning && _isWorkMode)
        {
            _remainingSeconds = GetWorkDurationSeconds();
            UpdateDisplay();
        }
    }

    private void BreakDurationInput_ValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        if (!_isRunning && !_isWorkMode)
        {
            _remainingSeconds = GetBreakDurationSeconds();
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        int minutes = _remainingSeconds / 60;
        int seconds = _remainingSeconds % 60;

        TimerText.Text = $"{minutes:D2}:{seconds:D2}";
        CycleCountText.Text = $"Cykle: {_completedCycles}";
        StatusText.Text = _isWorkMode ? "Tryb: Praca" : "Tryb: Przerwa";
    }
}