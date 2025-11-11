using CommunityToolkit.Mvvm.Input;
using PreDiabetes.Models;

namespace PreDiabetes.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}