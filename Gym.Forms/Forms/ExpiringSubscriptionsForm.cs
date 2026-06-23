using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Data.Helpers;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class ExpiringSubscriptionsForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

    public ExpiringSubscriptionsForm()
    {
        SetActiveNavKey("expiring");
        NavigationService.NavigateTo(this, "expiring");

        var header = new PageHeaderControl();
        header.SetTitle("Истекающие абонементы", "Менее 7 дней до окончания");
        header.ConfigureAction("", false);
        header.HideSearch();

        _grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "Client", HeaderText = "Клиент", Width = 180 },
            new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Тип", Width = 100 },
            new DataGridViewTextBoxColumn { Name = "EndDate", HeaderText = "Окончание", Width = 110 },
            new DataGridViewTextBoxColumn { Name = "DaysLeft", HeaderText = "Осталось дней", Width = 110 });

        var card = new RoundedPanel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        card.Controls.Add(_grid);
        ContentPanel.Controls.Add(card);
        ContentPanel.Controls.Add(header);

        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
        DataGridViewSortHelper.Attach(_grid);
    }

    private void LoadData()
    {
        var items = _db.Subscriptions.Include(s => s.Client).AsEnumerable()
            .Where(s => SubscriptionCalculator.IsExpiringSoon(s))
            .OrderBy(s => s.EndDate)
            .Select(s => new
            {
                s.Id,
                Client = s.Client.FullName,
                s.Type,
                EndDate = s.EndDate.ToString("dd.MM.yyyy"),
                DaysLeft = SubscriptionCalculator.DaysUntilExpiry(s)
            }).ToList();

        _grid.Rows.Clear();
        foreach (var item in items)
            _grid.Rows.Add(item.Id, item.Client, item.Type, item.EndDate, item.DaysLeft);
    }
}
