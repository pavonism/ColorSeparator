using ChartControl;
using ColorSeparatorApp.Components;
using ImageProcessor;
using SurfaceFiller.Components;
using System.Drawing.Imaging;

namespace ColorSeparator
{
    public partial class MainForm : Form
    {
        #region Managers
        private ImageMng imageMng;
        #endregion

        #region Controls
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new();
        private Charter charter;
        private PictureSampler imagePreview;
        private PictureSampler cyanPreview;
        private PictureSampler magentaPreview;
        private PictureSampler yellowPreview;
        private PictureSampler blackPreview;
        #endregion Controls

        #region Initialize
        public MainForm()
        {
            InitializeComponent();
            InitializeToolbar();
            ArrangeComponents();
            FinishInitialization();
        }

        private void FinishInitialization()
        {
        }

        private void InitializeComponent()
        {
            this.Text = Resources.ProgramTitle;
            this.MinimumSize = new Size(FormConstants.MinimumWidth, FormConstants.MinimumHeight);
            this.Size = new Size(FormConstants.InitialWidth, FormConstants.InitialHeight);

            this.charter = new(FormConstants.ChartMargin) { Dock= DockStyle.Top };
            this.imageMng = new();
            this.imagePreview = new(this.imageMng);
            this.cyanPreview = new(this.imageMng, CurveId.Cyan);
            this.magentaPreview = new(this.imageMng, CurveId.Magenta);
            this.yellowPreview = new(this.imageMng, CurveId.Yellow);
            this.blackPreview = new(this.imageMng, CurveId.Black);
            this.imageMng.InitializeWithChart(this.charter);
        }

        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.charter.Height = this.charter.Width = this.toolbar.Width;
            this.toolbar.Controls.Add(this.charter);
            this.toolbar.AddButton(SaveCurvesHandler, Glyphs.Save);
            this.toolbar.AddButton(LoadCurvesHandler, Glyphs.Chart);
            this.toolbar.AddDivider();
            this.toolbar.CreateNewRadioBox();
            this.toolbar.AddRadioOption(CyanRadioHandler, Labels.Cyan, string.Empty, true);
            this.toolbar.AddRadioOption(MagentaRadioHandler, Labels.Megenta);
            this.toolbar.AddRadioOption(YellowRadioHandler, Labels.Yellow);
            this.toolbar.AddRadioOption(BlackRadioHandler, Labels.Black);
            this.toolbar.AddOptionToBox(ShowAllCurvesHandler, Labels.ShowAllCurves);
            this.toolbar.AddDivider();
            this.toolbar.AddButton(LoadImageHandler, Glyphs.File);
            this.toolbar.AddButton(SaveImagesHandler, Glyphs.Save);
            this.toolbar.AddSlider(ThreadsSliderHandler, "T", 0.5f);
            this.toolbar.AddSlider(RetractionSliderHandler, "Retraction", 1f);
        }

        private void LoadCurvesHandler(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;

                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Json Files(*.json)|*.json";
                openFileDialog.Filter = codecFilter;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.charter.LoadFromFile(openFileDialog.FileName);
                }
            }
        }

        private void SaveCurvesHandler(object? sender, EventArgs e)
        {
            SaveFileDialog saveCurvesDialog = new SaveFileDialog();
            saveCurvesDialog.Filter = "Json|*.json";
            saveCurvesDialog.ShowDialog();

            if (!string.IsNullOrEmpty(saveCurvesDialog.FileName))
            {
                this.charter.SaveAsFile(saveCurvesDialog.FileName);
            }
        }

        private void ArrangeComponents()
        {
            this.mainTableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.mainTableLayout.ColumnCount = FormConstants.MainFormColumnCount;

            var mainPanelTable = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
            };

            mainPanelTable.ColumnStyles.Clear();
            mainPanelTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 66));
            mainPanelTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            var cmykPanel = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
            };

            cmykPanel.RowStyles.Clear();
            cmykPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            cmykPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            cmykPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            cmykPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

            mainPanelTable.Controls.Add(this.imagePreview, 0, 0);
            cmykPanel.Controls.Add(this.cyanPreview, 0, 0);
            cmykPanel.Controls.Add(this.magentaPreview, 0, 1);
            cmykPanel.Controls.Add(this.yellowPreview, 0, 2);
            cmykPanel.Controls.Add(this.blackPreview, 0, 3);
            mainPanelTable.Controls.Add(cmykPanel, 1, 0);

            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Controls.Add(mainPanelTable, 1, 0);
            this.mainTableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainTableLayout);
        }

        #endregion Initialize

        #region Handlers
        private void CyanRadioHandler(object? sender, EventArgs e)
        {
            this.charter.SelectCurve(CurveId.Cyan);
        }
        private void MagentaRadioHandler(object? sender, EventArgs e)
        {
            this.charter.SelectCurve(CurveId.Magenta);
        }
        private void YellowRadioHandler(object? sender, EventArgs e)
        {
            this.charter.SelectCurve(CurveId.Yellow);
        }
        private void BlackRadioHandler(object? sender, EventArgs e)
        {
            this.charter.SelectCurve(CurveId.Black);
        }
        private void ShowAllCurvesHandler(object? sender, EventArgs e)
        {
            this.charter.ShowAll = !this.charter.ShowAll;
        }
        private void LoadImageHandler(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;

                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Image Files|";
                foreach (var codec in codecs)
                {
                    codecFilter += codec.FilenameExtension + ";";
                }
                openFileDialog.Filter = codecFilter;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.imagePreview.ImagePath = openFileDialog.FileName;
                    this.cyanPreview.SourceImage = this.imagePreview.SourceImage;
                    this.magentaPreview.SourceImage = this.imagePreview.SourceImage;
                    this.yellowPreview.SourceImage = this.imagePreview.SourceImage;
                    this.blackPreview.SourceImage = this.imagePreview.SourceImage;
                }
            }
        }
        private void RetractionSliderHandler(float value)
        {
            this.imageMng.Retraction = value;
        }

        private void ThreadsSliderHandler(float value)
        {
            this.imageMng.RenderThreads = (int)(value * 100);
        }

        private void SaveImagesHandler(object? sender, EventArgs e)
        {
            if (this.imagePreview.SourceImage == null)
                return;

            SaveFileDialog saveCurvesDialog = new SaveFileDialog();
            saveCurvesDialog.ShowDialog();

            if (!string.IsNullOrEmpty(saveCurvesDialog.FileName))
            {
                this.imageMng.SaveAll(this.imagePreview.SourceImage, saveCurvesDialog.FileName, Path.GetExtension(this.imagePreview.ImagePath));
            }
        }
        #endregion
    }
}