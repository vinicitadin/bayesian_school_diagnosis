using System.Windows;
using BayesianDiagnosis.ViewModels;

namespace BayesianDiagnosis;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new DiagnosisViewModel();
    }

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is DiagnosisViewModel vm)
        {
            vm.Dispose();
        }

        base.OnClosed(e);
    }
}