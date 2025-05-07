using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Android.App;
using Microcharts;
using SkiaSharp;

namespace FinalProj_Tomogochi.Classes
{
    public class CharcterViewModel : INotifyPropertyChanged
    {
        private Character _activeCharacter;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ChartEntry> BGEntries { get; set; } = new ObservableCollection<ChartEntry>();

        public CharcterViewModel()
        {
        }

        public Character ActiveCharacter
        {
            get => _activeCharacter;
            set
            {
                if (_activeCharacter != value)
                {
                    _activeCharacter = value;
                    OnPropertyChanged(nameof(ActiveCharacter));
                    LoadBGEntriesFromCharacter(); // sync when setting ActiveCharacter
                }
            }
        }

        public void UpdateCharacterBloodSugar()
        {
            ActiveCharacter.UpdateBG();

            var newEntry = new ChartEntry(ActiveCharacter.CurrentBG)
            {
                Label = DateTime.Now.ToString("HH:mm"),
                ValueLabel = ActiveCharacter.CurrentBG.ToString(),
                Color = SKColor.Parse(ActiveCharacter.GetColorString(ActiveCharacter.CurrentBG, Application.Context))
            };

            if (BGEntries.Count >= 10)
                BGEntries.RemoveAt(0);

            BGEntries.Add(newEntry);

            // Update character list too (optional sync)
            ActiveCharacter.UpdateBG_List(newEntry);

            OnPropertyChanged(nameof(BGEntries)); // may not be necessary if ObservableCollection updates properly
        }

        private void LoadBGEntriesFromCharacter()
        {
            BGEntries.Clear();
            if (ActiveCharacter?.LastBGs != null)
            {
                foreach (var entry in ActiveCharacter.LastBGs)
                {
                    BGEntries.Add(entry);
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
