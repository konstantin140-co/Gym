using Gym.Data.Context;
using Gym.Data.Models;
using Gym.Forms.Ui;

namespace Gym.Forms.Forms;

public class ClientEditForm : Form
{
    private readonly AppDbContext _db = new();
    private readonly int _id;
    private readonly TextBox _txtLastName = new() { Width = 280 };
    private readonly TextBox _txtFirstName = new() { Width = 280 };
    private readonly TextBox _txtPhone = new() { Width = 280 };
    private readonly DateTimePicker _dtpBirth = new() { Width = 200, Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker _dtpReg = new() { Width = 200, Format = DateTimePickerFormat.Short };

    public ClientEditForm(int id)
    {
        _id = id;
        Text = id == 0 ? "Добавление клиента" : "Редактирование клиента";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(440, 320);
        BackColor = UiTheme.Background;
        Font = UiTheme.BodyFont;

        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(16) };
        void Row(int r, string label, Control c)
        {
            table.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = UiTheme.TextSecondary }, 0, r);
            table.Controls.Add(c is TextBox tb ? UiTheme.WrapInput(tb) : c, 1, r);
        }
        Row(0, "Фамилия:", _txtLastName);
        Row(1, "Имя:", _txtFirstName);
        Row(2, "Телефон:", _txtPhone);
        Row(3, "Дата рождения:", _dtpBirth);
        Row(4, "Дата регистрации:", _dtpReg);

        var btnSave = new Button { Text = "Сохранить" };
        var btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel };
        UiTheme.StylePrimaryButton(btnSave);
        UiTheme.StyleSecondaryButton(btnCancel);
        btnSave.Click += (_, _) => Save();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 48, Padding = new Padding(8) };
        buttons.Controls.AddRange([btnCancel, btnSave]);
        Controls.Add(table);
        Controls.Add(buttons);
        FormClosed += (_, _) => _db.Dispose();

        if (_id != 0)
        {
            var c = _db.Clients.Find(_id);
            if (c != null)
            {
                _txtLastName.Text = c.LastName;
                _txtFirstName.Text = c.FirstName;
                _txtPhone.Text = c.Phone;
                _dtpBirth.Value = c.BirthDate;
                _dtpReg.Value = c.RegistrationDate;
            }
        }
        else
        {
            _dtpBirth.Value = DateTime.Today.AddYears(-25);
            _dtpReg.Value = DateTime.Today;
        }
    }

    private void Save()
    {
        FormValidation.ResetBackColor(_txtLastName, _txtFirstName, _txtPhone);
        if (!FormValidation.RequireText(_txtLastName) | !FormValidation.RequireText(_txtFirstName) | !FormValidation.RequireText(_txtPhone))
        {
            MessageBox.Show("Заполните обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            var item = _id == 0 ? new Client() : _db.Clients.Find(_id) ?? throw new InvalidOperationException();
            item.LastName = _txtLastName.Text.Trim();
            item.FirstName = _txtFirstName.Text.Trim();
            item.Phone = _txtPhone.Text.Trim();
            item.BirthDate = _dtpBirth.Value.Date;
            item.RegistrationDate = _dtpReg.Value.Date;
            if (_id == 0) _db.Clients.Add(item);
            _db.SaveChanges();
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
