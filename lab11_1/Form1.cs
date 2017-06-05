using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lab1_WindowsForm
{
    public partial class Form1 : Form
    {

        private const string faceApi_Key = "33413afb703240b18fc1ff941093b4b2";                  //KEY
        private IFaceServiceClient faceServiceClient = new FaceServiceClient(faceApi_Key);      //

        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string imgPath = null;
            try
            {
                imgPath = this.GetImageFilePath();
                this.label1.Text = imgPath;
            }
            catch(ArgumentNullException)
            {
                return;
            }

            this.DrowImg(imgPath);
            this.label2.Visible = false;

            Face[] facesInfo = await GetFacesInfo(imgPath, FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.FacialHair, FaceAttributeType.Emotion);
            this.label1.Text = "找到了" + facesInfo.Length + "張臉";

            this.ShowData(facesInfo);

        }

        private void ShowData(Face[] facesInfo)
        {
            foreach (var tmp in facesInfo)
            {
                ListViewItem item = new ListViewItem(tmp.FaceId.ToString());
                item.SubItems.Add("" + tmp.FaceAttributes.Age);
                item.SubItems.Add("" + tmp.FaceAttributes.Gender == "male" ? "男" : "女");
                item.SubItems.Add("" + tmp.FaceAttributes.FacialHair.Moustache);
                item.SubItems.Add("" + tmp.FaceAttributes.Emotion.Anger);
                this.listView1.Items.Add(item);
            }
        }

        private async Task<Face[]> GetFacesInfo(string path, params FaceAttributeType[] attribute)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(path))
                {
                    var facesInfo = await this.faceServiceClient.DetectAsync(imageFileStream, returnFaceId: true, returnFaceAttributes: attribute);
                    return facesInfo;
                }
            }
            catch(Exception)
            {
                return new Face[0];
            }
        }

        private void DrowImg(string path)
        {
            Bitmap img = new Bitmap(path);
            pictureBox1.Image = img;
        }

        private string GetImageFilePath()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "JPEG Image(*.jpg)|*.jpg";
            if (fileDialog.ShowDialog(this) != DialogResult.OK)
            {
                throw new ArgumentNullException();
            }
                
            return fileDialog.FileName;
        }

    }
}
