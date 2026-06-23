namespace Gym.Forms.Ui;

internal class StatusBadgeControl : Label
{
    public StatusBadgeControl()
    {
        AutoSize = false;
        TextAlign = ContentAlignment.MiddleCenter;
        Font = UiTheme.SmallFont;
        Height = 24;
        Width = 90;
        Padding = new Padding(8, 2, 8, 2);
    }

    public void SetStatus(string status)
    {
        Text = status;
        var (back, fore) = status switch
        {
            "Активен" => (Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52)),
            "Истекает" => (Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14)),
            "Истёк" or "Должник" => (Color.FromArgb(254, 226, 226), Color.FromArgb(153, 27, 27)),
            "Отменён" => (Color.FromArgb(243, 244, 246), Color.FromArgb(75, 85, 99)),
            _ => (Color.FromArgb(243, 244, 246), UiTheme.TextSecondary)
        };
        BackColor = back;
        ForeColor = fore;
        Width = Math.Max(90, TextRenderer.MeasureText(status, Font).Width + Padding.Horizontal + 4);
    }
}
