﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Media;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Principal;

namespace dayz64
{
    public partial class Form1 : Form
    {
        //DLL FOR VOLUME CONTROL
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        //GET PATH OST AND INTRO SOUND FILE PATH
        static Stream istr = Properties.Resources.ost;

        //INTRO PLAYER
        private SoundPlayer iplay = new SoundPlayer(istr);

        //OST PLAYER
        private SoundPlayer oplay = new SoundPlayer(istr);

        //DAYZ PROCESSES
        Process[] process32;
        Process[] process64;

        //CHECK IF ADMIN
        private bool isAdmin;

        //OST AND INTRO START SETTINGS
        private static int playAfterIntro = 0;
        private static int playAfterOST = 60;
        private int _playAfterIntro;
        private int _playAfterOST;
        private bool isPlaying = false;

        //THREADS
        Thread thread1;

        //ON LOAD FORM
        public Form1()
        {
            InitializeComponent();

            //SET VOLUME
            uint CurrVol = 0;
            waveOutGetVolume(IntPtr.Zero, out CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            trackBarIntro.Value = CalcVol / (ushort.MaxValue / 30);

            //FORMS SETTINGS
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            button1.Text = "PLAY";
            checkBox1.Checked = false;
            numericUpDown1.Value = playAfterIntro;
            numericUpDown2.Value = playAfterOST;
            checkIntroTheme.Checked = true;
            checkOSTTheme.Checked = true; 

            //TIMER INTRO
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(countInt);

            //TIMER INTRO
            timer2.Interval = 1000;
            timer2.Tick += new EventHandler(countOST);

            //ADMIN
            isAdmin = IsAdministrator();

            if (isAdmin == false)
            {
                MessageBox.Show("You didn't run the launcher as an Administrator. This may cause some functions to not properly work", "Not running as admin", MessageBoxButtons.OK);
            }
        }

        //PLAY BUTTON PRESS EVENT
        private void button1_Click(object sender, EventArgs e)
        {
            if (isPlaying == false)
            {
                launch();
            }else
            {
                stop();
            }
        }

        private void getProc()
        {
            Thread.Sleep(5000);

            process32 = Process.GetProcessesByName("DayZ");
            process64 = Process.GetProcessesByName("DayZ_x64");

            if(checkHighPriority.Checked == true)
            {
                setHighPriority();
            }
        }

        //LAUNCH GAME AND PLAY SOUND
        private void launch()
        {
            //GET WAIT TIMES
            _playAfterIntro = (int)numericUpDown1.Value;
            _playAfterOST = (int)numericUpDown2.Value;

            //START TIMER 1
            if (checkIntroTheme.Checked == true)
            {
                timer1.Start();
            }

            //START TIMER 1
            if (checkOSTTheme.Checked == true)
            {
                timer2.Start();
            }

            isPlaying = true;

            //START DAYZ THROUGH STEAM
            if (checkBox1.Checked == true)
            {
                Process.Start("steam://rungameid/221100");

                //HIGH PROCESS THREAD
                thread1 = new Thread(getProc);
                thread1.Start();
            }

            button1.Text = "STOP";
        }

        //STOP SOUND
        private void stop()
        {
            iplay.Stop();
            isPlaying = false;
            button1.Text = "PLAY";

            foreach (Process pro32 in process32)
            {
                pro32.Kill();
            }

            foreach (Process pro64 in process64)
            {
                pro64.Kill();
            }
        }

        //TIMER INTRO EVENT 
        private void countInt(object sender, System.EventArgs e)
        {
            if(_playAfterIntro > 0)
            {
                _playAfterIntro--;
            }

            if (_playAfterIntro <= 0 && checkIntroTheme.Checked == true)
            {
                //PLAY AUDIO
                iplay.Stream.Position = 0;
                iplay.Play();
                timer1.Stop();
                _playAfterIntro = playAfterIntro;

                MessageBox.Show("PLAY INTRO");
            }
        }

        //TIMER OST EVENT
        private void countOST(object sender, System.EventArgs e)
        {
            if (_playAfterOST > 0)
            {
                _playAfterOST--;
            }

            if (_playAfterOST <= 0 && checkOSTTheme.Checked == true)
            {
                //PLAY AUDIO
                oplay.Stream.Position = 0;
                oplay.Play();
                timer2.Stop();
                //_playAfterOST = playAfterOST;

                MessageBox.Show("PLAY OST");
            }
        }

        //VOLUME BAR INTRO
        private void trackBarVolume(object sender, EventArgs e)
        {
            int NewVolume = ((ushort.MaxValue / 30) * trackBarIntro.Value);

            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));

            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }

        private void setHighPriority()
        {
            foreach (Process pro32 in process32)
            {
                pro32.PriorityClass = ProcessPriorityClass.High;
            }

            foreach (Process pro64 in process64)
            {
                pro64.PriorityClass = ProcessPriorityClass.High;
            }
        }

        //COUNTDOWN TIMER SET
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            playAfterIntro = (int)numericUpDown1.Value;
            playAfterOST = (int)numericUpDown2.Value;
        }

        //CHECK IF STARTED AS ADMIN
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
