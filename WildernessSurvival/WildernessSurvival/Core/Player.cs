using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using WildernessSurvival.Game;

namespace WildernessSurvival.Core
{
    public enum AttrType
    {
        Health,
        Food,
        Water,
        Energy
    }

    public delegate float ValueFixer(float raw);

    public partial class Player : INotifyPropertyChanged
    {
        private const float MaxValue = 1f;

        public const float MoveStep = 0.05f;

        private Backpack _backpack;

        public IRoute<IPlace> CurRoute;
        private float _healthValue;
        private float _energyValue;
        private float _foodValue;
        private float _waterValue;
        private bool _hasFire;
        private IPlace _location;
        private float _tripRatio;
        private int _actionNumber;

        public Player()
        {
            Reset();
        }

        public void Reset()
        {
            Health = Food = Water = Energy = MaxValue;
            HasFire = false;
            _tripRatio = 0;
            CurRoute = Routes.SubtropicsRoute();
            Location = CurRoute.InitialPlace;
            ActionNumber = 0;
            _backpack = new Backpack(this);
        }

        public IList<IItem> AllItems => _backpack.AllItems;

        public IEnumerable<ICookableItem> GetCookableItems() => _backpack.AllItems.OfType<ICookableItem>();

        public IEnumerable<IToolItem> GetToolsOf(ToolType type) =>
            _backpack.AllItems.OfType<IToolItem>().Where(e => e.ToolType == type);

        public bool HasToolOf(ToolType type) =>
            _backpack.AllItems.OfType<IToolItem>().Any(e => e.ToolType == type);

        public IToolItem TryGetBestToolOf(ToolType type) =>
            GetToolsOf(type).OrderByDescending(t => t.Level).FirstOrDefault();


        public bool HasWood => _backpack.AllItems.OfType<Log>().Any();

        public IPlace Location
        {
            get => _location;
            set
            {
                if (_location == value) return;
                _location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocationName)));
            }
        }

        public string LocationName => Location.LocalizedName();

        public int ActionNumber
        {
            get => _actionNumber;
            private set => _actionNumber = value < 0 ? 0 : value;
        }

        public float Health
        {
            get => _healthValue;
            private set
            {
                _healthValue = Math.Max(0, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Health)));
            }
        }

        public float Food
        {
            get => _foodValue;
            private set
            {
                _foodValue = Math.Max(0, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Food)));
            }
        }

        public float Water
        {
            get => _waterValue;
            private set
            {
                _waterValue = Math.Max(0, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Water)));
            }
        }

        public float Energy
        {
            get => _energyValue;
            private set
            {
                _energyValue = Math.Max(0, value);
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

        public bool HasFire
        {
            get => _hasFire;
            set
            {
                if (_hasFire == value) return;
                _hasFire = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasFire)));
            }
        }
        public bool IsDead => Health <= 0 || Food <= 0 || Water <= 0 || Energy <= 0;
        public bool IsAlive => !IsDead;
        public bool CanPerformAnyAction => IsAlive && !IsWon;
        public bool IsWon => TripRatio >= 1;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// If the result should be is more than <see cref="MaxValue"/>, the <param name="delta"></param> will be attenuated based on overflow.
        /// </summary>
        public void Modify(float delta, AttrType attr, ValueFixer fixer = null)
        {
            if (fixer != null)
            {
                delta = fixer(delta);
            }

            // [1] former = 0.8, delta = 0.5
            // [2] former = 1.2, delta = 0.6
            var former = GetAttr(attr);
            // [1] after = 1.3
            // [2] after = 1.8
            var after = former + delta;
            if (after > MaxValue)
            {
                // [1] restToMax = Max(0, 1 - 0.8) = 0.2
                // [2] restToMax = Max(0, 1 - 1.2) = 0
                var restToMax = Math.Max(MaxValue - former, 0f);
                // [1] extra = 0.5 - 0.2 = 0.3
                // [2] extra = 0.6 - 0.0 = 0.6
                var extra = delta - restToMax;
                // [1] after = 0.8 + 0.2 + 0.3 * 0.5^0.8 = 1.172
                // [2] after = 1.2 + 0.0 + 0.6 * 0.5^1.2 = 1.461
                after = (float)(former + restToMax + extra * Math.Pow(0.5f, former));

                SetAttr(attr, after);
            }
            else
            {
                SetAttr(attr, after);
            }
        }

        public void SetAttr(AttrType attr, float value)
        {
            switch (attr)
            {
                case AttrType.Health:
                    Health = value;
                    break;
                case AttrType.Food:
                    Food = value;
                    break;
                case AttrType.Water:
                    Water = value;
                    break;
                case AttrType.Energy:
                    Energy = value;
                    break;
            }
        }

        public float GetAttr(AttrType attr)
        {
            return attr switch
            {
                AttrType.Health => Health,
                AttrType.Food => Food,
                AttrType.Water => Water,
                _ => Energy
            };
        }

        public void AdvanceTrip(float delta = MoveStep) => TripRatio += delta;

        public void UseItem(IUsableItem item) => item.Use(this);

        public void RemoveItem(IItem item) => _backpack.RemoveItem(item);

        public void AddItem(IItem item) => _backpack.AddItem(item);

        public void AddItems(IEnumerable<IItem> items) => _backpack.AddItems(items);

        public void ConsumeWood(int Count) => _backpack.ConsumeWood(Count);
    }
}