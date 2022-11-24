using ChartControl;
using ImageProcessor;
using SurfaceFiller.Components;

namespace ColorSeparator
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new();
        private Charter charter = new(FormConstants.ChartSize, FormConstants.ChartMargin);

        public MainForm()
        {
            InitializeComponent();
            InitializeToolbar();
            ArrangeComponents();

            CMYKCurveGenerator.GenerateSample(this.charter);
        }

        private void InitializeComponent()
        {
            this.Text = Resources.ProgramTitle;
            this.MinimumSize = new Size(FormConstants.MinimumWidth, FormConstants.MinimumHeight);
            this.Size = new Size(FormConstants.InitialWidth, FormConstants.InitialHeight);
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
        }

        private void ArrangeComponents()
        {
            this.mainTableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.mainTableLayout.ColumnCount = FormConstants.MainFormColumnCount;

            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Controls.Add(this.charter, 1, 0);
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
        #endregion
    }
}