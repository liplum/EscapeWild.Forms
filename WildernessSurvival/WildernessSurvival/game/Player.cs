using System.Collections.Generic;
using System.ComponentModel;
using static WildernessSurvival.game.Route;

namespace WildernessSurvival.game
{
    public partial class Player : INotifyPropertyChanged
    {
        public enum ValueType
        {
            Hp,
            Food,
            Water,
            Energy
        }

        private const int MaxValue = 10;

        private const float PerActStep = 0.05f;

        private Backpack _backpack;

        private int _curPositionExploreCount;
        private Route _curRoute;

        private int _energyValue;

        private int _foodValue;

        private bool _hasFireValue;

        private int _hpValue;

        private string _locationValue;

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

        public IList<ItemBase> AllItems => _backpack.AllItems;

        public IList<IRawItem> RawItems => _backpack.RawItems;

        public IList<ItemBase> HuntingTools => _backpack.HuntingTools;

        public bool HasWood => _backpack.HasWood;

        public string POS
        {
            get => _locationValue;
            private set
            {
                if (_locationValue == value) return;
                _locationValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(POS)));
            }
        }

        public int TurnCount
        {
            get => _turnNumber;
            private set => _turnNumber = value < 0 ? 0 : value;
        }

        public int HP
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HP)));
            }
        }

        public int FOOD
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FOOD)));
            }
        }

        public int WATER
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WATER)));
            }
        }

        public int ENERGY
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ENERGY)));
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

        public bool IsDead => HP <= 0 || FOOD <= 0 || WATER <= 0 || ENERGY <= 0;

        public bool IsWin => TripRatio >= 1;

        public event PropertyChangedEventHandler PropertyChanged;

        public void Modify(int delta, ValueType type)
        {
            switch (type)
            {
                case ValueType.Hp:
                    HP += delta;
                    break;
                case ValueType.Food:
                    FOOD += delta;
                    break;
                case ValueType.Water:
                    WATER += delta;
                    break;
                case ValueType.Energy:
                    ENERGY += delta;
                    break;
            }
        }

        private void AddTrip(float delta = PerActStep)
        {
            TripRatio += delta;
        }

        public void Reset()
        {
            HP = FOOD = WATER = ENERGY = MaxValue;
            HasFire = false;
            _tripRatio = 0;
            _curRoute = SubtropicsRoute;
            _curRoute.Reset();
            SetLocation(_curRoute.CurPlace);
            TurnCount = 0;
            _curPositionExploreCount = 0;
            _backpack = new Backpack(this);
        }

        public bool Use(ItemBase item)
        {
            return _backpack.Use(item);
        }

        public void Remove(ItemBase item)
        {
            _backpack.Remove(item);
        }

        public void AddItem(ItemBase item)
        {
            _backpack.AddItem(item);
        }

        private void AddItems(IList<ItemBase> items)
        {
            _backpack.Append(items);
        }

        private void SetLocation(Place NewP)
        {
            if (_location?.Name != NewP.Name)
                _curPositionExploreCount = 0;
            _location = NewP;
            POS = NewP.Name;
        }

        public void ConsumeWood(int Count)
        {
            _backpack.ConsumeWood(Count);
        }
    }
}