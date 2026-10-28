using System.Text.Json;

namespace HypertenzeApp;

public partial class MainPage : ContentPage
{
    private const string StorageKey = "blood_pressure_measurements_v1";
    private List<BloodPressureMeasurement> measurements = new();

    public MainPage()
    {
        InitializeComponent();
        LoadMeasurements();
        RenderMeasurements();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (!int.TryParse(SystolicEntry.Text, out var systolic) ||
            !int.TryParse(DiastolicEntry.Text, out var diastolic) ||
            !int.TryParse(PulseEntry.Text, out var pulse) ||
            systolic < 50 || systolic > 300 || diastolic < 30 || diastolic > 200 ||
            pulse < 30 || pulse > 250 || systolic <= diastolic)
        {
            await DisplayAlert("Neplatné hodnoty", "Zadej platný systolický tlak, diastolický tlak a puls.", "OK");
            return;
        }

        measurements.Insert(0, new BloodPressureMeasurement(Guid.NewGuid(), DateTimeOffset.Now, systolic, diastolic, pulse));
        try
        {
            Preferences.Default.Set(StorageKey, JsonSerializer.Serialize(measurements));
        }
        catch (Exception)
        {
            measurements.RemoveAt(0);
            await DisplayAlert("Chyba ukládání", "Měření se nepodařilo uložit.", "OK");
            return;
        }
        SystolicEntry.Text = DiastolicEntry.Text = PulseEntry.Text = string.Empty;
        RenderMeasurements();
    }

    private void LoadMeasurements()
    {
        try
        {
            var json = Preferences.Default.Get(StorageKey, "[]");
            measurements = JsonSerializer.Deserialize<List<BloodPressureMeasurement>>(json) ?? new();
        }
        catch (Exception)
        {
            measurements = new();
        }
    }

    private void RenderMeasurements()
    {
        HistoryContainer.Children.Clear();
        LatestLabel.Text = measurements.Count == 0
            ? "Zatím žádné měření"
            : $"{measurements[0].Systolic}/{measurements[0].Diastolic} mmHg · puls {measurements[0].Pulse}/min";
        foreach (var m in measurements.Take(20))
        {
            HistoryContainer.Children.Add(new Label
            {
                Text = $"{m.MeasuredAt:dd.MM.yyyy HH:mm}  |  {m.Systolic}/{m.Diastolic} mmHg  |  {m.Pulse}/min",
                FontSize = 14,
                TextColor = Color.FromArgb("#17365D"),
                Padding = new Thickness(8)
            });
        }
    }
}

public record BloodPressureMeasurement(Guid Id, DateTimeOffset MeasuredAt, int Systolic, int Diastolic, int Pulse);
