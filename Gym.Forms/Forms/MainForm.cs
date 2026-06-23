using Gym.Data.Constants;
using Gym.Data.Context;
using Gym.Data.Helpers;
using Gym.Forms.Ui;
using Microsoft.EntityFrameworkCore;

namespace Gym.Forms.Forms;

public class MainForm : RightSidebarShellForm
{
    private readonly AppDbContext _db = new();
    private readonly Label _lblClients = CreateStatValue();
    private readonly Label _lblActiveSubs = CreateStatValue();
    private readonly Label _lblVisitsToday = CreateStatValue();

    public MainForm()
    {
        SetActiveNavKey("home");
        NavigationService.NavigateTo(this, "home");
        BuildDashboard();
        Load += (_, _) => LoadStats();
        FormClosed += (_, _) => _db.Dispose();
        ApplyTheme();
    }

    private void BuildDashboard()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Color.Transparent,
            Tag = "theme-bg"
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Спортклуб", Font = UiTheme.TitleFont, AutoSize = true, Tag = "primary", Margin = new Padding(0, 0, 0, 8) }, 0, 0);
        layout.Controls.Add(new Label { Text = "Панель управления клубом", Font = UiTheme.BodyFont, AutoSize = true, Tag = "secondary", Margin = new Padding(0, 0, 0, 16) }, 0, 1);

        var stats = new FlowLayoutPanel { AutoSize = true, WrapContents = true, BackColor = Color.Transparent, Margin = new Padding(0, 0, 0, 24) };
        stats.Controls.Add(CreateStatTile("Клиенты", _lblClients, UiTheme.Accent));
        stats.Controls.Add(CreateStatTile("Активные абонементы", _lblActiveSubs, Color.FromArgb(34, 197, 94)));
        stats.Controls.Add(CreateStatTile("Визитов сегодня", _lblVisitsToday, Color.FromArgb(59, 130, 246)));
        layout.Controls.Add(stats, 0, 2);

        var links = new FlowLayoutPanel { AutoSize = true, WrapContents = true, BackColor = Color.Transparent };
        links.Controls.Add(CreateLinkCard("Клиенты", "clients", "К"));
        links.Controls.Add(CreateLinkCard("Абонементы", "subscriptions", "А"));
        links.Controls.Add(CreateLinkCard("Продажа", "sell", "П"));
        layout.Controls.Add(links, 0, 3);

        ContentPanel.Controls.Add(layout);
    }

    private static Label CreateStatValue() => new() { AutoSize = true, Font = new Font("Segoe UI", 26F, FontStyle.Bold), Tag = "accent" };

    private static RoundedPanel CreateStatTile(string title, Label value, Color accent)
    {
        var card = new RoundedPanel { Size = new Size(240, 100), Margin = new Padding(0, 0, 16, 16) };
        card.Controls.Add(new Panel { Size = new Size(4, 100), BackColor = accent, Location = new Point(0, 0) });
        card.Controls.Add(new Label { Text = title, Location = new Point(16, 16), AutoSize = true, Font = UiTheme.SmallFont, Tag = "secondary" });
        value.Location = new Point(16, 44);
        card.Controls.Add(value);
        return card;
    }

    private RoundedPanel CreateLinkCard(string title, string navKey, string letter)
    {
        var card = new RoundedPanel { Size = new Size(200, 120), Margin = new Padding(0, 0, 16, 16), Cursor = Cursors.Hand };
        var avatar = new Label { Text = letter, Size = new Size(36, 36), Location = new Point(16, 16), TextAlign = ContentAlignment.MiddleCenter, BackColor = UiTheme.Accent, ForeColor = Color.White, Font = new Font("Segoe UI", 12F, FontStyle.Bold) };
        var titleLbl = new Label { Text = title, Location = new Point(16, 62), AutoSize = true, Font = new Font("Segoe UI", 11F, FontStyle.Bold), Tag = "primary" };
        void open(object? _, EventArgs __) => NavigationService.NavigateTo(CreateFormForNavKey(navKey)!, navKey);
        card.Click += open; avatar.Click += open; titleLbl.Click += open;
        card.Controls.AddRange([avatar, titleLbl]);
        return card;
    }

    private void LoadStats()
    {
        var today = DateTime.Today;
        _lblClients.Text = _db.Clients.Count().ToString();
        _lblActiveSubs.Text = _db.Subscriptions.AsEnumerable().Count(s => SubscriptionCalculator.IsActive(s)).ToString();
        _lblVisitsToday.Text = _db.Visits.Count(v => v.DateTime.Date == today).ToString();
    }
}
