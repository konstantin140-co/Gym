using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Data.Helpers;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class DebtorsListForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

    public DebtorsListForm()
    {
        SetActiveNavKey("debtors");
        NavigationService.NavigateTo(this, "debtors");

        var header = new PageHeaderControl();
        header.SetTitle("Должники", "Клиенты без активного абонемента");
        header.ConfigureAction("", false);
        header.HideSearch();

        _grid.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "Client", HeaderText = "Клиент", Width = 200 },
            new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Телефон", Width = 140 },
            new DataGridViewTextBoxColumn { Name = "LastSub", HeaderText = "Последний абонемент", Width = 140 },
            new DataGridViewTextBoxColumn { Name = "Ended", HeaderText = "Истёк", Width = 100 });

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
        var clients = _db.Clients.Include(c => c.Subscriptions).ToList();
        var debtors = clients.Where(c =>
            !c.Subscriptions.Any(s => SubscriptionCalculator.IsActive(s)) &&
            c.Subscriptions.Any(s => s.Status == SubscriptionStatuses.Expired || s.EndDate.Date < DateTime.Today))
            .Select(c =>
            {
                var last = c.Subscriptions.OrderByDescending(s => s.EndDate).First();
                return new { c.Id, Client = c.FullName, c.Phone, LastSub = last.Type, Ended = last.EndDate.ToString("dd.MM.yyyy") };
            }).ToList();

        _grid.Rows.Clear();
        foreach (var d in debtors)
            _grid.Rows.Add(d.Id, d.Client, d.Phone, d.LastSub, d.Ended);
    }
}
