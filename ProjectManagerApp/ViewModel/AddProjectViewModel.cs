﻿using ProjectManagerApp.Infrastructure.Commands;
using ProjectManagerApp.Model;
using ProjectManagerApp.Repositories;
using ProjectManagerApp.View;
using ProjectManagerApp.Data;
using ProjectManagerApp.ViewModel.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;

namespace ProjectManagerApp.ViewModel
{
    internal class AddProjectViewModel : ViewModelBase
    {
        private string _title = "Добавить новый проект";
        private string _projectName;
        private string _projectDescription;
        private DateTime _startDate;
        private DateTime _endDate;

        private readonly ProjectRepository _projectRepository;
        private ObservableCollection<Project> _projects;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        public string ProjectName
        {
            get => _projectName;
            set => SetProperty(ref _projectName, value);
        }
        public string ProjectDescription
        {
            get => _projectDescription;
            set => SetProperty(ref _projectDescription, value);
        }
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }
        public DateTime EndDate
        {
            get => _endDate; 
            set => SetProperty(ref _endDate, value);
        }
        public ObservableCollection<Project> Projects
        {
            get => _projects;
            set => SetProperty(ref _projects, value);
        }

        public ICommand AddProjectCommand { get; }
        public event EventHandler ProjectAdded;

        public AddProjectViewModel()
        {
            _projectRepository = new ProjectRepository(new ApplicationContext());
            AddProjectCommand = new RelayCommand(async _ => await AddProject());
            Task.Run(async () => await LoadPoject());
        }

        private async Task LoadPoject()
        {
            var projects = await _projectRepository.GetAll();
            Projects = new ObservableCollection<Project>(projects);
        }

        private async Task AddProject()
        {
            if (!string.IsNullOrWhiteSpace(ProjectName) && 
                !string.IsNullOrWhiteSpace(ProjectDescription) &&
                StartDate < EndDate)
            {
                var project = new Project()
                {
                    Name = ProjectName,
                    Description = ProjectDescription,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Status = StatusProject.InProgress
                };

                await _projectRepository.Add(project);
                await LoadPoject();
                ProjectAdded?.Invoke(this, EventArgs.Empty);

                Close();

                MessageBox.Show("Проект добавлен.",
                    "Успешно!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
            }
            else
                MessageBox.Show("Заполните все поля.",
                    "Ошибка!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
        }
        private void Close()
        {
            Application.Current.Windows.OfType<AddProjectWindow>().FirstOrDefault()?.Close();
        }
    }
}
