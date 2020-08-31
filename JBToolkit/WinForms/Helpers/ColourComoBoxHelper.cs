using LaMarvin.Windows.Forms;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace JBToolkit.WinForms
{
    /// <summary>
    /// Column colour - to be used with the ColourPicker control
    /// </summary>
    public class ColorPickerColumn : DataGridViewColumn
    {
        public ColorPickerColumn() : base(new ColorPickerCell())
        { }

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                // Ensure that the cell used for the template is a colour picker cell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(ColorPickerCell)))
                {
                    throw new InvalidCastException("Must be a ColorPicker");
                }

                base.CellTemplate = value;
            }
        }
    }

    /// <summary>
    /// Cell colour - to be used with the ColourPicker control
    /// </summary>
    public class ColorPickerCell : DataGridViewTextBoxCell
    {
#pragma warning disable IDE0052 // Remove unread private members
        private ColorPickerControl _ColorPicker;
#pragma warning restore IDE0052 // Remove unread private members

        public ColorPickerCell()
        {
            // Pick a random 'new' colour
            var r = new Random();

            // make sure the random colour is light enough that black text is visible (hopefull)
            RandomlySelectedColour =
                ColourHelper.ChangeColorBrightness(Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)),
                    0.7f);
        }

        public Color RandomlySelectedColour { get; set; }

        public override Type EditType => typeof(ColorPickerControl);

        public override Type ValueType => typeof(Color);

        public override object DefaultNewRowValue => Color.White;

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);
            var ctl = DataGridView.EditingControl as ColorPickerControl;
            _ColorPicker = ctl;

            if (Value != null && Value.GetType() == typeof(Color))
            {
                ctl.Color = (Color)Value;
            }
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            formattedValue = null;

            base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue,
                errorText, cellStyle, advancedBorderStyle, paintParts);


            var ColorBoxRect = new Rectangle();
            var TextBoxRect = new RectangleF();
            GetDisplayLayout(cellBounds, ref ColorBoxRect, ref TextBoxRect);

            //// Draw the cell background, if specified.
            if ((paintParts & DataGridViewPaintParts.Background) ==
                DataGridViewPaintParts.Background)
            {
                SolidBrush cellBackground;

                if (value != null && value.GetType() == typeof(Color))
                {
                    cellBackground = new SolidBrush((Color)value);
                }
                else
                {
                    cellBackground = new SolidBrush(RandomlySelectedColour);
                }

                graphics.FillRectangle(cellBackground, ColorBoxRect);
                graphics.DrawRectangle(Pens.Black, ColorBoxRect);

                if (string.IsNullOrEmpty(value.ToString()))
                {
                    value = RandomlySelectedColour;
                    Value = RandomlySelectedColour;
                }

                graphics.DrawString(((Color)value).Name, cellStyle.Font, Brushes.Black, TextBoxRect);

                cellBackground.Dispose();
            }
        }

        public override object ParseFormattedValue(object formattedValue, DataGridViewCellStyle cellStyle,
            TypeConverter formattedValueTypeConverter, TypeConverter valueTypeConverter)
        {
            if (int.TryParse(formattedValue.ToString(), NumberStyles.HexNumber, null, out _))
            {
                //Hex number
                return base.ParseFormattedValue("0x" + formattedValue, cellStyle, formattedValueTypeConverter,
                    valueTypeConverter);
            }

            return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
        }

        protected virtual void GetDisplayLayout(Rectangle CellRect, ref Rectangle colorBoxRect,
            ref RectangleF textBoxRect)
        {
            const int DistanceFromEdge = 2;

            colorBoxRect.X = CellRect.X + DistanceFromEdge;
            colorBoxRect.Y = CellRect.Y + 1;
            colorBoxRect.Size = new Size((int)(1.5 * 17), CellRect.Height - 2 * DistanceFromEdge);

            // The text occupies the middle portion.
            textBoxRect = RectangleF.FromLTRB(colorBoxRect.X + colorBoxRect.Width + 5, colorBoxRect.Y + 2,
                CellRect.X + CellRect.Width - DistanceFromEdge, colorBoxRect.Y + colorBoxRect.Height);
        }
    }

    /// <summary>
    /// Colour picker control (Requires dll: LaMarvin.Windows.Forms.ColorPicker - Install-Package ColorPicker -Version 1.0.2)
    /// </summary>
    internal class ColorPickerControl : ColorPicker, IDataGridViewEditingControl
    {
        public ColorPickerControl()
        { }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get { return this; }
            set { Color = Color.Pink; }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Color.Name;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex { get; set; }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange => false;

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView { get; set; }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged { get; set; }

        // Implements the IDataGridViewEditingControl
        public Cursor EditingPanelCursor => Cursor;

        protected virtual void NotifyDataGridViewOfValueChange()
        {
            EditingControlValueChanged = true;
            if (EditingControlDataGridView != null)
            {
                EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }
        }

        protected override void OnLeave(EventArgs eventargs)
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            base.OnLeave(eventargs);
            NotifyDataGridViewOfValueChange();
        }
    }
}