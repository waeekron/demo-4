using System;
using System.Timers;

var dc = new DigitalClock();
var run = true;
while (run)
{
    Console.Clear();
    Console.WriteLine(dc.GetStatusPrompt());
    var switchState = false;
    while (!switchState)
    {
        var key = Console.ReadKey(intercept: true);
        (key.Key switch
        {
            ConsoleKey.J => () => dc.Dec(),
            ConsoleKey.K => () => dc.Inc(),
            ConsoleKey.Enter => () =>
            {
                dc.Set();
                switchState = true;
            }
            ,
            _ => (Action)(() => Console.WriteLine("Bad input..."))
        })();
    }
}

class DigitalClock
{
    public enum ClockStatus { SetMins, SetHours, ShowTime }
    private Timer _timer = new Timer(1000);
    private int _mins;
    private int _hours;
    public int Mins
    {
        get { return _mins % 60; }
        set
        {
            if (_mins == 0 && value == -1) _mins = 59;
            else _mins = value;
        }
    }
    public int Hours
    {
        get { return (_hours % 24; }
        set
        {
            if (_hours == 0 && value == -1) _hours = 23;
            else _hours = value;
        }

    }
    private ClockStatus _status;
    public ClockStatus Status
    {
        get { return _status; }
        set
        {
            if (value == _status) throw new Exception($"status is already {_status}");
            _status = value;
        }
    }


    public DigitalClock()
    {
        _status = ClockStatus.SetHours;
        _timer.Elapsed += Step;
    }

    public void Set()
    {
        var nextStatus = Status switch
        {
            ClockStatus.SetHours => ClockStatus.SetMins,
            ClockStatus.SetMins => ClockStatus.ShowTime,
            ClockStatus.ShowTime => ClockStatus.SetHours,
            _ => throw new ArgumentException("invalid state")
        };
        if (nextStatus == ClockStatus.ShowTime) Tick();
        else StopTicking();
        Status = nextStatus;
    }

    private void StopTicking()
    {
        _timer.Stop();
    }

    private void Tick()
    {
        _timer.Start();
    }

    private void Step(object sender, ElapsedEventArgs e)
    {
        Console.Clear();
        Console.WriteLine("PRESS ANY BUTTON TO RESET");
        Console.WriteLine(GetTimeString());
        if (Mins == 59) Hours++;
        Mins++;
    }

    public void Inc()
    {
        switch (Status)
        {
            case ClockStatus.SetMins:
                Mins += 1;
                Console.WriteLine($"piip-mm-{Format(Mins)}");
                break;
            case ClockStatus.SetHours:
                Hours += 1;
                Console.WriteLine($"piip-hh-{Format(Hours)}");
                break;

            default:
                Console.WriteLine("pööp -- invalid state");
                break;

        }
    }
    public void Dec()
    {
        switch (Status)
        {
            case ClockStatus.SetMins:
                Mins -= 1;
                Console.WriteLine($"piip-mm-{Format(Mins)}");
                break;
            case ClockStatus.SetHours:
                Hours -= 1;
                Console.WriteLine($"piip-hh-{Format(Hours)}");
                break;

            default:
                Console.WriteLine("pööp -- invalid state");
                break;

        }
    }
    private String Format(int time) => time switch
    {
        < 10 => $"0{time.ToString()}",
        >= 10 => time.ToString(),
    };
    public String GetTimeString() => Status switch
    {
        ClockStatus.SetHours => Format(Hours),
        ClockStatus.SetMins => $"{Format(Hours)}:{Format(Mins)}",
        ClockStatus.ShowTime => $"{Format(Hours)}:{Format(Mins)}",
        _ => throw new ArgumentException("invalid state")
    };
    public String GetStatusPrompt() => Status switch
    {
        ClockStatus.SetHours => "Set Hours, press j to decrement, k to increment, enter to set",
        ClockStatus.SetMins => "Set Mins, j to decrement, k to increment, enter to set",
        ClockStatus.ShowTime => "winding the clock...",
        _ => throw new ArgumentException("invalid state")
    };
}