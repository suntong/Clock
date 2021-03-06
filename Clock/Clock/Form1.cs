﻿using System;
using System.Drawing;
using System.Windows.Forms;
using IWshRuntimeLibrary;


class DigitalClock : Form
{
    double OpacityStep = 0.2;

    public DigitalClock()
    {
        stylSetting();
        timer();
        this.MouseClick += dcMouseClick;
        this.KeyDown += dcKeyDown;
        Shortcut_Start();
    }

    private void dcMouseClick(object sender, MouseEventArgs e)
    {
        double Opacity = this.Opacity;

        switch (e.Button)
        {
            case MouseButtons.Left:
                // Left click, increase opacity
                if (Opacity < 1) 
                    Opacity += OpacityStep;
                break;

            case MouseButtons.Right:
                // Right click, decrease opacity
                if (Opacity > OpacityStep)
                    Opacity -= OpacityStep;
                break;
        }
        this.Opacity = Math.Round(Opacity, 1);
    }

    public void stylSetting()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.DoubleBuffered = true;
        this.SetStyle(ControlStyles.ResizeRedraw, true);
        this.TopMost = true;
        ResizeRedraw = true;
        Rectangle workingArea = Screen.GetWorkingArea(this);
        this.Location = new Point(workingArea.Right - Size.Width,
                                  workingArea.Bottom - Size.Height);
        this.Size = new Size(90, 50);
        this.ShowInTaskbar = false;

    }
    public void Shortcut_Start()
    {

        WshShell shell = new WshShell();
        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\Clock.lnk");

        shortcut.TargetPath = Application.ExecutablePath;
      
        shortcut.Description = "Make clock autostart whenever the PC start";

        //shortcut.Save();
    }
    public void timer()
    {
        Timer timer = new Timer();
        timer.Tick += new EventHandler(TimerOnTick);
        timer.Interval = 1000;
        timer.Start();
    }
    private void TimerOnTick(object obj, EventArgs ea)
    {
        this.Invalidate();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        string strTime = System.DateTime.Now.ToLongTimeString().Remove(System.DateTime.Now.ToLongTimeString().Length - 3);
        SizeF sizef = g.MeasureString(strTime, Font);
        float fScale = Math.Min(ClientSize.Width / sizef.Width,
                                    ClientSize.Height / sizef.Height);
        Font font = new Font(Font.FontFamily,
                                    fScale * Font.SizeInPoints);

        StringFormat strfmt = new StringFormat();
        strfmt.Alignment = strfmt.LineAlignment = StringAlignment.Center;

        g.DrawString(strTime, font, new SolidBrush(ForeColor),
                        ClientRectangle, strfmt);
    }

    protected override void WndProc(ref Message m)
    {
        const int cGrip = 16;      // Grip size
        const int cCaption = 16;   // Caption bar height
        if (m.Msg == 0x84)
        {
            Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
            pos = this.PointToClient(pos);
            if (pos.Y < cCaption)
            {
                m.Result = (IntPtr)0x2;  // HTCAPTION
                return;
            }
            if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
            {
                m.Result = (IntPtr)0x11; // HTBOTTOMRIGHT
                return;
            }
        }

        base.WndProc(ref m);
    }

    private void InitializeComponent()
    {
            this.SuspendLayout();
            // 
            // DigitalClock
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "DigitalClock";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dcKeyDown);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dcMouseClick);
            this.ResumeLayout(false);

    }

    private void dcKeyDown(object sender, KeyEventArgs e)
    {
        if ( (e.Alt && e.KeyCode == Keys.Q) ||
            (e.Control && e.KeyCode == Keys.Q))
        {
            Application.Exit();
        }
    }
}
