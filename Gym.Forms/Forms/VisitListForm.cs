using Gym.Data.Context;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class VisitListForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly CardGridPanel _cardGrid = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };

    public VisitListForm()
    {
        SetActiveNavKey("visits");
        NavigationService.NavigateTo(this, "visits");
        _header.SetTitle("Посещения", "Журнал посещений клуба");
        _header.ConfigureAction("Зарегистрировать");

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28 };
        footer.Controls.Add(_lblCount);
        ContentPanel.Controls.Add(_cardGrid);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_header);

        _header.ActionClicked += (_, _) => NavigationService.NavigateTo(new RegisterVisitForm(), "register");
        _header.SearchTextChanged += (_, _) => { _searchTimer.Stop(); _searchTimer.Start(); };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); LoadData(_header.SearchBox.Text); };
        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadData(string filter = "")
    {
        var query = _db.Visits.Include(v => v.Client).Include(v => v.Subscription).AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(v => v.Client.LastName.Contains(f) || v.ActivityType.Contains(f));
        }

        var items = query.OrderByDescending(v => v.DateTime).ToList();
        _cardGrid.ClearCards();
        foreach (var item in items)
        {
            var card = new EntityCardControl();
            card.Bind(item.Id, item.Client.FullName, item.ActivityType, item.ActivityType[..1],
                $"Дата: {item.DateTime:dd.MM.yyyy HH:mm}\nАбонемент: {item.Subscription.Type}",
                "Удалить", "");
            card.SecondaryAction.Visible = false;
            card.PrimaryClicked += (_, id) => Delete(id);
            _cardGrid.AddCard(card);
        }
        _lblCount.Text = $"Записей: {items.Count}";
    }

    private void Delete(int id)
    {
        var item = _db.Visits.Find(id);
        if (item == null) return;
        if (MessageBox.Show("Удалить запись о посещении?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;
        _db.Visits.Remove(item);
        _db.SaveChanges();
        LoadData(_header.SearchBox.Text);
    }
}
