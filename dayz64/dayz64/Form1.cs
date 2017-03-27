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

namespace dayz64
{
    public partial class Form1 : Form
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);

        private static string opath = Application.StartupPath + @"\resources\ost.wav";
        private static string ipath = Application.StartupPath + @"\resources\int.wav";

        //OST PLAYER
        private SoundPlayer iplay = new SoundPlayer(ipath);

        //OST PLAYER
        private SoundPlayer oplay = new SoundPlayer(opath);

        private static int playAfterIntro = 2;
        private static int playAfterOST = 60;
        private int _playAfterIntro;
        private int _playAfterOST;
        private bool isPlaying = false;

        //ON LOAD FORM
        public Form1()
        {
            InitializeComponent();

            //SET VOLUME
            uint CurrVol = 0;
            waveOutGetVolume(IntPtr.Zero, out CurrVol);
            ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
            trackBarIntro.Value = CalcVol / (ushort.MaxValue / 30);
            trackBarOST.Value = CalcVol / (ushort.MaxValue / 30);

            //FORMS SETTINGS
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            button1.Text = "PLAY";
            checkBox1.Checked = false;
            numericUpDown1.Value = playAfterIntro;
            numericUpDown2.Value = playAfterOST;

            //PLAYER AFTER SETTINGS
            _playAfterIntro = (int)numericUpDown1.Value;
            _playAfterOST = (int)numericUpDown1.Value;

            //TIMER INTRO
            timer1.Interval = 1000;
            timer1.Tick += new EventHandler(countInt);

            //TIMER INTRO
            timer2.Interval = 1000;
            timer2.Tick += new EventHandler(countOST);
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

        //LAUNCH GAME AND PLAY SOUND
        private void launch()
        {
            //START TIMER
            timer1.Start();

            //ISPLAYING TRUE
            isPlaying = true;

            //START DAYZ THROUGH STEAM
            if (checkBox1.Checked == true)
            {
                Process.Start("steam://rungameid/221100");
            }

            button1.Text = "STOP";
        }

        //STOP SOUND
        private void stop()
        {
            iplay.Stop();
            isPlaying = false;
            button1.Text = "PLAY";
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
                iplay.Play();
                timer1.Stop();
                //_playAfterIntro = playAfterIntro;

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
                oplay.Play();
                timer2.Stop();
                //_playAfterOST = playAfterOST;

                MessageBox.Show("PLAY OST");
            }
        }

        //VOLUME BAR INTRO
        private void trackBarIntro_Scroll(object sender, EventArgs e)
        {
            int NewVolume = ((ushort.MaxValue / 30) * trackBarIntro.Value);

            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));

            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }

        //VOLUME BAR
        private void trackBarOST_Scroll(object sender, EventArgs e)
        {
            int NewVolume = ((ushort.MaxValue / 30) * trackBarIntro.Value);

            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));

            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }

        //COUNTDOWN TIMER SET
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            playAfterIntro = (int)numericUpDown1.Value;
            playAfterOST = (int)numericUpDown2.Value;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}