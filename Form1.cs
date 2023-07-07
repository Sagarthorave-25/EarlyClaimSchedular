using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchedulerEarlyClaim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
      helper help=  new helper();
        private void Form1_Load(object sender, EventArgs e)
        {
            DataSet ds = help.GetPdfData();
            if (ds.Tables[0] != null) {
                if (ds.Tables[0].Rows.Count > 0) {
                    string token = help.BlobTokn();
                    if (!string.IsNullOrEmpty(token)) { 
                        help.GeneratePdf(ds, token);
                    }
                }
            }
        }
    }
    }
