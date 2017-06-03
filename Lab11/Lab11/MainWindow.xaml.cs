using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Lab11
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string faceApi_Key = "33413afb703240b18fc1ff941093b4b2";                  //KEY
        private IFaceServiceClient faceServiceClient = new FaceServiceClient(faceApi_Key);      //

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            string filePath = this.getFile();
            BitmapImage bitmapSource = null;
            try
            {
                bitmapSource = this.DrowPicture(filePath);
            }
            catch (ArgumentNullException artuNullEx)
            {
                status.Content = "尚未選取圖片";
                return;
            }
            status.Content = "已經選取圖片";

            Face[] faceInfo = await this.GetFaceInfo(filePath, FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.FacialHair, FaceAttributeType.Emotion);

            foreach (var tmp in faceInfo)
            {
                this.setText(tmp);
                this.ImgDrawRect(bitmapSource, tmp);
            }

        }

        //新增你要回傳回來的FACE屬性
        private List<FaceAttributeType> AddFaceAttributes(params FaceAttributeType[] attributeType)
        {
            List<FaceAttributeType> faceAttributes = new List<FaceAttributeType>();
            foreach (var tmp in attributeType)
            {
                faceAttributes.Add(tmp);
            }

            return faceAttributes;
        }

        //將圖片傳至微軟判別後取得臉部資訊
        private async Task<Face[]> GetFaceInfo(string imageFilePath, params FaceAttributeType[] attributeType)
        {
            List<FaceAttributeType> returnFaceAttributes = this.AddFaceAttributes(attributeType);

            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await this.faceServiceClient.DetectAsync(imageStream: imageFileStream, returnFaceLandmarks: true, returnFaceAttributes: returnFaceAttributes);
                    return faces.ToArray();
                }
            }
            catch (Exception e)
            {
                status.Content = "沒找到臉喔，請重新選擇照片";
                /**
                 * 必須將錯誤寫入LOG
                 */
                return new Face[0];
            }

        }

        //設定文字敘述
        private void setText(Face faceInfo)
        {
            //設定輸出資訊
            status.Content +=
                "年齡：" + faceInfo.FaceAttributes.Age +
                "\n性別：" + (faceInfo.FaceAttributes.Gender == "male" ? "男" : "女") +
                "\n鬍子濃度：" + faceInfo.FaceAttributes.FacialHair.Moustache +
                "\n生氣程度：" + faceInfo.FaceAttributes.Emotion.Anger + "\n\n";
        }

        //將臉部標示框框
        private void ImgDrawRect(BitmapImage bitmapSource, Face faceInfo)
        {
            //繪製人臉框初始設定
            DrawingVisual visual = new DrawingVisual();
            DrawingContext drawingContext = visual.RenderOpen();
            drawingContext.DrawImage(bitmapSource, new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
            double dpi = bitmapSource.DpiX;
            double resizeFactor = 96 / dpi;

            //設定人臉框框
            drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Yellow, 7),
                new Rect(
                    faceInfo.FaceRectangle.Left * resizeFactor,
                    faceInfo.FaceRectangle.Top * resizeFactor,
                    faceInfo.FaceRectangle.Width * resizeFactor,
                    faceInfo.FaceRectangle.Height * resizeFactor)
            );
            
            drawingContext.Close();
        }

        //取得圖片相關資訊
        private string getFile()
        {
            var openFileDilog = new Microsoft.Win32.OpenFileDialog();   //開啟檔案選擇視窗
            openFileDilog.Filter = "JPEG Image(*.jpg)|*.jpg";           //限定JPG格式
            
            //感覺上是在說是否有選擇檔案了，但從API的取名來看並不向是這麼一回事
            return (bool)openFileDilog.ShowDialog(this) ? openFileDilog.FileName : null;
        }

        //將圖片渲染至畫面上
        private BitmapImage DrowPicture(string filePath = null)
        {
            BitmapImage bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();                           //初始化開始
            bitmapSource.CacheOption = BitmapCacheOption.None;  //模式選取，選取了直接顯示畫面，圖片並不佔據記憶體空間
            bitmapSource.UriSource = new Uri(filePath);        //讀取要顯示的圖片
            bitmapSource.EndInit();                             //結束初始化
            image.Source = bitmapSource;                        //將畫面指定

            return bitmapSource;
        }

    }
    
}
