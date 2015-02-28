using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;

namespace QRCode生成解析工具
{
    /// <summary>
    /// 生成和解析二维码，可以在二维码中添加logo.
    /// 生成二维码时，可一设定二维码的版本和大小，以及设置logo的大小.
    /// 生成成功后，可以通过解析二维码来验证二维码是否有效(有时候logo的大小不合适会造成生成的二维码无效)。
    /// </summary>
    public partial class MainWindow : Window
    {
        private Bitmap bimg = null; //保存生成的二维码，方便后面保存
        //private string qrCodeImagePath = string.Empty; //存储打开的二维码的路径，共后面解码使用
        private string logoImagepath = string.Empty; //存储Logo的路径

        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        /// <summary>
        /// 初始化页面，为Combobox添加数据源和默认选中项
        /// </summary>
        public void Init()
        {
            for (int i = 1; i <= 40; i++)
            {
                cboVersion.Items.Add(i);
            }
            cboVersion.SelectedIndex = 6;
        }

        /// <summary>
        /// 生成二维码，如果有Logo，则在二维码中添加Logo
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public Bitmap CreateQRCode(string content)
        {
            QRCodeEncoder qrEncoder = new QRCodeEncoder();
            qrEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrEncoder.QRCodeScale = Convert.ToInt32(txtSize.Text);
            qrEncoder.QRCodeVersion = Convert.ToInt32(cboVersion.SelectedValue);
            qrEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            try
            {
                Bitmap qrcode = qrEncoder.Encode(content, Encoding.UTF8);
                if (!logoImagepath.Equals(string.Empty))
                {
                    Graphics g = Graphics.FromImage(qrcode);
                    Bitmap bitmapLogo = new Bitmap(logoImagepath);
                    int logoSize = Convert.ToInt32(txtLogoSize.Text);
                    bitmapLogo = new Bitmap(bitmapLogo, new System.Drawing.Size(logoSize, logoSize));
                    PointF point = new PointF(qrcode.Width / 2 - logoSize / 2, qrcode.Height / 2 - logoSize / 2);
                    g.DrawImage(bitmapLogo, point);
                }
                return qrcode;
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("超出当前二维码版本的容量上限，请选择更高的二维码版本！", "系统提示");
                return new Bitmap(100, 100);
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成二维码出错！", "系统提示");
                return new Bitmap(100, 100);
            }
        }

        /// <summary>
        /// 显示生成的二维码
        /// </summary>
        public void ShowQRCode()
        {
            if (txtQRCodeContent.Text.Trim().Length <= 0)
            {
                MessageBox.Show("二维码内容不能为空，请输入内容！", "系统提示");
                txtQRCodeContent.Focus();
                return;
            }
            bimg = CreateQRCode(txtQRCodeContent.Text);
            imgQRcode.Source = BitmapToBitmapImage(bimg);
            ResetImageStrethch(imgQRcode, bimg);
        }

        /// <summary>
        /// 将Bitmap转换成BitmapImage,使其能够在Image控件中显示
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            BitmapImage bImage = new BitmapImage();
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bImage.BeginInit();
            bImage.StreamSource = new MemoryStream(ms.ToArray());
            bImage.EndInit();
            return bImage;
        }

        /// <summary>
        /// 重置Image的Strethch属性
        /// 当图片小于size时显示原始大小
        /// 当图片大于size时，缩小图片比例，让图片全部显示出来
        /// </summary>
        /// <param name="img"></param>
        /// <param name="size"></param>
        private void ResetImageStrethch(System.Windows.Controls.Image img, Bitmap bImg)
        {
            if (bImg.Width <= img.Width)
            {
                img.Stretch = Stretch.None;
            }
            else
            {
                img.Stretch = Stretch.Fill;
            }
        }

        /// <summary>
        /// 保存二维码，并为二维码添加白色背景。
        /// </summary>
        /// <param name="path"></param>
        public void SaveQRCode(string path)
        {
            if (bimg != null)
            {
                Bitmap bitmap = new Bitmap(bimg.Width + 30, bimg.Height + 30);
                Graphics g = Graphics.FromImage(bitmap);
                g.FillRectangle(System.Drawing.Brushes.White, 0, 0, bitmap.Width, bitmap.Height);
                g.DrawImage(bimg, new PointF(15, 15));
                bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// 生成按钮处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateQRCodeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowQRCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("生成二维码出错！", "系统提示");
                return;
            }
        }

        /// <summary>
        /// 保存二维码,将二维码保存为Png格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Png文件(*.Png)|*.png|All files(*.*)|*.*";
            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    SaveQRCode(saveDialog.FileName);
                    MessageBox.Show("保存成功！", "系统提示");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存二维码出错！", "系统提示");
                    return;
                }
            }
        }

        /// <summary>
        /// 添加Logo按钮处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddLogoClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "图片文件|*.jpg;*.png;*.gif|All files(*.*)|*.*";
            if (openDialog.ShowDialog() == true)
            {
                logoImagepath = openDialog.FileName;
                Bitmap bImg = new Bitmap(logoImagepath);
                imgLogo.Source = new BitmapImage(new Uri(openDialog.FileName));
                ResetImageStrethch(imgLogo, bImg);
            }
        }

        /// <summary>
        /// 解析二维码按钮处理事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DecodeQRCodeClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DecodeQRCode();
            }
            catch (Exception ex)
            {
                MessageBox.Show("解析二维码出错！", "系统提示");
                return;
            }
        }

        /// <summary>
        /// 解析二维码
        /// </summary>
        public void DecodeQRCode()
        {
            if (bimg == null)
            {
                MessageBox.Show("请先打开一张二维码图片！", "系统提示");
                return;
            }
            QRCodeDecoder qrDecoder = new QRCodeDecoder();
            QRCodeImage qrImage = new QRCodeBitmapImage(bimg);
            tbDecodeResult.Text = qrDecoder.decode(qrImage, Encoding.UTF8);
        }

        /// <summary>
        /// 将验证码解析结果复制到剪切板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(tbDecodeResult.Text);
            MessageBox.Show("复制成功！", "系统提示");
        }

        /// <summary>
        /// 打开二维码图片，并显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenQRCode_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "图片文件|*.jpg;*.png;*.gif|All files(*.*)|*.*";
            if (openDialog.ShowDialog() == true)
            {
                bimg = new Bitmap(openDialog.FileName);
                imgQRcode.Source = new BitmapImage(new Uri(openDialog.FileName));
                ResetImageStrethch(imgQRcode, bimg);
            }
        }
    }
}

