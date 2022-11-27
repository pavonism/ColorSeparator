using FastBitmap;
using System.Text.Json;

namespace ChartControl
{
    public partial class Charter : PictureBox
    {
        private DirectBitmap directBitmap;
        private Dictionary<CurveId, Curve> curves;
        private ControlPoint? selectedPoint;
        private int chartSize;
        private int margin;
        private CurveId? selectedCurve;

        public event Action<CurveId?>? CurveChanged;

        private bool showAll;
        public bool ShowAll
        {
            get => this.showAll;
            set
            {
                this.showAll = value;

                foreach (var curveId in this.curves.Keys)
                {
                    if (!curveId.Equals(selectedCurve))
                        this.curves[curveId].Visible = value;
                }

                Refresh();
            }
        }

        public Charter(int margin)
        {
            this.margin = margin;
            this.curves = new();

            ReInitialize();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            ReInitialize();
        }

        private void ReInitialize()
        {
            this.directBitmap?.Dispose();
            int size = Math.Min(this.Width, this.Height);
            this.directBitmap = new(size, size);
            this.chartSize = size - margin * 2;

            this.Image = this.directBitmap.Bitmap;
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            foreach (var curve in this.curves.Values)
            {
                if(curve.TrySelectPoint(e.X - margin, e.Y - margin, this.chartSize, out var point))
                {
                    selectedPoint = point;
                    break;
                }
            }

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if(this.selectedPoint != null)
            {
                var newX = Math.Min(Math.Max(e.X, margin), chartSize + margin) - margin;
                var newY = Math.Min(Math.Max(2*margin + chartSize - e.Y, margin), chartSize + margin) - margin;

                this.selectedPoint.MoveTo(newX, newY);
                Refresh();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.selectedPoint = null;
        }

        public override void Refresh()
        {
            Clear();

            foreach (var curve in this.curves.Values)
            {
                curve.Render(this.directBitmap, chartSize, margin);
            }

            DrawMargin();
            base.Refresh();
        }

        public float GetCurveValueAt(CurveId curveId, float k)
        {
            if(this.curves.TryGetValue(curveId, out var curve))
            {
                return curve.GetValueAt(k);
            }

            return 0;
        }
        public float[] GetCurveValues(CurveId curveId)
        {
            if (this.curves.TryGetValue(curveId, out var curve))
            {
                return curve.Values;
            }

            throw new ArgumentException();
        }


        private void DrawMargin()
        {
            using (var g = Graphics.FromImage(directBitmap.Bitmap))
            {
                g.DrawRectangle(Pens.Black, margin, margin, chartSize, chartSize);
            }
        }

        public void AddCurve(Curve curve)
        {
            this.curves.Add(curve.Id, curve);
            curve.CurveChanged += CurveChangedHandler;
        }

        private void CurveChangedHandler(CurveId curveId)
        {
            this.CurveChanged?.Invoke(curveId);
        }


        public void HideAllConstructionPoints()
        {
            foreach (var curve in this.curves.Values)
            {
                curve.ShowControlPoints = false;
            }
        }

        public void SelectCurve(CurveId curveId) 
        {
            this.selectedCurve = curveId;

            if(!ShowAll)
                ShowAll = false;

            HideAllConstructionPoints();

            if (this.curves.TryGetValue(curveId, out var curve))
            {
                curve.Visible = true;
                curve.ShowControlPoints = true;
            }

            Refresh();
        }

        public void Clear()
        {
            using (var g = Graphics.FromImage(directBitmap.Bitmap))
            {
                g.Clear(Color.LightGray);
            }
        }

        public void Reset()
        {
            this.curves.Clear();
        }

        public void SaveAsFile(string path)
        {
            using (var fs = new StreamWriter(path))
            {
                var serialized = JsonSerializer.Serialize(this.curves, new JsonSerializerOptions() { Converters = { new ColorJsonConverter() } });
                fs.Write(serialized);
            }
        }

        public void LoadFromFile(string path)
        {
            using (var fs = new StreamReader(path))
            {
                var serialized = fs.ReadToEnd();
                var deserialized = JsonSerializer.Deserialize(serialized, typeof(Dictionary<CurveId, Curve>), new JsonSerializerOptions() { Converters = {new ColorJsonConverter()}}) as Dictionary<CurveId, Curve>;

                if (deserialized != null)
                {
                    this.curves.Clear();
                    foreach (var pair in deserialized)
                    {
                        pair.Value.CurveChanged += CurveChangedHandler;
                    }

                    this.curves = deserialized;
                    if(selectedCurve.HasValue)
                        SelectCurve(selectedCurve.Value);
                    Refresh();
                    this.CurveChanged?.Invoke(null);
                }
            }
        }
    }
}
