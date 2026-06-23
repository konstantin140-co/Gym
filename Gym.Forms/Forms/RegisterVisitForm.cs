using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Data.Helpers;
using Gym.Data.Models;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class RegisterVisitForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly ComboBox _cmbClient = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbSubscription = new() { Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly ComboBox _cmbActivity = new() { Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly DateTimePicker _dtpWhen = new() { Width = 220, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };

    public RegisterVisitForm()
    {
        SetActiveNavKey("register");
        NavigationService.NavigateTo(this, "register");

        var header = new PageHeaderControl();
        header.SetTitle("Регистрация визита", "Проверка активного абонемента");
        header.ConfigureAction("", false);
        header.HideSearch();

        _cmbActivity.Items.AddRange(ActivityTypes.All);
        _cmbActivity.SelectedIndex = 0;
        _dtpWhen.Value = DateTime.Now;
        _cmbClient.SelectedIndexChanged += (_, _) => LoadSubscriptions();

        var card = new RoundedPanel { Dock = DockStyle.Top, Height = 240, Padding = new Padding(24) };
        var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        AddRow(table, 0, "Клиент:", _cmbClient);
        AddRow(table, 1, "Абонемент:", _cmbSubscription);
        AddRow(table, 2, "Тип занятия:", _cmbActivity);
        AddRow(table, 3, "Дата и время:", _dtpWhen);
        card.Controls.Add(table);

        var btn = new Button { Text = "Зарегистрировать", Width = 160 };
        UiTheme.StylePrimaryButton(btn);
        btn.Click += (_, _) => Register();
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
        _cmbClient.DisplayMember = "FullName";
        _cmbClient.ValueMember = "Id";
        _cmbClient.DataSource = _db.Clients.OrderBy(c => c.LastName).ToList();
        LoadSubscriptions();
    }

    private void LoadSubscriptions()
    {
        if (_cmbClient.SelectedValue is not int clientId) return;
        var subs = _db.Subscriptions.Where(s => s.ClientId == clientId).AsEnumerable()
            .Where(s => SubscriptionCalculator.IsActive(s))
            .ToList();
        _cmbSubscription.DisplayMember = "Type";
        _cmbSubscription.ValueMember = "Id";
        _cmbSubscription.DataSource = subs;
    }

    private void Register()
    {
        try
        {
            if (_cmbClient.SelectedItem is not Client client)
                throw new ArgumentException("Выберите клиента");
            if (_cmbSubscription.SelectedItem is not Subscription sub)
                throw new ArgumentException("Нет активного абонемента");
            if (!SubscriptionCalculator.IsActive(sub))
                throw new ArgumentException("Абонемент не активен");

            _db.Visits.Add(new Visit
            {
                ClientId = client.Id,
                SubscriptionId = sub.Id,
                DateTime = _dtpWhen.Value,
                ActivityType = _cmbActivity.SelectedItem?.ToString() ?? ActivityTypes.Gym
            });
            _db.SaveChanges();
            MessageBox.Show($"Визит зарегистрирован: {client.FullName}", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
