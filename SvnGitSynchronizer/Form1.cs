﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SyncRepo;
using System.Collections.ObjectModel;

namespace SvnGitSynchronizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void txtSvnURL_Enter(object sender, EventArgs e)
        {

        }

        private void txtSvnURL_Leave(object sender, EventArgs e)
        {

        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            string svnCommand = Method.GetInfo(txtTortoiseLocation.Text);
            
            if (svnCommand == "update")
            {
                MessageBox.Show("updated successfully");
            }
            else if (svnCommand == "commit")
            {
                MessageBox.Show("commited successfully");
            }
            else if (svnCommand == "nothing changed")
            {
                MessageBox.Show("nothing changed....");
            }
        }
    }
}

//var fileInfoLog = Directory.EnumerateFiles(txtTortoiseLocation.Text, "*_info.xml", SearchOption.AllDirectories).ToArray();
//if (!Method.CompareFile(path, fileInfoLog[0]))
//{
//    MessageBox.Show("different log");
//}
//else
//    MessageBox.Show("nothing change");