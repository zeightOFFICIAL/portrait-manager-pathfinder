﻿/*    
    Pathfinder Portrait Manager. Desktop application for managing in game
    portraits for Pathfinder: Kingmaker and Pathfinder: Wrath of the Righteous
    Copyright (C) 2023 Artemii "Zeight" Saganenko
    LICENSE terms are written in LICENSE file
    Primal license header is written in Program.cs
*/

using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PathfinderPortraitManager
{
    public partial class MainForm : Form
    {
        public void RestoreFilePage()
        {
            _isAnyLoaded = false;
            _imageFlag = 0;
            ButtonNextImageType.Visible = true;
            ButtonNextImageType.Enabled = true;
            ButtonNextImageType.Text = Properties.TextVariables.BUTTON_ADVANCED;
        }
        public void AddClickEventsFromMainButtons()
        {
            RootFunctions.AddClickEvent(ButtonToFilePage, ButtonToFilePage_Click);
            RootFunctions.AddClickEvent(ButtonToExtractPage, ButtonToExtract_Click);
            RootFunctions.AddClickEvent(ButtonToGalleryPage, ButtonToGalleryPage_Click);
        }
        public void RemoveClickEventsFromMainButtons()
        {
            RootFunctions.RemoveClickEvent(ButtonToFilePage, ButtonToFilePage_Click);
            RootFunctions.RemoveClickEvent(ButtonToExtractPage, ButtonToExtract_Click);
            RootFunctions.RemoveClickEvent(ButtonToGalleryPage, ButtonToGalleryPage_Click);
        }
        public void ClearPrimeImages(Image replacement)
        {
            ImageControl.Utils.Replace(PicPortraitTemp, replacement);
            ImageControl.Utils.Replace(PicPortraitLrg, replacement);
            ImageControl.Utils.Replace(PicPortraitMed, replacement);
            ImageControl.Utils.Replace(PicPortraitSml, replacement);
        }
        public void DisposePrimeImages()
        {
            ImageControl.Utils.Dispose(PicPortraitTemp);
            ImageControl.Utils.Dispose(PicPortraitLrg);
            ImageControl.Utils.Dispose(PicPortraitMed);
            ImageControl.Utils.Dispose(PicPortraitSml);
        }
        public void ResizeImageToParent(Control control, Image image, Control parent)
        {
            float aspect = control.Height * 1.0f / control.Width * 1.0f;
            Tuple<int, int> newSize = MapNewSize(parent, aspect);
            if (control is PictureBox pictureBox)
            {
                pictureBox.Image = ImageControl.Direct.Resize(image, newSize.Item1, newSize.Item2);
            }
            ArrangeAutoScroll(parent, newSize.Item1, newSize.Item2);
        }
        public void ResizeVisibleImagesToWindow()
        {
            if (LayoutScalePage.Enabled == true)
            {
                using (Image img = new Bitmap(TEMP_SMALL_APPEND))
                    ResizeImageToParent(PicPortraitSml, img, PanelPortraitSml);
                using (Image img = new Bitmap(TEMP_MEDIUM_APPEND))
                    ResizeImageToParent(PicPortraitMed, img, PanelPortraitMed);
                using (Image img = new Bitmap(TEMP_LARGE_APPEND))
                    ResizeImageToParent(PicPortraitLrg, img, PanelPortraitLrg);
            }
            if (LayoutFilePage.Enabled == true)
            {
                using (Image img = new Bitmap(PicPortraitTemp.Image))
                    ResizeImageToParent(PicPortraitTemp, img, PanelPortraitTemp);
            }
        }
        public static Tuple<int, int> MapNewSize(Control parent, float aspect)
        {
            int inWidth = parent.Width,
                inHeight = parent.Height,
                outWidth,
                outHeight;
            outHeight = inHeight;
            outWidth = (int)(inHeight * 1.0f / aspect * 1.0f);
            if (outWidth < parent.Width)
            {
                outWidth = inWidth;
                outHeight = (int)(inWidth * 1.0f / (1.0f / aspect * 1.0f));
            }
            return Tuple.Create(outWidth, outHeight);
        }
        public void ParentLayoutsDisable()
        {
            RootFunctions.LayoutDisable(LayoutFilePage);
            RootFunctions.LayoutDisable(LayoutMainPage);
            RootFunctions.LayoutDisable(LayoutScalePage);
            RootFunctions.LayoutDisable(LayoutExtractPage);
            RootFunctions.LayoutDisable(LayoutGallery);
            RootFunctions.LayoutDisable(LayoutSettingsPage);
            RootFunctions.LayoutDisable(LayoutCustom);
        }
        public void ParentLayoutsSetDockFill()
        {
            foreach (Control control in Controls)
            {
                RootFunctions.LayoutsSetDockFill(control);
            }
        }
        class RootFunctions
        {
            static public void AddClickEvent(object sender, EventHandler handler)
            {
                if (sender is Button button)
                {
                    if (button != null)
                    {
                        button.Click -= handler;
                        button.Click += handler;
                    }
                }
            }
            static public void RemoveClickEvent(object sender, EventHandler handler)
            {
                if (sender is Button button)
                {
                    if (button != null)
                    {
                        button.Click -= handler;
                    }
                }
            }
            static public void LayoutEnable(TableLayoutPanel table)
            {
                if (table.Visible == false && table.Enabled == false)
                {
                    table.Visible = true;
                    table.Enabled = true;
                }
            }
            static public void LayoutDisable(TableLayoutPanel table)
            {
                if (table.Visible == true && table.Enabled == true)
                {
                    table.Visible = false;
                    table.Enabled = false;
                }
            }
            static public void LayoutsSetDockFill(Control control)
            {
                if (control is TableLayoutPanel)
                {
                    control.Dock = DockStyle.Fill;
                }
                foreach (Control subCtrl in control.Controls)
                {
                    LayoutsSetDockFill(subCtrl);
                }
            }
            static public void HideScrollBar(Control control)
            {
                if (control is Panel panel)
                {
                    if (panel != null)
                    {
                        panel.VerticalScroll.Visible = false;
                        panel.HorizontalScroll.Visible = false;
                        panel.AutoScroll = false;
                    }
                }
            }
        }
        public void LoadAllTempImages()
        {
            LoadTempImages(200);
        }
        public void LoadTempImages(ushort flag)
        {
            if (flag == 0 || flag == 100)
            {
                using (Image img = new Bitmap(TEMP_LARGE_APPEND))
                    ImageControl.Utils.Replace(PicPortraitTemp, new Bitmap(img));
            }
            else if (flag == 1)
            {
                using (Image img = new Bitmap(TEMP_MEDIUM_APPEND))
                    ImageControl.Utils.Replace(PicPortraitTemp, new Bitmap(img));
            }
            else if (flag == 2)
            {
                using (Image img = new Bitmap(TEMP_SMALL_APPEND))
                    ImageControl.Utils.Replace(PicPortraitTemp, new Bitmap(img));
            }
            else if (flag == 200)
            {
                using (Image img = new Bitmap(TEMP_SMALL_APPEND))
                    ImageControl.Utils.Replace(PicPortraitSml, new Bitmap(img));
                using (Image img = new Bitmap(TEMP_MEDIUM_APPEND))
                    ImageControl.Utils.Replace(PicPortraitMed, new Bitmap(img));
                using (Image img = new Bitmap(TEMP_LARGE_APPEND))
                    ImageControl.Utils.Replace(PicPortraitLrg, new Bitmap(img));
                ArrangeAutoScroll(PanelPortraitLrg, PicPortraitLrg.Height, PicPortraitLrg.Width);
                ArrangeAutoScroll(PanelPortraitMed, PicPortraitLrg.Height, PicPortraitLrg.Width);
                ArrangeAutoScroll(PanelPortraitSml, PicPortraitLrg.Height, PicPortraitLrg.Width);
            }
        }
        public static void ArrangeAutoScroll(Control control, int xMax, int yMax)
        {
            if (control is Panel panel)
            {
                panel.AutoScroll = false;
                panel.VerticalScroll.Minimum = 0;
                panel.HorizontalScroll.Minimum = 0;
                panel.VerticalScroll.Maximum = xMax;
                panel.HorizontalScroll.Maximum = yMax;
                panel.VerticalScroll.Visible = true;
                panel.HorizontalScroll.Visible = true;
            }
        }
        public static bool CheckPortraitExistence(string path)
        {
            if (SystemControl.FileControl.Readonly.DirectoryExists(path))
                if (SystemControl.FileControl.Readonly.FileExist(path + LARGE_APPEND) &&
                    SystemControl.FileControl.Readonly.FileExist(path + MEDIUM_APPEND) &&
                    SystemControl.FileControl.Readonly.FileExist(path + SMALL_APPEND))
                    if (SystemControl.FileControl.Readonly.GetFileExtension(path + LARGE_APPEND) == ".png" &&
                        SystemControl.FileControl.Readonly.GetFileExtension(path + MEDIUM_APPEND) == ".png" &&
                        SystemControl.FileControl.Readonly.GetFileExtension(path + SMALL_APPEND) == ".png")
                        return true;
                    else
                        return false;
                else
                    return false;
            else
                return false;
        }
        public void ExploreDirectory(string path)
        {
            if (!CheckPortraitExistence(path) ||
                SystemControl.FileControl.Readonly.CheckImagePixeling(path + LARGE_APPEND, 692, 1024) &&
                SystemControl.FileControl.Readonly.CheckImagePixeling(path + MEDIUM_APPEND, 330, 432) &&
                SystemControl.FileControl.Readonly.CheckImagePixeling(path + SMALL_APPEND, 185, 242))
            {
                return;
            }

            int pathHops = path.Split('\\').Length;
            string folderName = path.Split('\\')[pathHops - 1];
            try
            {
                using (Image img = new Bitmap(path + "\\Fulllength.png"))
                {
                    ImgListExtract.Images.Add(path, img);
                }
            }
            catch
            {
                return;
            }
            ListViewItem item = new ListViewItem
            {
                Text = folderName,
                ImageIndex = ListExtract.Items.Count
            };
            ListExtract.Items.Add(item);
 
            string[] subDirs = Directory.GetDirectories(path);
            foreach (string subDir in subDirs)
            {
                ExploreDirectory(subDir);
            }
        }
        public void SetFonts(PrivateFontCollection fonts)
        {
            Font _bebas_neue20 = new Font(fonts.Families[0], 20),
                 _bebas_neue16 = new Font(fonts.Families[0], 16);
            ButtonToFilePage.Font = _bebas_neue20;
            ButtonToExtractPage.Font = _bebas_neue20;
            ButtonToGalleryPage.Font = _bebas_neue20;
            ButtonToSettingsPage.Font = _bebas_neue20;
            ButtonExit.Font = _bebas_neue20;
            ButtonKingmaker.Font = _bebas_neue20;
            ButtonWotR.Font = _bebas_neue20;
            LabelSelectedPath.Font = _bebas_neue16;
            ButtonValidatePath.Font = _bebas_neue16;
            ButtonSelectPath.Font = _bebas_neue16;
            ButtonToMainPage5.Font = _bebas_neue20;
            LabelSettings.Font = _bebas_neue16;
            ButtonApplyChange.Font = _bebas_neue16;
            ButtonLocalPortraitLoad.Font = _bebas_neue20;
            ButtonWebPortraitLoad.Font = _bebas_neue20;
            ButtonToMainPage.Font = _bebas_neue20;
            ButtonToScalePage.Font = _bebas_neue20;
            ButtonNextImageType.Font = _bebas_neue20;
            ButtonHintOnFilePage.Font = _bebas_neue20;
            ButtonToFilePage2.Font = _bebas_neue20;
            ButtonCreatePortrait.Font = _bebas_neue20;
            LabelMedImage.Font = _bebas_neue20;
            LabelLrgImg.Font = _bebas_neue20;
            LabelSmlImg.Font = _bebas_neue20;
            ButtonHintOnScalePage.Font = _bebas_neue20;
            ButtonDeletePortait.Font = _bebas_neue20;
            ButtonToMainPage3.Font = _bebas_neue20;
            ButtonOpenFolder.Font = _bebas_neue20;
            ButtonChangePortrait.Font = _bebas_neue20;
            ButtonHintFolder.Font = _bebas_neue20;
            LabelURLInfo.Font = _bebas_neue20;
            ButtonDenyWeb.Font = _bebas_neue20;
            ButtonLoadWeb.Font = _bebas_neue20;
            LabelFinalMesg.Font = _bebas_neue20;
            ButtonToFilePage3.Font = _bebas_neue20;
            ButtonToMainPage4.Font = _bebas_neue20;
            ButtonToMainPageAndFolder.Font = _bebas_neue20;
            ButtonChooseFolder.Font = _bebas_neue20;
            ButtonExtractAll.Font = _bebas_neue20;
            ButtonExtractSelected.Font = _bebas_neue20;
            ButtonOpenFolders.Font = _bebas_neue20;
            ButtonHintExtract.Font = _bebas_neue20;
            ButtonToMainPage2.Font = _bebas_neue20;
            ButtonToCustomPortraits.Font = _bebas_neue20;
        }
        public void SetFontsNotEN()
        {
            Font defFont = new Font(DefaultFont.FontFamily, 12);
            ButtonToFilePage.Font = defFont;
            ButtonToExtractPage.Font = defFont;
            ButtonToGalleryPage.Font = defFont;
            ButtonToSettingsPage.Font = defFont;
            ButtonExit.Font = defFont;
            ButtonKingmaker.Font = defFont;
            ButtonWotR.Font = defFont;
            LabelSelectedPath.Font = defFont;
            ButtonValidatePath.Font = defFont;
            ButtonSelectPath.Font = defFont;
            ButtonToMainPage5.Font = defFont;
            LabelSettings.Font = defFont;
            ButtonApplyChange.Font = defFont;
            ButtonLocalPortraitLoad.Font = defFont;
            ButtonWebPortraitLoad.Font = defFont;
            ButtonToMainPage.Font = defFont;
            ButtonToScalePage.Font = defFont;
            ButtonNextImageType.Font = defFont;
            ButtonHintOnFilePage.Font = defFont;
            ButtonToFilePage2.Font = defFont;
            ButtonCreatePortrait.Font = defFont;
            LabelMedImage.Font = defFont;
            LabelLrgImg.Font = defFont;
            LabelSmlImg.Font = defFont;
            ButtonHintOnScalePage.Font = defFont;
            ButtonDeletePortait.Font = defFont;
            ButtonToMainPage3.Font = defFont;
            ButtonOpenFolder.Font = defFont;
            ButtonChangePortrait.Font = defFont;
            ButtonHintFolder.Font = defFont;
            LabelURLInfo.Font = defFont;
            ButtonDenyWeb.Font = defFont;
            ButtonLoadWeb.Font = defFont;
            LabelFinalMesg.Font = defFont;
            ButtonToFilePage3.Font = defFont;
            ButtonToMainPage4.Font = defFont;
            ButtonToMainPageAndFolder.Font = defFont;
            ButtonChooseFolder.Font = defFont;
            ButtonExtractAll.Font = defFont;
            ButtonExtractSelected.Font = defFont;
            ButtonOpenFolders.Font = defFont;
            ButtonHintExtract.Font = defFont;
            ButtonToMainPage2.Font = defFont;
            ButtonToCustomPortraits.Font = defFont;
        }
        public void SetTexts()
        {
            ButtonToFilePage.Text = Properties.TextVariables.BUTTON_TOFILEPAGE;
            ButtonToExtractPage.Text = Properties.TextVariables.BUTTON_TOEXRACTPAGE;
            ButtonToGalleryPage.Text = Properties.TextVariables.BUTTON_TOGALLERYPAGE;
            ButtonToSettingsPage.Text = Properties.TextVariables.BUTTON_TOSETTINGSPAGE;
            ButtonExit.Text = Properties.TextVariables.BUTTON_EXIT;
            ButtonKingmaker.Text = Properties.TextVariables.KING;
            ButtonWotR.Text = Properties.TextVariables.WOTR;
            LabelSelectedPath.Text = Properties.TextVariables.LABEL_PATH;
            ButtonValidatePath.Text = Properties.TextVariables.BUTTON_VALIDATE;
            ButtonSelectPath.Text = Properties.TextVariables.BUTTON_SELECTPATH;
            ButtonToMainPage5.Text = Properties.TextVariables.BUTTON_BACK;
            LabelSettings.Text = Properties.TextVariables.LABEL_SETTINGS;
            ButtonApplyChange.Text = Properties.TextVariables.BUTTON_APPLY;
            ButtonLocalPortraitLoad.Text = Properties.TextVariables.BUTTON_LOADLOCALPORTRAIT;
            ButtonWebPortraitLoad.Text = Properties.TextVariables.BUTTON_LOADWEBPORTRAIT;
            ButtonToMainPage.Text = Properties.TextVariables.BUTTON_BACK;
            ButtonToScalePage.Text = Properties.TextVariables.BUTTON_TOSCALEPAGE;
            ButtonNextImageType.Text = Properties.TextVariables.BUTTON_ADVANCED;
            ButtonHintOnFilePage.Text = Properties.TextVariables.BUTTON_HINT;
            ButtonToFilePage2.Text = Properties.TextVariables.BUTTON_BACK;
            ButtonCreatePortrait.Text = Properties.TextVariables.BUTTON_TOCREATE;
            LabelMedImage.Text = Properties.TextVariables.LABEL_MEDIUMIMG;
            LabelLrgImg.Text = Properties.TextVariables.LABEL_LARGEIMG;
            LabelSmlImg.Text = Properties.TextVariables.LABEL_SMALLIMG;
            ButtonHintOnScalePage.Text = Properties.TextVariables.BUTTON_HINT;
            ButtonDeletePortait.Text = Properties.TextVariables.BUTTON_SELECTEDDELETE;
            ButtonToMainPage3.Text = Properties.TextVariables.BUTTON_BACK;
            ButtonOpenFolder.Text = Properties.TextVariables.BUTTON_GALLERYOPENFOLDER;
            ButtonChangePortrait.Text = Properties.TextVariables.BUTTON_SELECTEDCHANGE;
            ButtonHintFolder.Text = Properties.TextVariables.BUTTON_HINT;
            LabelURLInfo.Text = Properties.TextVariables.LABEL_URLDIALOG;
            ButtonDenyWeb.Text = Properties.TextVariables.BUTTON_CANCEL;
            ButtonLoadWeb.Text = Properties.TextVariables.BUTTON_LOAD;
            TextBoxURL.Text = Properties.TextVariables.TEXTBOX_URL_INPUT;
            LabelFinalMesg.Text = Properties.TextVariables.LABEL_CREATEDOK;
            ButtonToFilePage3.Text = Properties.TextVariables.BUTTON_NEW;
            ButtonToMainPage4.Text = Properties.TextVariables.BUTTON_MENU;
            ButtonToMainPageAndFolder.Text = Properties.TextVariables.BUTTON_FINALOPENFOLDER;
            ButtonChooseFolder.Text = Properties.TextVariables.TEXT_TITLEOPENFOLDER;
            ButtonExtractAll.Text = Properties.TextVariables.BUTTON_EXTRACTALL;
            ButtonExtractSelected.Text = Properties.TextVariables.BUTTON_EXTRACTSELECTED;
            ButtonOpenFolders.Text = Properties.TextVariables.BUTTON_EXTRACTOPENFOLDER;
            ButtonHintExtract.Text = Properties.TextVariables.BUTTON_HINT;
            ButtonToMainPage2.Text = Properties.TextVariables.BUTTON_BACK;
            ButtonToCustomPortraits.Text = Properties.TextVariables.BUTTON_CUTSTOMPORT;
            LabelCopyright.Text = Properties.TextVariables.LABEL_COPY;
        }
        public void UpdateObjectColoring(Control ctrl, Color a, Color b)
        {
            if (ctrl is PictureBox || ctrl.Equals(LayoutURLDialog) 
                                   || ctrl.Equals(LayoutFinalPage)
                                   || ctrl.Equals(LayoutSettingsPage))
            {
                return;
            }
            if (ctrl.Equals(LabelCopyright) || ctrl.Equals(LabelVersion))
            {
                ctrl.ForeColor = a;
                return;
            }
            ctrl.ForeColor = a;
            ctrl.BackColor = b;
            foreach (Control subCtrl in ctrl.Controls)
            {
                UpdateObjectColoring(subCtrl, a, b);
            }
        }
        public void ReplacePrimeImagesToDefault()
        {
            using (Image placeholder = new Bitmap(GAME_TYPES[_gameSelected].PlaceholderImage))
            {
                ClearPrimeImages(placeholder);
            }
        }
        public static void SafeCopyAllImages(string newImagePath, ushort flag)
        {
            using (Image placeholder = new Bitmap(GAME_TYPES[_gameSelected].PlaceholderImage))
            {
                if (flag == 1)
                {
                    SystemControl.FileControl.DeleteFile(TEMP_MEDIUM_APPEND);
                    SystemControl.FileControl.CreateTempImages(newImagePath, TEMP_APPENDS, placeholder, flag);
                }
                else if (flag == 2)
                {
                    SystemControl.FileControl.DeleteFile(TEMP_SMALL_APPEND);
                    SystemControl.FileControl.CreateTempImages(newImagePath, TEMP_APPENDS, placeholder, flag);
                }
                else if (flag == 100 || flag == 0)
                {
                    SystemControl.FileControl.ClearTempImages();
                    SystemControl.FileControl.CreateTempImages(newImagePath, TEMP_APPENDS, placeholder, flag);
                }
            }
        }
        public void UpdateColorScheme()
        {
            Color foreColor = GAME_TYPES[_gameSelected].ForeColor;
            Color backColor = GAME_TYPES[_gameSelected].BackColor;
            Icon = GAME_TYPES[_gameSelected].GameIcon;
            PictureBoxTitle.BackgroundImage = GAME_TYPES[_gameSelected].TitleImage;
            LayoutMainPage.BackgroundImage = GAME_TYPES[_gameSelected].BackgroundImage;
            foreach (Control ctrl in Controls)
            {
                UpdateObjectColoring(ctrl, foreColor, backColor);
            }
            Text = GAME_TYPES[_gameSelected].TitleText;
            TextBoxFullPath.Clear();
            if (_gameSelected == 'p')
            {
                ButtonKingmaker.Enabled = false;
                ButtonKingmaker.ForeColor = backColor;
                ButtonKingmaker.BackColor = foreColor;
                ButtonWotR.Enabled = true;
                ButtonWotR.ForeColor = Color.White;
                ButtonWotR.BackColor = Color.Black;
                TextBoxFullPath.Text = Properties.CoreSettings.Default.KINGPath;
            }
            else
            {
                ButtonKingmaker.Enabled = true;
                ButtonKingmaker.ForeColor = Color.White;
                ButtonKingmaker.BackColor = Color.Black;
                ButtonWotR.Enabled = false;
                ButtonWotR.ForeColor = backColor;
                ButtonWotR.BackColor = foreColor;
                TextBoxFullPath.Text = Properties.CoreSettings.Default.WOTRPath;
            }
        }
        public bool LoadGallery(string fromPath)
        {
            if(!SystemControl.FileControl.Readonly.DirectoryExists(fromPath))
            {
                return false;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(() => {
                int count = 0;
                foreach (string dir in Directory.GetDirectories(fromPath))
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        Invoke((MethodInvoker)delegate {
                            ImgListGallery.Images.Clear();
                            ListGallery.Items.Clear();
                        });
                        count = 0;
                        return;
                    }

                    int pathHops = dir.Split('\\').Length;

                    using (Image img = new Bitmap(dir + "\\Fulllength.png"))
                        {
                            Invoke((MethodInvoker)delegate {
                                ImgListGallery.Images.Add(dir.Split('\\')[pathHops - 1], img);
                            });
                        }

                    ListViewItem item = new ListViewItem
                    {
                        Text = dir.Split('\\')[pathHops - 1],
                        ImageIndex = count
                    };

                    ListGallery.BeginInvoke((MethodInvoker)delegate {
                        ListGallery.Items.Add(item);
                    });

                    count++;
                }

            }, cancelToken);

            return true;
        }
        public static void ClearImageLists(ListView listView, ImageList imageList)
        {
            listView.Clear();
            imageList.Images.Clear();
        }
        public string ParseDragDropFile(DragEventArgs e)
        {
            string[] filesList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string filePath = "!NONE!";
            if (filesList[0] != null && File.Exists(filesList[0]) &&
                (Path.GetExtension(filesList[0]) == ".png") ||
                (Path.GetExtension(filesList[0]) == ".jpg") ||
                (Path.GetExtension(filesList[0]) == ".jpeg") ||
                (Path.GetExtension(filesList[0]) == ".bmp") ||
                (Path.GetExtension(filesList[0]) == ".gif"))
            {
                filePath = filesList[0];
            }
            return filePath;
        }
        public void CheckWebResourceAndLoad(string URL)
        {
            try
            {
                var request = System.Net.WebRequest.Create(URL);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    using (Image webImage = Image.FromStream(stream))
                    {
                        
                        _isAnyLoaded = true;
                        if (_imageFlag == 1)
                        {
                            SystemControl.FileControl.DeleteFile(TEMP_MEDIUM_APPEND);
                            webImage.Save(TEMP_MEDIUM_APPEND);
                        }
                        else if (_imageFlag == 2)
                        {
                            SystemControl.FileControl.DeleteFile(TEMP_SMALL_APPEND);
                            webImage.Save(TEMP_SMALL_APPEND);
                        }
                        else if (_imageFlag == 100 || _imageFlag == 0)
                        {
                            SystemControl.FileControl.ClearTempImages();
                            SystemControl.FileControl.CreateDirectory("temp_DoNotDeleteWhileRunning/");
                            webImage.Save(TEMP_MEDIUM_APPEND);
                            webImage.Save(TEMP_SMALL_APPEND);
                            webImage.Save(TEMP_LARGE_APPEND);
                        }
                        LoadTempImages(_imageFlag);
                        ResizeVisibleImagesToWindow();
                    }
                }
            }
            catch
            {
                using (forms.MyMessageDialog Message = new forms.MyMessageDialog(Properties.TextVariables.MESG_CANNOTLOAD))
                {
                    Message.StartPosition = FormStartPosition.CenterParent;
                    Message.ShowDialog();
                }
            }
        }
        public static bool ValidatePortraitPath(string path)
        {
            if (SystemControl.FileControl.Readonly.DirectoryExists(path) &&
                path.Split('\\').Last() == "Portraits")
            {
                return true;
            }
            return false;
        }
        public void GeneratePortraits(string path)
        {
            Directory.CreateDirectory(path);
            ImageControl.Wraps.CropImage(PicPortraitLrg, PanelPortraitLrg, TEMP_LARGE_APPEND,
                                         path + LARGE_APPEND, LARGE_ASPECT, 692, 1024);
            ImageControl.Wraps.CropImage(PicPortraitMed, PanelPortraitMed, TEMP_MEDIUM_APPEND,
                                         path + MEDIUM_APPEND, MEDIUM_ASPECT, 330, 432);
            ImageControl.Wraps.CropImage(PicPortraitSml, PanelPortraitSml, TEMP_SMALL_APPEND,
                                         path + SMALL_APPEND, SMALL_ASPECT, 185, 242);
        }
        public void GenerateImageFlagString(ushort flag = 0)
        {
            if (flag == 0)
            {
                LabelImageFlag.Text = "◼◼◼";
            }
            else if (flag == 1)
            {
                LabelImageFlag.Text = "◼◧◻";
            }
            else if (flag == 2)
            {
                LabelImageFlag.Text = "◼◼◧";
            }
            else if (flag == 100)
            {
                LabelImageFlag.Text = "◻◻◻";
            }
        }
    }
}
