using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Data.Helpers;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class SubscriptionListForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly FilterTabsControl _filterTabs = new();
    private readonly CardGridPanel _cardGrid = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };
    private string? _statusFilter;

    public SubscriptionListForm()
    {
        SetActiveNavKey("subscriptions");
        NavigationService.NavigateTo(this, "subscriptions");
        _header.SetTitle("Абонементы", "Проданные абонементы клиентов");
        _header.ConfigureAction("Продать");
        _filterTabs.SetTabs("Все", SubscriptionStatuses.Active, SubscriptionStatuses.Expired, SubscriptionStatuses.Cancelled);

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28 };
        footer.Controls.Add(_lblCount);
        ContentPanel.Controls.Add(_cardGrid);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_filterTabs);
        ContentPanel.Controls.Add(_header);

        _header.ActionClicked += (_, _) => NavigationService.NavigateTo(new SellSubscriptionForm(), "sell");
        _header.SearchTextChanged += (_, _) => { _searchTimer.Stop(); _searchTimer.Start(); };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); LoadData(_header.SearchBox.Text); };
        _filterTabs.FilterChanged += (_, s) => { _statusFilter = s; LoadData(_header.SearchBox.Text); };
        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadData(string filter = "")
    {
        var query = _db.Subscriptions.Include(s => s.Client).AsQueryable();
        if (!string.IsNullOrEmpty(_statusFilter))
            query = query.Where(s => s.Status == _statusFilter);
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(s => s.Client.LastName.Contains(f) || s.Client.FirstName.Contains(f) || s.Type.Contains(f));
        }

        var items = query.OrderByDescending(s => s.StartDate).ToList();
        _cardGrid.ClearCards();
        foreach (var item in items)
        {
            var badge = item.Status;
            if (SubscriptionCalculator.IsExpiringSoon(item))
                badge = "Истекает";
            var card = new EntityCardControl();
            card.Bind(item.Id, item.Client.FullName, badge, item.Type,
                $"Тип: {item.Type}\nПериод: {item.StartDate:dd.MM.yyyy} — {item.EndDate:dd.MM.yyyy}\nЦена: {item.Price:N0} ₽",
                "Открыть", "Удалить");
            card.PrimaryClicked += (_, id) => Edit(id);
            card.SecondaryClicked += (_, id) => Delete(id);
            _cardGrid.AddCard(card);
        }
        _lblCount.Text = $"Записей: {items.Count}";
    }

    private void Edit(int id)
    {
        using var form = new SubscriptionEditForm(id);
        if (form.ShowDialog() == DialogResult.OK)
            LoadData(_header.SearchBox.Text);
    }

    private void Delete(int id)
    {
        var item = _db.Subscriptions.Include(s => s.Client).FirstOrDefault(s => s.Id == id);
        if (item == null) return;
        if (MessageBox.Show($"Удалить абонемент «{item.Client.FullName} — {item.Type}»?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        try
        {
            _db.Subscriptions.Remove(item);
            _db.SaveChanges();
            LoadData(_header.SearchBox.Text);
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Нельзя удалить: есть связанные посещения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
