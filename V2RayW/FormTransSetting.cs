﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace V2RayW
{
    public partial class FormTransSetting : Form
    {
        public FormTransSetting()
        {
            InitializeComponent();
        }

        private void buttonTSCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonTSHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.v2ray.com/chapter_02/05_transport.html");
        }

        private void buttonTSSave_Click(object sender, EventArgs e)
        {
            var res = MessageBox.Show("Make sure you have read the help before clicking OK!", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            if (res != DialogResult.OK)
            {
                return;
            }
            var muxSettings = new
            {
                enabled = checkBoxMuxEnable.Checked,
                concurrency = Program.strToInt(textBoxMuxCc.Text, 8)
            };
            var transportSettings = new {
                network = "tcp",
                security = checkBoxTLSEnable.Checked ? "tls" : "none",
                tlsSettings = new {
                    serverName = textBoxTLSSn.Text,
                    allowInsecure = checkBoxTLSAI.Checked
                },
                kcpSettings = new 
                {
                    mtu = Program.strToInt(textBoxKcpMtu.Text, 1350),
                    tti = Program.strToInt(textBoxKcpTti.Text, 50),
                    uplinkCapacity = Program.strToInt(textBoxKcpUc.Text, 5),
                    downlinkCapacity = Program.strToInt(textBoxKcpDc.Text, 20),
                    readBufferSize = Program.strToInt(textBoxKcpRb.Text, 2),
                    writeBufferSize = Program.strToInt(textBoxKcpWb.Text, 2),
                    congestion = comboBoxKcpCon.SelectedIndex == 1,
                    header = new
                    {
                        type = comboBoxKcpHt.Text
                    }
                },
                tcpSettings = new 
                {
                    connectionReuse = checkBoxTcpCr.Checked,
                    header = new
                    {
                        type = comboBoxTcpHt.Text
                    }
                },
                wsSettings = new
                {
                    connectionReuse = checkBoxWsCr.Checked,
                    path = textBoxWsPath.Text,
                } 
            };
            Properties.Settings.Default["transportSettings"] = JsonConvert.SerializeObject(transportSettings);
            Properties.Settings.Default.mux = JsonConvert.SerializeObject(muxSettings);
            Properties.Settings.Default.Save();
            this.Close();
        }



        private void FormTransSetting_Load(object sender, EventArgs e)
        {
            //Properties.Settings.Default.Upgrade();
            string transportSettingsStr = Properties.Settings.Default.transportSettings;
            dynamic transportSettings = JObject.Parse(transportSettingsStr);
            textBoxKcpMtu.Text = transportSettings.kcpSettings.mtu;
            textBoxKcpTti.Text = transportSettings.kcpSettings.tti;
            textBoxKcpUc.Text = transportSettings.kcpSettings.uplinkCapacity;
            textBoxKcpDc.Text = transportSettings.kcpSettings.downlinkCapacity;
            textBoxKcpRb.Text = transportSettings.kcpSettings.readBufferSize;
            textBoxKcpWb.Text = transportSettings.kcpSettings.writeBufferSize;
            comboBoxKcpCon.SelectedIndex = transportSettings.kcpSettings.congestion == false ? 0 : 1;
            var headertype = transportSettings.kcpSettings.header.type;
            comboBoxKcpHt.SelectedIndex = headertype == "srtp" ? 1 : (headertype == "utp" ? 2 : 0);
            checkBoxTcpCr.Checked = transportSettings.tcpSettings.connectionReuse;
            comboBoxTcpHt.SelectedIndex = transportSettings.tcpSettings.header.type == "none" ? 0 : 1;
            checkBoxWsCr.Checked = transportSettings.wsSettings.connectionReuse;
            textBoxWsPath.Text = transportSettings.wsSettings.path;

            checkBoxTLSEnable.Checked = transportSettings.security == "tls";
            checkBoxTLSAI.Checked = transportSettings.tlsSettings.allowInsecure;
            textBoxTLSSn.Text = transportSettings.tlsSettings.serverName;

            string muxSettingsStr = Properties.Settings.Default.mux;
            dynamic muxSettings = JObject.Parse(muxSettingsStr);
            checkBoxMuxEnable.Checked = muxSettings.enabled;
            textBoxMuxCc.Text = muxSettings.concurrency;
        }

        private void buttonTsReset_Click(object sender, EventArgs e)
        {
            textBoxKcpMtu.Text = "1350";
            textBoxKcpTti.Text = "20";
            textBoxKcpUc.Text = "5";
            textBoxKcpDc.Text = "20";
            textBoxKcpRb.Text = "2";
            textBoxKcpWb.Text = "2";
            comboBoxKcpCon.SelectedIndex = 0;
            comboBoxKcpHt.SelectedIndex = 0;
            checkBoxTcpCr.Checked = true;
            comboBoxTcpHt.SelectedIndex = 0;
            checkBoxWsCr.Checked = true;
            textBoxWsPath.Text = "";

            checkBoxMuxEnable.Checked = false;
            textBoxMuxCc.Text = "8";

            checkBoxTLSEnable.Checked = false;
        }
    }
}
