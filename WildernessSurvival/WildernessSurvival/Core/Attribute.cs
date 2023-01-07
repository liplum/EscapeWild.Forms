using System;

namespace WildernessSurvival.Core
{
    public enum AttrType
    {
        Health,
        Food,
        Water,
        Energy
    }

    public interface IAttributeModel
    {
        public float Health { get; set; }
        public float Food { get; set; }
        public float Water { get; set; }
        public float Energy { get; set; }
    }

    public class AttributeManager
    {
        public const float MaxValue = 1f;
        public const float UnderflowPunishmentRadio = 2f;
        private readonly IAttributeModel _model;

        public AttributeManager(IAttributeModel model)
        {
            _model = model;
        }

        /// <summary>
        /// If the result should be is more than <see cref="MaxValue"/>, the <param name="delta"></param> will be attenuated based on overflow.
        /// </summary>
        public void Modify(AttrType attr, float delta)
        {
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
                if (after < 0f)
                {
                    var underflow = Math.Abs(after);
                    switch (attr)
                    {
                        case AttrType.Food:
                        case AttrType.Water:
                        case AttrType.Energy:
                            SetAttr(attr, 0);
                            SetAttr(AttrType.Health,GetAttr(AttrType.Health) - underflow * UnderflowPunishmentRadio);
                            break;
                    }
                }
                else
                {
                    SetAttr(attr, after);
                }
            }
        }

        public void SetAttr(AttrType attr, float value)
        {
            switch (attr)
            {
                case AttrType.Health:
                    _model.Health = Math.Min(value, 1f);
                    break;
                case AttrType.Food:
                    _model.Food = value;
                    break;
                case AttrType.Water:
                    _model.Water = value;
                    break;
                case AttrType.Energy:
                    _model.Energy = value;
                    break;
            }
        }

        public float GetAttr(AttrType attr)
        {
            return attr switch
            {
                AttrType.Health => _model.Health,
                AttrType.Food => _model.Food,
                AttrType.Water => _model.Water,
                _ => _model.Energy
            };
        }
    }

    public class DefaultAttributeModel : IAttributeModel
    {
        public float Health { get; set; }
        public float Food { get; set; }
        public float Water { get; set; }
        public float Energy { get; set; }
    }
}