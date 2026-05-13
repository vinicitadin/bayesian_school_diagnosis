using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BayesianDiagnosis.Netica;

namespace BayesianDiagnosis.ViewModels;

public sealed class DiagnosisViewModel : INotifyPropertyChanged, IDisposable
{
    private const string ResultNodeName = "desempenho";

    private static readonly IReadOnlyDictionary<string, string> InputNodeNames = new Dictionary<string, string>
    {
        [nameof(StudyHours)] = "horas_estudo",
        [nameof(SleepQuality)] = "qualidade_sono",
        [nameof(ClassAttendance)] = "frequencia",
        [nameof(SocialMediaUse)] = "uso_redes_sociais",
        [nameof(StressLevel)] = "nivel_estresse",
        [nameof(Diet)] = "alimentacao",
        [nameof(StudyEnvironment)] = "ambiente_de_estudo",
        [nameof(Motivation)] = "motivacao"
    };

    private readonly NeticaEnvironment _env;
    private readonly NeticaNetwork _net;

    private string? _studyHours;
    private string? _sleepQuality;
    private string? _classAttendance;
    private string? _socialMediaUse;
    private string? _stressLevel;
    private string? _diet;
    private string? _studyEnvironment;
    private string? _motivation;
    private string? _result;

    public DiagnosisViewModel()
    {
        var license = Environment.GetEnvironmentVariable("NETICA_LICENSE") ?? string.Empty;

        _env = new NeticaEnvironment(license);
        _env.Initialize();

        var netPath = Path.Combine(AppContext.BaseDirectory, "diagnosis_network.dne");
        _net = _env.LoadNetwork(netPath);

        CalculateCommand = new RelayCommand(Calculate);
    }

    public string? StudyHours
    {
        get => _studyHours;
        set => SetField(ref _studyHours, value);
    }

    public string? SleepQuality
    {
        get => _sleepQuality;
        set => SetField(ref _sleepQuality, value);
    }

    public string? ClassAttendance
    {
        get => _classAttendance;
        set => SetField(ref _classAttendance, value);
    }

    public string? SocialMediaUse
    {
        get => _socialMediaUse;
        set => SetField(ref _socialMediaUse, value);
    }

    public string? StressLevel
    {
        get => _stressLevel;
        set => SetField(ref _stressLevel, value);
    }

    public string? Diet
    {
        get => _diet;
        set => SetField(ref _diet, value);
    }

    public string? StudyEnvironment
    {
        get => _studyEnvironment;
        set => SetField(ref _studyEnvironment, value);
    }

    public string? Motivation
    {
        get => _motivation;
        set => SetField(ref _motivation, value);
    }

    public string? Result
    {
        get => _result;
        set => SetField(ref _result, value);
    }

    public ICommand CalculateCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void Dispose()
    {
        _net.Dispose();
        _env.Dispose();
    }

    private void Calculate()
    {
        var inputs = new Dictionary<string, string?>
        {
            [InputNodeNames[nameof(StudyHours)]] = StudyHours,
            [InputNodeNames[nameof(SleepQuality)]] = SleepQuality,
            [InputNodeNames[nameof(ClassAttendance)]] = ClassAttendance,
            [InputNodeNames[nameof(SocialMediaUse)]] = SocialMediaUse,
            [InputNodeNames[nameof(StressLevel)]] = StressLevel,
            [InputNodeNames[nameof(Diet)]] = Diet,
            [InputNodeNames[nameof(StudyEnvironment)]] = StudyEnvironment,
            [InputNodeNames[nameof(Motivation)]] = Motivation
        };

        foreach (var input in inputs)
        {
            if (string.IsNullOrWhiteSpace(input.Value))
            {
                Result = "Preencha todos os campos.";
                return;
            }
        }

        try
        {
            _net.RetractFindings();

            foreach (var input in inputs)
            {
                _net.SetFinding(input.Key, input.Value!);
            }

            var beliefs = _net.GetBeliefs(ResultNodeName);
            var (bestState, bestProb) = GetMaxBelief(beliefs);

            Result = $"Mais provável: {bestState} ({bestProb:P2})";
        }
        catch (Exception ex)
        {
            Result = $"Erro ao calcular: {ex.Message}";
        }
    }

    private static (string State, float Prob) GetMaxBelief(IReadOnlyDictionary<string, float> beliefs)
    {
        var bestState = string.Empty;
        var bestProb = -1f;

        foreach (var item in beliefs)
        {
            if (item.Value > bestProb)
            {
                bestState = item.Key;
                bestProb = item.Value;
            }
        }

        return (bestState, bestProb);
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}