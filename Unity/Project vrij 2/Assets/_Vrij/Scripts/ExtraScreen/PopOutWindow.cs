using System;
using System.Threading;
using System.Windows.Forms;
using UnityEngine;

public class WindowsPopOut : MonoBehaviour
{
    private Thread windowThread;
    private Label timerLabel;
    private System.Windows.Forms.Timer timer;
    private int currentSeconds;


    void Start()
    {
        DelegateManager.Instance.StartTimerDelegate += StartTimer;
        CreatePopupWindow();
    }

    void OnApplicationQuit()
    {
        if (windowThread != null && windowThread.IsAlive)
        {
            windowThread.Abort();
        }
    }

    private void CreatePopupWindow()
    {
        windowThread = new Thread(() =>
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            Form timerForm = new Form
            {
                Text = "Timer Window",
                Width = 200,
                Height = 100,
                StartPosition = FormStartPosition.CenterScreen
            };

            timerLabel = new Label
            {
                Text = "Waiting...",
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Arial", 24),
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            timerForm.Controls.Add(timerLabel);

            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            timer.Tick += Timer_Tick;

            System.Windows.Forms.Application.Run(timerForm);
        });

        windowThread.SetApartmentState(ApartmentState.STA);
        windowThread.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (currentSeconds > 0)
        {
            currentSeconds--;
            timerLabel.Invoke((MethodInvoker)(() => timerLabel.Text = currentSeconds.ToString()));
        }
        else
        {
            timer.Stop();
            timerLabel.Invoke((MethodInvoker)(() => timerLabel.Text = "Done"));
        }
    }

    private void StartTimer(int seconds)
    {
        currentSeconds = seconds;
        timerLabel.Invoke((MethodInvoker)(() => timerLabel.Text = seconds.ToString()));
        timer.Start();
    }
}
