using System.Collections.Generic;
using System.ComponentModel;
using static WildernessSurvival.game.Route;

namespace WildernessSurvival.game {
    public partial class Player : INotifyPropertyChanged {
        private Route CurRoute;

        private const int MAX_VALUE = 10;

        public const float PER_ACT_STEP = 0.05f;

        private Place Position;

        private int CurPositionExploreCount = 0;

        private Backpack Backpack;

        public IList<ItemBase> AllItems => Backpack.AllItems;

        public IList<IRawItem> RawItems => Backpack.RawItems;

        public IList<ItemBase> HuntingTools => Backpack.HuntingTools;

        public bool HasWood => Backpack.HasWood;

        private string POS_Value;

        public string POS {
            get => POS_Value;
            private set {
                if (POS_Value != value) {
                    POS_Value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(POS)));
                }
            }
        }

        private int TurnCount_Value;

        public int TurnCount {
            get => TurnCount_Value;
            private set => TurnCount_Value = value < 0 ? 0 : value;
        }

        private int HP_Value;

        public int HP {
            get => HP_Value;
            private set {
                if (HP_Value != value) {
                    if (value > MAX_VALUE)
                        HP_Value = MAX_VALUE;
                    else if (value < 0)
                        HP_Value = 0;
                    else
                        HP_Value = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HP)));
                }
            }
        }
        private int FOOD_Value;

        public int FOOD {
            get => FOOD_Value;
            private set {
                if (FOOD_Value != value) {
                    if (value > MAX_VALUE)
                        FOOD_Value = MAX_VALUE;
                    else if (value < 0)
                        FOOD_Value = 0;
                    else
                        FOOD_Value = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FOOD)));
                }
            }
        }

        private int WATER_Value;

        public int WATER {
            get => WATER_Value;
            private set {
                if (WATER_Value != value) {
                    if (value > MAX_VALUE)
                        WATER_Value = MAX_VALUE;
                    else if (value < 0)
                        WATER_Value = 0;
                    else
                        WATER_Value = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WATER)));
                }
            }
        }

        private int ENERGY_Value;

        public int ENERGY {
            get => ENERGY_Value;
            private set {
                if (ENERGY_Value != value) {
                    if (value > MAX_VALUE)
                        ENERGY_Value = MAX_VALUE;
                    else if (value < 0)
                        ENERGY_Value = 0;
                    else
                        ENERGY_Value = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ENERGY)));
                }
            }
        }

        private float Trip_Value;

        public float Trip {
            get => Trip_Value;
            private set {
                if (value < 0)
                    Trip_Value = 0;
                else if (value > 1)
                    Trip_Value = 1;
                else
                    Trip_Value = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool HasFire_Value;

        /// <summary>
        /// 是否生火
        /// </summary>
        public bool HasFire {
            get => HasFire_Value;
            set {
                if (HasFire_Value != value) {
                    HasFire_Value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasFire)));
                }
            }
        }

        public bool CanFish => Backpack.HasFishRod;

        public bool HasOxe => Backpack.HasOxe;

        public bool CanHunt => Backpack.HasHunting;

        public bool IsDead => HP <= 0 || FOOD <= 0 || WATER <= 0 || ENERGY <= 0;

        public bool IsWin => Trip >= 1;

        public Player() {
            if (ExploreActions == null)
                RegisterExplore();
            Reset();
        }

        public enum ValueType {
            HP, FOOD, WATER, ENERGY
        }

        public void Modify(int delta, ValueType type) {
            switch (type) {
                case ValueType.HP:
                    HP += delta;
                    break;
                case ValueType.FOOD:
                    FOOD += delta;
                    break;
                case ValueType.WATER:
                    WATER += delta;
                    break;
                case ValueType.ENERGY:
                    ENERGY += delta;
                    break;
            }
        }

        public void AddTrip(float delta) {
            Trip += delta;
        }

        public void AddTrip() {
            AddTrip(PER_ACT_STEP);
        }

        public void Reset() {
            HP = FOOD = WATER = ENERGY = MAX_VALUE;
            HasFire = false;
            Trip_Value = 0;
            CurRoute = Route.SubtropicsRoute;
            CurRoute.Reset();
            SetPostion(CurRoute.CurPlace);
            TurnCount = 0;
            CurPositionExploreCount = 0;
            Backpack = new Backpack(this);
        }
        public bool Use(ItemBase item) {
            return Backpack.Use(item);
        }
        public void Remove(ItemBase item) {
            Backpack.Remove(item);
        }
        public void AddItem(ItemBase item) {
            Backpack.AddItem(item);
        }
        public void AddItems(IList<ItemBase> items) {
            Backpack.Append(items);
        }

        private void SetPostion(Place NewP) {
            if (Position?.Name != NewP.Name)
                CurPositionExploreCount = 0;
            Position = NewP;
            POS = NewP.Name;
        }

        public void ConsumeWood(int Count) {
            Backpack.ConsumeWood(Count);
        }
    }
}
