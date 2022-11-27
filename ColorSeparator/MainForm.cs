using ChartControl;
using ColorSeparatorApp;
using ImageProcessor;
using SurfaceFiller.Components;
using System.Drawing.Imaging;

namespace ColorSeparator
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new();
        private Charter charter = new(FormConstants.ChartMargin) { Dock = DockStyle.Fill };
        private ImageMng imageMng;
        private PictureSampler imagePreview = new() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage };
        private SampleViewer cyanPreview = new() { Dock = DockStyle.Fill };
        private SampleViewer magentaPreview = new() { Dock = DockStyle.Fill };
        private SampleViewer yellowPreview = new() { Dock = DockStyle.Fill };
        private SampleViewer blackPreview = new() { Dock = DockStyle.Fill };


        public MainForm()
        {
            InitializeComponent();
            InitializeToolbar();
            ArrangeComponents();
        }

        private void ParametersChangedHandler()
        {
            ReloadAllSamples();
        }

        private void CurveChangedHandler(object obj)
        {
            CurveId curveId = (CurveId)obj;

            switch (curveId)
            {
                case CurveId.Cyan:
                    ReloadSample(cyanPreview, CurveId.Cyan);
                    break;
                case CurveId.Magenta:
                    ReloadSample(magentaPreview, CurveId.Magenta);
                    break;
                case CurveId.Yellow:
                    ReloadSample(yellowPreview, CurveId.Yellow);
                    break;
                case CurveId.Black:
                    ReloadSample(blackPreview, CurveId.Black);
                    break;
            }
        }

        private void ReloadSample(SampleViewer sampleViewer, CurveId curveId)
        {
            //this.imageMng.GenerateSeparateImageAsync(sampleViewer, curveId);
            this.imageMng.GenerateSeparateImage(sampleViewer, curveId);
        }

        private void ReloadAllSamples()
        {
            ReloadSample(cyanPreview, CurveId.Cyan);
            ReloadSample(magentaPreview, CurveId.Magenta);
            ReloadSample(yellowPreview, CurveId.Yellow);
            ReloadSample(blackPreview, CurveId.Black);
        }

        private void InitializeComponent()
        {
            this.Text = Resources.ProgramTitle;
            this.MinimumSize = new Size(FormConstants.MinimumWidth, FormConstants.MinimumHeight);
            this.Size = new Size(FormConstants.InitialWidth, FormConstants.InitialHeight);

            this.imageMng = new(this.imagePreview, this.charter);
            this.imageMng.ParametersChanged += ParametersChangedHandler;
            this.charter.CurveChanged += CurveChangedHandler;
        }

        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.toolbar.CreateNewRadioBox();
            this.toolbar.AddRadioOption(CyanRadioHandler, Labels.Cyan, string.Empty, true);
            this.toolbar.AddRadioOption(MagentaRadioHandler, Labels.Megenta);
            this.toolbar.AddRadioOption(YellowRadioHandler, Labels.Yellow);
            this.toolbar.AddRadioOption(BlackRadioHandler, Labels.Black);
            this.toolbar.AddOption(ShowAllCurvesHandler, Labels.ShowAllCurves);
            this.toolbar.AddButton(LoadImageHandler, Glyphs.File);
            this.toolbar.AddSlider(ThreadsSlider, "T", 0.5f);
            this.toolbar.AddSlider(RetractionSlider, "Retraction", 1f);
        }

        private void RetractionSlider(float value)
        {
            this.imageMng.Retraction = value;
        }

        private void ThreadsSlider(float value)
        {
            this.imageMng.RenderThreads = (int)(value * 100);
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
            mainPanelTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            mainPanelTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            mainPanelTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

            mainPanelTable.RowStyles.Clear();
            mainPanelTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            mainPanelTable.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

            mainPanelTable.Controls.Add(this.charter, 0, 0);
            mainPanelTable.Controls.Add(this.imagePreview, 0, 1);
            mainPanelTable.Controls.Add(this.cyanPreview, 1, 0);
            mainPanelTable.Controls.Add(this.magentaPreview, 1, 1);
            mainPanelTable.Controls.Add(this.yellowPreview, 2, 0);
            mainPanelTable.Controls.Add(this.blackPreview, 2, 1);


            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Controls.Add(mainPanelTable, 1, 0);
            this.mainTableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainTableLayout);
        }

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
                    this.imagePreview.LoadImage(openFileDialog.FileName);
                    ReloadAllSamples();
                }
            }
        }
        #endregion
    }
}