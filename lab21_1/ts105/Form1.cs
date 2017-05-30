using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;


namespace ts105
{
    public partial class Form1 : Form
    {
        public VideoCaptureDevice cam = null;
        public FilterInfoCollection usbCams;
        Bitmap image_from_cam;

        public Form1()
        {
            InitializeComponent();
        }

        void got_frame(object sender,NewFrameEventArgs eventArgs)
        {
            image_from_cam = (Bitmap)eventArgs.Frame.Clone();
            pictureBox1.Image = image_from_cam;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start") {
                btnStart.Text = "Stop";            
                usbCams = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                cam = new VideoCaptureDevice(usbCams[0].MonikerString);
                cam.NewFrame += new NewFrameEventHandler(got_frame); // 註冊got_frame 服務;
                btnCapture.Enabled = true;
                cam.Start();
            }
             else
            {
                btnStart.Text = "Start";
                btnCapture.Enabled = false;
                cam.Stop();
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog(this) == DialogResult.Cancel)
            {
                return;

            }
            image_from_cam.Save(sfd.FileName); // 存圖, 副檔名記得給他bmp
        }
    }
}
