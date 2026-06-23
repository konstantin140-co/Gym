using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Data.Helpers;
using Gym.Data.Models;
using Gym.Forms.Ui;

namespace Gym.Forms.Forms;

public class SellSubscriptionForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly ComboBox _cmbClient = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbType = new() { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpStart = new() { Width = 200, Format = DateTimePickerFormat.Short };
    private readonly Label _lblEnd = new() { AutoSize = true, Font = UiTheme.BodyFont, Tag = "primary" };
    private readonly Label _lblPrice = new() { AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = UiTheme.Accent };

    public SellSubscriptionForm()
    {
        SetActiveNavKey("sell");
        NavigationService.NavigateTo(this, "sell");

        var header = new PageHeaderControl();
        header.SetTitle("Продажа абонемента", "Оформление нового абонемента");
        header.ConfigureAction("", false);
        header.HideSearch();

        _cmbType.Items.AddRange(SubscriptionTypes.All);
        _cmbType.SelectedIndex = 0;
        _dtpStart.Value = DateTime.Today;
        _cmbType.SelectedIndexChanged += (_, _) => UpdatePreview();
        _dtpStart.ValueChanged += (_, _) => UpdatePreview();

        var card = new RoundedPanel { Dock = DockStyle.Top, Height = 260, Padding = new Padding(24) };
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        AddRow(table, 0, "Клиент:", _cmbClient);
        AddRow(table, 1, "Тип:", _cmbType);
        AddRow(table, 2, "Дата начала:", _dtpStart);
        AddRow(table, 3, "Дата окончания:", _lblEnd);
        AddRow(table, 4, "Стоимость:", _lblPrice);
        card.Controls.Add(table);

        var btn = new Button { Text = "Оформить", Width = 140 };
        UiTheme.StylePrimaryButton(btn);
        btn.Click += (_, _) => Sell();
        var actions = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(0, 8, 0, 0) };
        actions.Controls.Add(btn);

        ContentPanel.Controls.Add(actions);
        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(header);

        Load += (_, _) => LoadClients();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private static void AddRow(TableLayoutPanel t, int row, string label, Control c)
    {
        t.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = UiTheme.TextSecondary }, 0, row);
        t.Controls.Add(c, 1, row);
    }

    private void LoadClients()
    {
        var clients = _db.Clients.OrderBy(c => c.LastName).ToList();
        _cmbClient.DisplayMember = "FullName";
        _cmbClient.ValueMember = "Id";
        _cmbClient.DataSource = clients;
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        var type = _cmbType.SelectedItem?.ToString() ?? SubscriptionTypes.Month;
        var end = SubscriptionCalculator.CalculateEndDate(type, _dtpStart.Value);
        _lblEnd.Text = end.ToString("dd.MM.yyyy");
        _lblPrice.Text = $"{SubscriptionTypes.GetPrice(type):N0} ₽";
    }

    private void Sell()
    {
        try
        {
            if (_cmbClient.SelectedItem is not Client client)
                throw new ArgumentException("Выберите клиента");
            var type = _cmbType.SelectedItem?.ToString() ?? SubscriptionTypes.Month;
            var start = _dtpStart.Value.Date;
            var sub = new Subscription
            {
                ClientId = client.Id,
                Type = type,
                StartDate = start,
                EndDate = SubscriptionCalculator.CalculateEndDate(type, start),
                Price = SubscriptionTypes.GetPrice(type),
                Status = SubscriptionStatuses.Active
            };
            _db.Subscriptions.Add(sub);
            _db.SaveChanges();
            MessageBox.Show($"Абонемент оформлен для {client.FullName}.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadClients();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
