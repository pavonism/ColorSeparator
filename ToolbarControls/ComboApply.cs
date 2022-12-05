using SurfaceFiller.Components;
using System.Windows.Forms;

namespace ToolbarControls
{
    internal class ComboApply<Combo, T> : TableLayoutPanel where Combo : ComboPicker<T>, new()
    {
        private Combo comboBox = new() { Dock = DockStyle.Fill };
        private RoundedButton button = new() { Dock = DockStyle.Fill };

        public Action<T>? Apply;

        private string hint;
        public string Hint
        {
            get => this.hint;
            set
            {
                this.hint = value;
                var tooltip = new ToolTip();
                tooltip.SetToolTip(button, hint);
            }
        }

        public ComboApply(IEnumerable<T> options, T defaultOption)
            : this(options)
        {
            this.comboBox.DefaultValue = defaultOption;
        }

        public ComboApply(IEnumerable<T> options)
        {
            this.comboBox.AddOptions(options);
            this.comboBox.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            this.button.Font = this.comboBox.Font;
            this.button.Text = "Apply";
            this.button.BackColor = Color.Transparent;
            this.button.ForeColor = Color.White;
            this.button.Margin = new Padding(10, 8, 10, 8);
            Margin = Padding.Empty;
            Padding = Padding.Empty;
            Controls.Add(comboBox, 0, 0);
            Controls.Add(button, 1, 0);

            this.button.Click += ClickHandler;
        }

        private void ClickHandler(object? sender, EventArgs e)
        {
            this.Apply?.Invoke((T)this.comboBox.SelectedItem);
        }
    }
}
