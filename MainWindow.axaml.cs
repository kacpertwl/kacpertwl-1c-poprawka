using System;
using Avalonia.Controls;
using Avalonia.Threading;

namespace kacpertwl_1c_poprawka;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private int _remainingSeconds;
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

        _remainingSeconds = 25 * 60;
        UpdateDisplay();
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
                // Przejście z pracy na przerwę
                _isWorkMode = false;
                _remainingSeconds = 5 * 60;
            }
            else
            {
                // Zakończenie przerwy - zaliczenie pełnego cyklu
                _isWorkMode = true;
                _completedCycles++;
                _remainingSeconds = 25 * 60;
            }
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int minutes = _remainingSeconds / 60;
        int seconds = _remainingSeconds % 60;

        TimerText.Text = $"{minutes:D2}:{seconds:D2}";
        CycleCountText.Text = $"Cykle: {_completedCycles}";
        StatusText.Text = _isWorkMode ? "Tryb: Praca (25 min)" : "Tryb: Przerwa (5 min)";
    }
}