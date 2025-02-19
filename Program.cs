using System;
using System.Diagnostics;
using System.Timers;

var dc = new DigitalClock();

dc.Inc();
dc.Inc();

dc.Set();


dc.Inc();
dc.Inc();
dc.Inc();

dc.Set();

Console.WriteLine(dc.GetTimeString());
Console.ReadLine();
class DigitalClock
{
    public enum ClockStatus { SetMins, SetHours, ShowTime }
    private Timer _timer = new Timer(1000);
    private int _mins;
    private int _hours;
    public int Mins
    {
        get { return _mins % 60; }
        set { _mins++; }
    }
    public int Hours
    {
        get { return _hours % 24; }
        set { _hours++; }
    }
    private ClockStatus _status;
    private ClockStatus Status
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
        Console.WriteLine(GetTimeString());
        if (Mins == 59) Hours++;
        Mins++;
    }

    public void Inc()
    {
        switch (Status)
        {
            case ClockStatus.SetMins:
                Mins++;
                Console.WriteLine($"piip-mm-{Format(Mins)}");
                break;
            case ClockStatus.SetHours:
                Hours++;
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
}