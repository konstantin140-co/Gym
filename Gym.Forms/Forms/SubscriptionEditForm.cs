using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Forms.Ui;

namespace Gym.Forms.Forms;

public class SubscriptionEditForm : Form
{
    private readonly AppDbContext _db = new();
    private readonly int _id;
    private readonly ComboBox _cmbStatus = new() { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Label _lblInfo = new() { AutoSize = true, MaximumSize = new Size(360, 0), Font = UiTheme.BodyFont, Tag = "secondary" };

    public SubscriptionEditForm(int id)
    {
        _id = id;
        Text = "Абонемент";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(420, 220);
        BackColor = UiTheme.Background;

        _cmbStatus.Items.AddRange(SubscriptionStatuses.All);
        var panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
        panel.Controls.Add(_lblInfo);
        _lblInfo.Location = new Point(0, 0);
        panel.Controls.Add(new Label { Text = "Статус:", Location = new Point(0, 80), AutoSize = true, ForeColor = UiTheme.TextSecondary });
        _cmbStatus.Location = new Point(0, 100);

        var btnSave = new Button { Text = "Сохранить" };
        UiTheme.StylePrimaryButton(btnSave);
        btnSave.Click += (_, _) => Save();
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Height = 48, Padding = new Padding(8) };
        buttons.Controls.Add(new Button { Text = "Отмена", DialogResult = DialogResult.Cancel });
        buttons.Controls.Add(btnSave);
        Controls.Add(buttons);
        Controls.Add(panel);
        FormClosed += (_, _) => _db.Dispose();

        var s = _db.Subscriptions.Find(_id);
        if (s != null)
        {
            _lblInfo.Text = $"Клиент ID: {s.ClientId}\nТип: {s.Type}\n{s.StartDate:dd.MM.yyyy} — {s.EndDate:dd.MM.yyyy}\nЦена: {s.Price:N0} ₽";
            _cmbStatus.SelectedItem = s.Status;
        }
    }

    private void Save()
    {
        var s = _db.Subscriptions.Find(_id);
        if (s == null) return;
        s.Status = _cmbStatus.SelectedItem?.ToString() ?? SubscriptionStatuses.Active;
        _db.SaveChanges();
        DialogResult = DialogResult.OK;
        Close();
    }
}
