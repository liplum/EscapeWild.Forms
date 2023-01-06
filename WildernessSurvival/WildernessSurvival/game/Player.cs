using System.Collections.Generic;
using System.ComponentModel;
using static WildernessSurvival.game.Route;

namespace WildernessSurvival.game
{
    public enum AttrType
    {
        Hp,
        Food,
        Water,
        Energy
    }

    public partial class Player : INotifyPropertyChanged
    {
        private const int MaxValue = 10;

        private const float PerActStep = 0.05f;

        private Backpack _backpack;

        private int _curPositionExploreCount;
        private Route _curRoute;

        private int _energyValue;

        private int _foodValue;

        private bool _hasFireValue;

        private int _hpValue;

        private Place _location;

        private float _tripRatio;

        private int _turnNumber;

        private int _waterValue;

        public Player()
        {
            if (ExploreActions == null)
                RegisterExplore();
            Reset();
        }

        public IList<IItem> AllItems => _backpack.AllItems;

        public IList<IRawItem> RawItems => _backpack.RawItems;

        public IList<IItem> HuntingTools => _backpack.HuntingTools;

        public bool HasWood => _backpack.HasWood;

        public Place Location
        {
            get => _location;
            private set
            {
                if (_location != value) _curPositionExploreCount = 0;
                _location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocationName)));
            }
        }

        public string LocationName => Location.Name;

        public int TurnCount
        {
            get => _turnNumber;
            private set => _turnNumber = value < 0 ? 0 : value;
        }

        public int Hp
        {
            get => _hpValue;
            private set
            {
                if (_hpValue == value) return;
                if (value > MaxValue)
                    _hpValue = MaxValue;
                else if (value < 0)
                    _hpValue = 0;
                else
                    _hpValue = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Hp)));
            }
        }

        public int Food
        {
            get => _foodValue;
            private set
            {
                if (_foodValue == value) return;
                if (value > MaxValue)
                    _foodValue = MaxValue;
                else if (value < 0)
                    _foodValue = 0;
                else
                    _foodValue = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Food)));
            }
        }

        public int Water
        {
            get => _waterValue;
            private set
            {
                if (_waterValue == value) return;
                if (value > MaxValue)
                    _waterValue = MaxValue;
                else if (value < 0)
                    _waterValue = 0;
                else
                    _waterValue = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Water)));
            }
        }

        public int Energy
        {
            get => _energyValue;
            private set
            {
                if (_energyValue == value) return;
                if (value > MaxValue)
                    _energyValue = MaxValue;
                else if (value < 0)
                    _energyValue = 0;
                else
                    _energyValue = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Energy)));
            }
        }

        public float TripRatio
        {
            get => _tripRatio;
            private set
            {
                if (value < 0)
                    _tripRatio = 0;
                else if (value > 1)
                    _tripRatio = 1;
                else
                    _tripRatio = value;
            }
        }

        /// <summary>
        ///     是否生火
        /// </summary>
        public bool HasFire
        {
            get => _hasFireValue;
            set
            {
                if (_hasFireValue == value) return;
                _hasFireValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasFire)));
            }
        }

        private bool CanFish => _backpack.HasFishRod;

        private bool HasOxe => _backpack.HasOxe;

        private bool CanHunt => _backpack.HasHunting;

        public bool IsDead => Hp <= 0 || Food <= 0 || Water <= 0 || Energy <= 0;

        public bool IsWin => TripRatio >= 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Modify(int delta, AttrType type)
        {
            switch (type)
            {
                case AttrType.Hp:
                    Hp += delta;
                    break;
                case AttrType.Food:
                    Food += delta;
                    break;
                case AttrType.Water:
                    Water += delta;
                    break;
                case AttrType.Energy:
                    Energy += delta;
                    break;
            }
        }

        private void AddTrip(float delta = PerActStep)
        {
            TripRatio += delta;
        }

        public void Reset()
        {
            Hp = Food = Water = Energy = MaxValue;
            HasFire = false;
            _tripRatio = 0;
            _curRoute = SubtropicsRoute;
            _curRoute.Reset();
            Location = _curRoute.CurPlace;
            TurnCount = 0;
            _curPositionExploreCount = 0;
            _backpack = new Backpack(this);
        }

        public bool Use(IItem item)
        {
            return _backpack.Use(item);
        }

        public void Remove(IItem item)
        {
            _backpack.Remove(item);
        }

        public void AddItem(IItem item)
        {
            _backpack.AddItem(item);
        }

        private void AddItems(IList<IItem> items)
        {
            _backpack.Append(items);
        }


        public void ConsumeWood(int Count)
        {
            _backpack.ConsumeWood(Count);
        }
    }
}