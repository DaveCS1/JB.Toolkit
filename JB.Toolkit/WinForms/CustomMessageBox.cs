using JBToolkit.AssemblyHelper;
using System;
using System.Drawing;
using System.Windows.Forms;

/*
 * Developer: James Brindle
 * 24/01/2019
 */

namespace JBToolkit.WinForms
{
    /// <summary>
    /// Simplified MsgBox style dialogue using MetroForms layout
    /// </summary>
    public partial class CustomMessageBox : OptimisedMetroForm
    {
        public enum MessageTypeEnum
        {
            Info,
            Question,
            Warning,
            Danger,
            Success
        }

        public enum ButtonTypeEnum
        {
            Close,
            OKCancel,
            YesNo
        }

        public MessageTypeEnum MessageType { get; set; }
        public ButtonTypeEnum ButtonType { get; set; }
        public string TitleText { get; set; }
        public string MessageText { get; set; }

        public CustomMessageBox(string titleText, string messageText, MessageTypeEnum messageType, ButtonTypeEnum buttonType, int width = 570, int height = 245) : base(false, false, false)
        {
            TitleText = titleText;
            MessageText = messageText;
            MessageType = messageType;
            ButtonType = buttonType;

            SetStyle(
                ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.SupportsTransparentBackColor, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            InitializeComponent();

            Size = new Size(width, height);

            FormBorderStyle = FormBorderStyle.None;

            Text = TitleText;
            MessageText = messageText;
            lblMessage.Text = MessageText;

            switch (MessageType)
            {
                case MessageTypeEnum.Info:
                    Style = MetroFramework.MetroColorStyle.Blue;
                    pbIcon.Image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        "Images.cmb-info.png",
                        "Dependencies_Embedded",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                    break;
                case MessageTypeEnum.Question:
                    Style = MetroFramework.MetroColorStyle.Blue;
                    pbIcon.Image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        "Images.cmb-question.png",
                        "Dependencies_Embedded",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                    break;
                case MessageTypeEnum.Warning:
                    Style = MetroFramework.MetroColorStyle.Orange;
                    pbIcon.Image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        "Images.cmb-warning.png",
                        "Dependencies_Embedded",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                    break;
                case MessageTypeEnum.Danger:
                    Style = MetroFramework.MetroColorStyle.Red;
                    pbIcon.Image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        "Images.cmb-danger.png",
                        "Dependencies_Embedded",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                    break;
                case MessageTypeEnum.Success:
                    Style = MetroFramework.MetroColorStyle.Green;
                    pbIcon.Image = EmbeddedResourceHelper.GetEmbeddedResourceImage(
                        "Images.cmb-success.png",
                        "Dependencies_Embedded",
                        EmbeddedResourceHelper.TargetAssemblyType.Executing);
                    break;
                default:
                    break;
            }

            switch (ButtonType)
            {
                case ButtonTypeEnum.Close:
                    btnCancel.Visible = true;
                    btnCancel.Text = "Close";
                    btnOk.Visible = false;
                    break;
                case ButtonTypeEnum.OKCancel:
                    btnCancel.Visible = true;
                    btnCancel.Text = "Cancel";
                    btnOk.Visible = true;
                    btnOk.Text = "OK";
                    break;
                case ButtonTypeEnum.YesNo:
                    btnCancel.Visible = true;
                    btnCancel.Text = "No";
                    btnOk.Visible = true;
                    btnOk.Text = "Yes";
                    break;
                default:
                    break;
            }

            OnLoad(null);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (btnOk.Text == "OK")
            {
                DialogResult = DialogResult.OK;
            }
            else if (btnOk.Text == "Yes")
            {
                DialogResult = DialogResult.Yes;
            }

            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (btnCancel.Text == "Cancel" || btnCancel.Text == "Close")
            {
                DialogResult = DialogResult.Cancel;
            }
            else if (btnCancel.Text == "No")
            {
                DialogResult = DialogResult.No;
            }

            Close();
        }

        private void CustomMessageBox_Shown(object sender, EventArgs e)
        {
            Focus();
            BringToFront();
        }
    }
}
