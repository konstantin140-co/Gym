using Gym.Data.Context;
using Gym.Data.Models;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class ClientListForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly PageHeaderControl _header = new();
    private readonly CardGridPanel _cardGrid = new();
    private readonly Label _lblCount = new() { AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" };
    private readonly System.Windows.Forms.Timer _searchTimer = new() { Interval = 300 };

    public ClientListForm()
    {
        SetActiveNavKey("clients");
        NavigationService.NavigateTo(this, "clients");
        _header.SetTitle("Клиенты", "База клиентов клуба");
        _header.ConfigureAction("Добавить");

        var footer = new Panel { Dock = DockStyle.Bottom, Height = 28 };
        footer.Controls.Add(_lblCount);
        ContentPanel.Controls.Add(_cardGrid);
        ContentPanel.Controls.Add(footer);
        ContentPanel.Controls.Add(_header);

        _header.ActionClicked += (_, _) => Edit(0);
        _header.SearchTextChanged += (_, _) => { _searchTimer.Stop(); _searchTimer.Start(); };
        _searchTimer.Tick += (_, _) => { _searchTimer.Stop(); LoadData(_header.SearchBox.Text); };
        Load += (_, _) => LoadData();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void LoadData(string filter = "")
    {
        var query = _db.Clients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim();
            query = query.Where(c => c.LastName.Contains(f) || c.FirstName.Contains(f) || c.Phone.Contains(f));
        }

        var items = query.OrderBy(c => c.LastName).ToList();
        _cardGrid.ClearCards();
        foreach (var item in items)
        {
            var card = new EntityCardControl();
            card.Bind(item.Id, item.FullName, "Клиент", item.LastName,
                $"Телефон: {item.Phone}\nРегистрация: {item.RegistrationDate:dd.MM.yyyy}\nДата рождения: {item.BirthDate:dd.MM.yyyy}");
            card.PrimaryClicked += (_, id) => Edit(id);
            card.SecondaryClicked += (_, id) => Delete(id);
            _cardGrid.AddCard(card);
        }
        _lblCount.Text = $"Записей: {items.Count}";
    }

    private void Edit(int id)
    {
        using var form = new ClientEditForm(id);
        if (form.ShowDialog() == DialogResult.OK)
            LoadData(_header.SearchBox.Text);
    }

    private void Delete(int id)
    {
        try
        {
            var item = _db.Clients.Find(id);
            if (item == null) return;
            if (MessageBox.Show($"Удалить «{item.FullName}»?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;
            _db.Clients.Remove(item);
            _db.SaveChanges();
            LoadData(_header.SearchBox.Text);
        }
        catch (DbUpdateException)
        {
            MessageBox.Show("Нельзя удалить: есть связанные абонементы или посещения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
