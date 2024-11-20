using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.WinUi.Components.Controls.Buttons;
using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage.Pickers;

namespace LuckyProject.Lib.WinUi.Components.Controls
{
    public enum FsPickerMode
    {
        SelectSingleDir,
        SelectSingleFile,
        SelectMultipleDirs,
        SelectMultipleFiles,
        SaveSelectSingleFile
    }

    public class FsPickerLocalization
    {
        public static readonly FsPickerLocalization Default = new()
        {
            Select = "Select",
            Dir = "Directory",
            Dirs = "Directories",
            File = "File",
            Files = "Files"
        };

        public string Select { get; set; }
        public string Dir { get; set; }
        public string Dirs { get; set; }
        public string File { get; set; }
        public string Files { get; set; }
    }

    public class FsPickerOptions
    {
        public string SettingsIdentifier { get; set; }
        public PickerViewMode ViewMode { get; set; } = PickerViewMode.List;
        public PickerLocationId? SuggestedStartLocation { get; set; }
        public List<string> SelectFileTypeFilter { get; set; } = ["*"];
        public Dictionary<string, List<string>> SaveFileTypeChoices { get; set; } = new();
        public string SaveSuggestedFileName { get; set; }
    }

    public sealed partial class FsPicker : UserControl
    {
        public FsPicker()
        {
            InitializeComponent();
            Update(Localization, Mode, SelectedSingle, SelectedMultiple);
        }

        #region DependancyProperties
        #region Localization
        public static readonly DependencyProperty LocalizationProperty =
            DependencyProperty.Register(
                nameof(Localization),
                typeof(FsPickerLocalization),
                typeof(FsPicker),
                new PropertyMetadata(FsPickerLocalization.Default, OnLocalizationPropertyChanged));
        public FsPickerLocalization Localization
        {
            get => (FsPickerLocalization)GetValue(LocalizationProperty);
            set => SetValue(LocalizationProperty, value);
        }

        private static void OnLocalizationPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (FsPicker)d;
            var newValue = (FsPickerLocalization)e.NewValue;
            self.Update(newValue, self.Mode, self.SelectedSingle, self.SelectedMultiple);
        }
        #endregion

        #region Mode
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                nameof(Mode),
                typeof(FsPickerMode),
                typeof(FsPicker),
                new PropertyMetadata(FsPickerMode.SelectSingleFile, OnModePropertyChanged));
        public FsPickerMode Mode
        {
            get => (FsPickerMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        private static void OnModePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (FsPicker)d;
            var newValue = (FsPickerMode)e.NewValue;
            self.Update(self.Localization, newValue, self.SelectedSingle, self.SelectedMultiple);
        }
        #endregion

        #region Options
        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(
                nameof(Options),
                typeof(FsPickerOptions),
                typeof(FsPicker),
                new PropertyMetadata(new FsPickerOptions()));
        public FsPickerOptions Options
        {
            get => (FsPickerOptions)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }
        #endregion

        #region DialogService
        public static readonly DependencyProperty DialogServiceProperty =
            DependencyProperty.Register(
                nameof(DialogService),
                typeof(ILpDialogService),
                typeof(FsPicker),
                new PropertyMetadata(null));
        public ILpDialogService DialogService
        {
            get => (ILpDialogService)GetValue(DialogServiceProperty);
            set => SetValue(DialogServiceProperty, value);
        }
        #endregion

        #region SelectedSingle
        public static readonly DependencyProperty SelectedSingleProperty =
            DependencyProperty.Register(
                nameof(SelectedSingle),
                typeof(string),
                typeof(FsPicker),
                new PropertyMetadata(null, OnSelectedSinglePropertyChanged));
        public string SelectedSingle
        {
            get => (string)GetValue(SelectedSingleProperty);
            set => SetValue(SelectedSingleProperty, value);
        }

        private static void OnSelectedSinglePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (FsPicker)d;
            var newValue = (string)e.NewValue;
            self.Update(self.Localization, self.Mode, newValue, self.SelectedMultiple);
        }
        #endregion

        #region SelectedMultiple
        public static readonly DependencyProperty SelectedMultipleProperty =
            DependencyProperty.Register(
                nameof(SelectedMultiple),
                typeof(ObservableCollection<string>),
                typeof(FsPicker),
                new PropertyMetadata(
                    new ObservableCollection<string>(),
                    OnSelectedMultiplePropertyChanged));
        public ObservableCollection<string> SelectedMultiple
        {
            get => (ObservableCollection<string>)GetValue(SelectedMultipleProperty);
            set => SetValue(SelectedMultipleProperty, value);
        }

        private static void OnSelectedMultiplePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (FsPicker)d;
            var newValue = (ObservableCollection<string>)e.NewValue;
            self.Update(self.Localization, self.Mode, self.SelectedSingle, newValue);
        }
        #endregion

        #region HasSelection
        public static readonly DependencyProperty HasSelectionProperty =
            DependencyProperty.Register(
                nameof(HasSelection),
                typeof(bool),
                typeof(FsPicker),
                new PropertyMetadata(false));
        public bool HasSelection
        {
            get => (bool)GetValue(HasSelectionProperty);
            set => SetValue(HasSelectionProperty, value);
        }
        #endregion
        #endregion

        #region Internals
        private void Update(
            FsPickerLocalization localization,
            FsPickerMode mode,
            string selectedSingle,
            ObservableCollection<string> selectedMultiple)
        {
            UpdateCaption(localization, mode);
            var singleSelectionMode = mode == FsPickerMode.SelectSingleDir ||
                mode == FsPickerMode.SelectSingleFile ||
                mode == FsPickerMode.SaveSelectSingleFile;
            if (singleSelectionMode)
            {
                ibSelectAdd.Glyph = "\uF0AF";
                tbSelectedSingle.Text = selectedSingle;
                lvSelectMultiple.Visibility = Visibility.Collapsed;
                tbSelectedSingle.Visibility = Visibility.Visible;
                HasSelection = !string.IsNullOrEmpty(selectedSingle);
                ibClear.Visibility = HasSelection ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                ibSelectAdd.Glyph = "\uECC8";
                lvSelectMultiple.Visibility = Visibility.Visible;
                tbSelectedSingle.Visibility = Visibility.Collapsed;
                HasSelection = !selectedMultiple.IsNullOrEmpty();
                ibClear.Visibility = HasSelection ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void UpdateCaption(FsPickerLocalization localization, FsPickerMode mode)
        {
            tbCaption.Text = mode switch
            {
                FsPickerMode.SelectSingleDir => $"{localization.Select} {localization.Dir}:",
                FsPickerMode.SelectSingleFile => $"{localization.Select} {localization.File}:",
                FsPickerMode.SelectMultipleDirs => $"{localization.Select} {localization.Dirs}:",
                FsPickerMode.SelectMultipleFiles => $"{localization.Select} {localization.Files}:",
                FsPickerMode.SaveSelectSingleFile => $"{localization.Select} {localization.File}:",
                _ => throw new NotImplementedException()
            };
        }

        private async void ibSelectAdd_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == FsPickerMode.SelectSingleDir)
            {
                var ds = await DialogService.PickDirAsync(new()
                {
                    SettingsIdentifier = Options.SettingsIdentifier,
                    ViewMode = Options.ViewMode,
                    SuggestedStartLocation = Options.SuggestedStartLocation
                });
                if (ds != null)
                {
                    SelectedSingle = ds.Path;
                }

                return;
            }

            if (Mode == FsPickerMode.SelectSingleFile)
            {
                var fs = await DialogService.PickSingleFileAsync(new()
                {
                    SettingsIdentifier = Options.SettingsIdentifier,
                    ViewMode = Options.ViewMode,
                    SuggestedStartLocation = Options.SuggestedStartLocation,
                    FileTypeFilter = Options.SelectFileTypeFilter
                });
                if (fs != null)
                {
                    SelectedSingle = fs.Path;
                }

                return;
            }

            if (Mode == FsPickerMode.SelectMultipleDirs)
            {
                var dm = await DialogService.PickDirAsync(new()
                {
                    SettingsIdentifier = Options.SettingsIdentifier,
                    ViewMode = Options.ViewMode,
                    SuggestedStartLocation = Options.SuggestedStartLocation
                });
                if (dm != null)
                {
                    SelectedMultiple.Add(dm.Path);
                    Update(Localization, Mode, SelectedSingle, SelectedMultiple);
                }

                return;
            }

            if (Mode == FsPickerMode.SelectMultipleFiles)
            {
                var fm = await DialogService.PickMultipleFilesAsync(new()
                {
                    SettingsIdentifier = Options.SettingsIdentifier,
                    ViewMode = Options.ViewMode,
                    SuggestedStartLocation = Options.SuggestedStartLocation,
                    FileTypeFilter = Options.SelectFileTypeFilter
                });
                if (fm.Count > 0)
                {
                    fm.ForEach(f => SelectedMultiple.Add(f.Path));
                    Update(Localization, Mode, SelectedSingle, SelectedMultiple);
                }

                return;
            }

            var fss = await DialogService.PickSaveFileAsync(new()
            {
                SettingsIdentifier = Options.SettingsIdentifier,
                ViewMode = Options.ViewMode,
                SuggestedStartLocation = Options.SuggestedStartLocation,
                FileTypeChoices = Options.SaveFileTypeChoices,
                SuggestedFileName = Options.SaveSuggestedFileName
            });
            if (fss != null)
            {
                SelectedSingle = fss.Path;
            }
        }

        private void SelectMultiple_IconButton_Click(object sender, RoutedEventArgs e)
        {
            var s = (string)((IconButton)sender).DataContext;
            SelectedMultiple.Remove(s);
            Update(Localization, Mode, SelectedSingle, SelectedMultiple);
        }

        private void ibClear_Click(object sender, RoutedEventArgs e)
        {
            var mode = Mode;
            var singleSelectionMode = mode == FsPickerMode.SelectSingleDir ||
                mode == FsPickerMode.SelectSingleFile ||
                mode == FsPickerMode.SaveSelectSingleFile;
            HasSelection = false;
            if (singleSelectionMode)
            {
                SelectedSingle = null;
            }
            else
            {
                SelectedMultiple.Clear();
            }
            Update(Localization, Mode, SelectedSingle, SelectedMultiple);
        }
        #endregion
    }
}
